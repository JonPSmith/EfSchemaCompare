#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlCompare.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Linq;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareCore
{
    public class SqlCompare
    {
        private readonly string _refDatabaseName;
        private readonly string _toBeCheckDatabaseName;

        public SqlCompare(string refDatabaseName, string toBeCheckDatabaseName)
        {
            _refDatabaseName = refDatabaseName;
            _toBeCheckDatabaseName = toBeCheckDatabaseName;
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
                    status.AddSingleError(
                        "Missing Table: The '{0}' SQL database has a table called {1}, which is missing in the '{2}' database.",
                        _refDatabaseName, sqlTable.CombinedName, _toBeCheckDatabaseName);
                else
                {
                    //has table, so compare the columns/properties
                    var sqlTable2Info = sqlTable2Dict[sqlTable.CombinedName];
                    sqlTable2Dict.Remove(sqlTable.CombinedName);

                    //we create a dict for columns in SECOND db, which we check. As we find columns we remove them
                    var sqlColsDict = sqlTable2Info.ColumnInfo.ToDictionary(x => x.ColumnName);

                    foreach (var col in sqlTable.ColumnInfo)
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
                                 _toBeCheckDatabaseName, sqlTable.CombinedName, missingCol.ColumnName, missingCol.ColumnSqlType, _refDatabaseName);
                        }
                    }
                }
            }

            //Now check the foreign keys
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
                            "Foreign Key Delete Action: The {{0}] database has a foreign key {1} that has delete action of {2}. Second database was '{3}'.",
                            _refDatabaseName, foreignKey.ToString(), foreignKey.DeleteAction, foreignKey2.DeleteAction, _toBeCheckDatabaseName);
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

            //now see what SQL tables haven't been mentioned
            if (sqlTable2Dict.Any())
            {
                foreach (var unusedTable in sqlTable2Dict.Values)
                {
                    status.AddWarning("SQL database '{0}', table {1} table contained an extra table, {1}", _refDatabaseName, unusedTable.CombinedName);
                }
            }
            
            return status;
        }

        //-------------------------------------------------------------------------------
        //private helpers

        private ISuccessOrErrors CheckSqlColumn(SqlColumnInfo sqlCol, SqlColumnInfo colToCheck, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (sqlCol.ColumnSqlType != colToCheck.ColumnSqlType)
                status.AddSingleError(
                    "Column Type: SQL column {0}.{1} type does not match EF. '{2}' db type = {3}, '{4}' db type = {5}.",
                    combinedName, sqlCol.ColumnName, 
                    _refDatabaseName, sqlCol.ColumnSqlType, 
                    _toBeCheckDatabaseName, colToCheck.ColumnSqlType);
            
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