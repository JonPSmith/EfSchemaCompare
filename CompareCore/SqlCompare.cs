#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlCompare.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CompareCore.SqlInfo;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareCore
{
    public class SqlCompare
    {
        private readonly string _refDatabaseName;
        private readonly string _toBeCheckDatabaseName;
        private readonly IList<string> _tablesToIgnore;
        private readonly bool _showMismatchedIndexsAsErrors;

        public SqlCompare(string refDatabaseName, string toBeCheckDatabaseName, string sqlTableNamesToIgnore, bool showMismatchedIndexsAsErrors)
        {
            _refDatabaseName = refDatabaseName;
            _toBeCheckDatabaseName = toBeCheckDatabaseName;
            _tablesToIgnore = sqlTableNamesToIgnore.Split(',').Select(x => x.Trim()).ToList();
            _showMismatchedIndexsAsErrors = showMismatchedIndexsAsErrors;
        }

        /// <summary>
        /// This compares two sets of SQL data looking at each table, its columns, its keys and its foreign keys.
        /// </summary>
        /// <param name="refSqlData">reference database sql data</param>
        /// <param name="toBeCheckSqlData">sql data of the database that is to be checked as matching the reference data</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareSqlToSql(SqlAllInfo refSqlData, SqlAllInfo toBeCheckSqlData)
        {
            var status = SuccessOrErrors.Success("All Ok");

            var sqlTable2Dict = toBeCheckSqlData.TableInfos.ToDictionary(x => x.CombinedName);

            foreach (var sqlTable in refSqlData.TableInfos)
            {
                if (!sqlTable2Dict.ContainsKey(sqlTable.CombinedName))
                {
                    if (!_tablesToIgnore.Contains(sqlTable.TableName))
                        //only mark as an error if the table isn't in the ignore list
                        status.AddSingleError(
                            "Missing Table: The '{0}' SQL database has a table called {1}, which is missing in the '{2}' database.",
                            _refDatabaseName, sqlTable.CombinedName, _toBeCheckDatabaseName);
                }
                else
                {
                    //has table, so compare the columns/properties
                    var sqlTable2Info = sqlTable2Dict[sqlTable.CombinedName];
                    sqlTable2Dict.Remove(sqlTable.CombinedName);

                    //we create a dict for columns in SECOND db, which we check. As we find columns we remove them
                    var sqlColsDict = sqlTable2Info.ColumnInfos.ToDictionary(x => x.ColumnName);

                    foreach (var col in sqlTable.ColumnInfos)
                    {
                        if (!sqlColsDict.ContainsKey(col.ColumnName))
                            status.AddSingleError(
                                "Missing Column: The SQL table {0} in second database does not contain a column called {1}.",
                                sqlTable.CombinedName, col.ColumnName);
                        else
                        {
                            //check the columns match
                            var colToCheck = sqlColsDict[col.ColumnName];
                            sqlColsDict.Remove(col.ColumnName);            //remove it as it has been used

                            status.Combine(CheckSqlColumn(col, colToCheck, sqlTable.CombinedName));
                        }
                    }
                    //At the end we check if any sql columns are left
                    if (sqlColsDict.Any())
                    {
                        foreach (var missingCol in sqlColsDict.Values)
                        {
                            status.AddWarning("The '{0}' database SQL table {1} has a column called {2} (type {3}), which database '{4}' did not have.",
                                 _toBeCheckDatabaseName, sqlTable.CombinedName, missingCol.ColumnName, missingCol.SqlTypeName, _refDatabaseName);
                        }
                    }
                }
            }

            //now see what SQL tables haven't been mentioned
            if (sqlTable2Dict.Any())
            {    
                foreach (var unusedTable in sqlTable2Dict.Values.Where(x => !_tablesToIgnore.Contains(x.TableName)))
                {
                    status.AddWarning("SQL database '{0}', table {1} table contained an extra table, {1}", _refDatabaseName, unusedTable.CombinedName);
                }
            }

            //Now check the foreign keys
            status.Combine( CheckForeignKey(refSqlData, toBeCheckSqlData));

            //finally compare non primary-key indexes
            status.Combine( CheckAllIndexes(refSqlData, toBeCheckSqlData));

            return status;
        }

        //-------------------------------------------------------------------------------
        //private helpers

        private ISuccessOrErrors CheckAllIndexes(SqlAllInfo refSqlData, SqlAllInfo toBeCheckSqlData)
        {
            var status = SuccessOrErrors.Success("All Ok");
            //Note: There can be multiple indexes on the same table+column. We therefore group indexes by the scheme.tabel.column to make feedback useful
            var toBeCheckedIndexDict = toBeCheckSqlData.Indexes
                .GroupBy(x => x.CombinedName).ToDictionary(x => x.Key, v => v.ToList());
            //we also do not check on tables that we will ignore
            foreach (var refIndexGroup in refSqlData.Indexes.Where(x => !_tablesToIgnore.Contains(x.TableName)).GroupBy(x => x.CombinedName))
            {
                if (!toBeCheckedIndexDict.ContainsKey(refIndexGroup.Key))
                {
                    foreach (var eachKey in refIndexGroup)
                    {
                        SetAppropriateIndexError(status, 
                            "Missing Index: The '{0}' SQL database has an index {1}, which is missing in the '{2}' database.",
                            _refDatabaseName, eachKey.ToString(), _toBeCheckDatabaseName);                   
                    }
                }
                else
                {
                    var listRef = refIndexGroup.ToList();
                    var listToCheck = toBeCheckedIndexDict[refIndexGroup.Key].ToList();
                    toBeCheckedIndexDict.Remove(refIndexGroup.Key);

                    if (listRef.Count() == 1 && listToCheck.Count() == 1)
                    {
                        //simple case
                        status.Combine(CheckSpecificIndex(listRef.First(), listToCheck.First()));
                    }
                    else if (listRef.Count() == 1 && !listToCheck.Any())
                    {
                        //simple case - missing in toBeChecked
                        SetAppropriateIndexError(status,
                            "The '{0}' database has an index {1}, which the '{2}' database did not have.",
                            _refDatabaseName, listRef.ToString(), _toBeCheckDatabaseName);
                    }
                    else
                    {
                        if (listRef.Any() && listToCheck.Any() && listRef.Last().IsPrimaryIndex &&
                            listToCheck.Last().IsPrimaryIndex)
                        {
                            //normal case is that both have a primary key, so deal with this and take out of list
                            status.Combine(CheckSpecificIndex(listRef.Last(), listToCheck.Last()));
                            listRef.Remove(listRef.Last());
                            listToCheck.Remove(listToCheck.Last());
                        }

                        //we cheat a bit here, in that we assume these are only two multiple indexes on a column: primary key and one other
                        //we assume that the indexes are in the right order to check (because of the order by in the sql request)
                        //it will work with other combinations, but might not give such good error messages.
                        var maxIndex = Math.Max(listRef.Count(), listToCheck.Count());
                        for (int i = 0; i < maxIndex; i++)
                        {
                            if (i >= listToCheck.Count())
                                SetAppropriateIndexError(status,
                                    "The '{0}' database has an index {1}, which the '{2}' database did not have.",
                                    _refDatabaseName, listRef[i].ToString(), _toBeCheckDatabaseName);
                            else if (i >= listRef.Count())
                                //Note: this is warning, as an extra index in the toBeChecked database isn't an error
                                status.AddWarning(
                                    "The '{0}' database has an index {1}, which the '{2}' database did not have.",
                                    _toBeCheckDatabaseName, listToCheck[i].ToString(), _refDatabaseName);
                            else
                                //they both have a index, so check it
                                status.Combine(CheckSpecificIndex(listRef[i], listToCheck[i]));
                        }                 
                    }
                }
            }

            if (toBeCheckedIndexDict.Any())
            {
                foreach (var missingIndex in toBeCheckedIndexDict.Values.SelectMany(x => x).Where(x => !_tablesToIgnore.Contains(x.TableName)))
                {
                    //Note: this a warning as an extra index isn't an error
                    status.AddWarning(
                        "The '{0}' database has an index {1}, which the '{2}' database did not have.",
                        _toBeCheckDatabaseName, missingIndex.ToString(), _refDatabaseName);
                }
            }

            return status;
        }

        private ISuccessOrErrors CheckSpecificIndex(SqlIndex refIndex, SqlIndex indexToCheck)
        {
            var status = SuccessOrErrors.Success("All Ok");
            if (refIndex.IsPrimaryIndex != indexToCheck.IsPrimaryIndex)
            {
                SetAppropriateIndexError(status,
                    "Index Mismatch: The '{0}' SQL database, the index on {1} is {2} primary key index," +
                    " while the index on the same table.column in SQL database {3} is {4} primary key index.",
                    _refDatabaseName, refIndex.CombinedName,
                    refIndex.IsPrimaryIndex ? "a" : "NOT a",
                    _toBeCheckDatabaseName,
                    indexToCheck.IsPrimaryIndex ? "a" : "NOT a");
            }

            if (refIndex.Clustered != indexToCheck.Clustered)
            {
                SetAppropriateIndexError(status,
                    "Index Mismatch: The '{0}' SQL database, the index on {1} is {2}clustered," +
                    " while the index on the same table.column in SQL database {3} is {4}clustered.",
                    _refDatabaseName, refIndex.CombinedName,
                    refIndex.Clustered ? "" : "NOT ",
                    _toBeCheckDatabaseName,
                    indexToCheck.Clustered ? "" : "NOT ");
            }
            if (refIndex.IsUnique != indexToCheck.IsUnique)
            {
                SetAppropriateIndexError(status,
                    "Index Mismatch: The '{0}' SQL database, index on {1} is {2}unique," +
                    " while the index on the same table.column in SQL database {3} is {4}unique.",
                    _refDatabaseName, refIndex.CombinedName,
                    refIndex.IsUnique ? "" : "NOT ",
                    _toBeCheckDatabaseName,
                    indexToCheck.IsUnique ? "" : "NOT ");
            }

            return status;
        }

        private void SetAppropriateIndexError(ISuccessOrErrors status, string format, params string [] args)
        {
            if (_showMismatchedIndexsAsErrors)
                status.AddSingleError(format, args);
            else
                status.AddWarning(format, args);
        }

        private ISuccessOrErrors CheckForeignKey(SqlAllInfo refSqlData, SqlAllInfo toBeCheckSqlData)
        {

            var status = SuccessOrErrors.Success("All Ok");
            var foreignKeyDict = toBeCheckSqlData.ForeignKeys.ToDictionary(x => x.ToString());
            //now we check the foreign keys
            foreach (var foreignKey in refSqlData.ForeignKeys)
            {
                if (!foreignKeyDict.ContainsKey(foreignKey.ToString()))
                    status.AddSingleError(
                        "Missing Foreign key: The '{0}' SQL database has a foreign key {1}, which is missing in the '{2}' database.",
                        _refDatabaseName, foreignKey.ToString(), _toBeCheckDatabaseName);
                else
                {
                    var foreignKey2 = foreignKeyDict[foreignKey.ToString()];
                    foreignKeyDict.Remove(foreignKey.ToString());
                    if (foreignKey.DeleteAction != foreignKey2.DeleteAction)
                        status.AddSingleError(
                            "Foreign Key Delete Action: The [{0}] database has a foreign key {1} that has delete action of {2}, while database [{3}] has delete action of {4}.",
                            _refDatabaseName, foreignKey.ToString(), foreignKey.DeleteAction,
                            _toBeCheckDatabaseName, foreignKey2.DeleteAction);
                }
            }
            if (foreignKeyDict.Any())
            {
                foreach (var missingFKey in foreignKeyDict.Values)
                {
                    status.AddWarning("The '{0}' database has a foreign key {1}, which the '{2}' database did not have.",
                        _toBeCheckDatabaseName, missingFKey.ToString(), _refDatabaseName);
                }
            }

            return status;
        }

        private ISuccessOrErrors CheckSqlColumn(SqlColumnInfo sqlCol, SqlColumnInfo colToCheck, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (sqlCol.SqlTypeName != colToCheck.SqlTypeName)
                status.AddSingleError(
                    "Column Type: SQL column {0}.{1} type does not match EF. '{2}' db type = {3}, '{4}' db type = {5}.",
                    combinedName, sqlCol.ColumnName, 
                    _refDatabaseName, sqlCol.SqlTypeName, 
                    _toBeCheckDatabaseName, colToCheck.SqlTypeName);
            
            if (sqlCol.IsNullable != colToCheck.IsNullable)
                status.AddSingleError(
                    "Column Nullable: SQL column {0}.{1} nullablity does not match. '{2}' db is {3}NULL, '{4}' db is {5}NULL.",
                    combinedName, sqlCol.ColumnName,
                    _refDatabaseName, sqlCol.IsNullable ? "" : "NOT ",
                    _toBeCheckDatabaseName, colToCheck.IsNullable ? "" : "NOT ");
            
            if (sqlCol.MaxLength != colToCheck.MaxLength)
                status.AddSingleError(
                    "Column MaxLength: SQL column {0}.{1} MaxLength does not match. '{2}' db MaxLength = {2}, '{4}' db MaxLength = {5}.",
                    combinedName, sqlCol.ColumnName,
                    _refDatabaseName, sqlCol.MaxLength,
                    _toBeCheckDatabaseName, colToCheck.MaxLength);

            if (sqlCol.IsPrimaryKey != colToCheck.IsPrimaryKey)
                status.AddSingleError(
                    "Primary Key: The SQL column {0}.{1} primary key settings don't match. '{2}' db says is {3}a key, '{4}' db says it is {5}a key.",
                    combinedName, sqlCol.ColumnName,
                    _refDatabaseName, sqlCol.IsPrimaryKey ? "" : "NOT ",
                    _toBeCheckDatabaseName, colToCheck.IsPrimaryKey ? "" : "NOT ");
            else if (sqlCol.IsPrimaryKey && sqlCol.PrimaryKeyOrder != colToCheck.PrimaryKeyOrder)
                status.AddSingleError(
                    "Key Order: The SQL column {0}.{1} primary key order does not match. '{2}' db order = {3}, '{4}' db order = {5}.",
                    combinedName, sqlCol.ColumnName,
                    _refDatabaseName, sqlCol.PrimaryKeyOrder,
                    _toBeCheckDatabaseName, colToCheck.PrimaryKeyOrder);

            return status;
        }
    }
}