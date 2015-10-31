#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareSqlAndSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Linq;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace Ef6Compare
{
    public class CompareSqlAndSql
    {
        private string _database1Name;
        private string _database2Name;

        /// <summary>
        /// This compares two SQL databases looking at each table, its columns, its keys and its foreign keys
        /// </summary>
        /// <param name="refDbConnectionOrConfig">Either a full connection string or the name of a connection string in Config file</param>
        /// <param name="toBeCheckDbConnectionOrConfig">Either a full connection string or the name of a to connection string in Config file</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareSqlToSql(string refDbConnectionOrConfig, string toBeCheckDbConnectionOrConfig)
        {
            var status = SuccessOrErrors.Success("All Ok");

            var refDbConnection = refDbConnectionOrConfig.GetConfigurationOrActualString();
            var toBeCheckDbConnection = toBeCheckDbConnectionOrConfig.GetConfigurationOrActualString();
            _database1Name = refDbConnection.GetDatabaseNameFromConnectionString();
            _database2Name = toBeCheckDbConnection.GetDatabaseNameFromConnectionString();

            var sqlInfo1 = SqlTableInfo.GetAllSqlTablesWithColInfo(refDbConnection);
            var sqlInfo2 = SqlTableInfo.GetAllSqlTablesWithColInfo(toBeCheckDbConnection);

            var sqlTable2Dict = sqlInfo2.ToDictionary(x => x.CombinedName);

            foreach (var sqlTable in sqlInfo1)
            {
                if (!sqlTable2Dict.ContainsKey(sqlTable.CombinedName))
                    status.AddSingleError(
                        "Missing Table: The '{0}' SQL database has a table called {1}, which is missing in the second database.",
                        _database1Name, sqlTable.CombinedName);
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
                                 _database2Name, sqlTable.CombinedName, missingCol.ColumnName, missingCol.ColumnSqlType, _database1Name);
                        }
                    }

                    var foreignKeyDict =
                        sqlTable2Info.ForeignKeys.ToDictionary(x => x.ToString());
                    //now we check the foreign keys
                    foreach (var foreignKey in sqlTable.ForeignKeys)
                    {

                        if (!foreignKeyDict.ContainsKey(foreignKey.ToString()))
                            status.AddSingleError(
                                "Missing Foreign key: The '{0}' SQL database has a foreign key {1}, which is missing in the '{2}' database.",
                                _database1Name, foreignKey.ToString(), _database2Name);
                        else
                        {
                            var foreignKey2 = foreignKeyDict[foreignKey.ToString()];
                            foreignKeyDict.Remove(foreignKey.ToString());
                            if (foreignKey.DeleteAction != foreignKey2.DeleteAction)
                                status.AddSingleError(
                                    "Foreign Key Delete Action: The {{0}] database has a foreign key {1} that has delete action of {2}. Second database was '{3}'.",
                                    _database1Name, foreignKey.ToString(), foreignKey.DeleteAction, foreignKey2.DeleteAction, _database2Name);
                        }
                    }
                    if (foreignKeyDict.Any())
                    {
                        foreach (var missingFKey in foreignKeyDict.Values)
                        {
                            status.AddWarning("The '{0}' database SQL table {1} has a foreign key {2}, which the '{3}' database did not have.",
                                _database2Name, sqlTable.CombinedName, missingFKey.ToString(), _database1Name);
                        }
                    }
                }
            }

            //now see what SQL tables haven't been mentioned
            if (sqlTable2Dict.Any())
            {
                foreach (var unusedTable in sqlTable2Dict.Values)
                {
                    status.AddWarning("SQL database '{0}', table {1} table contained an extra table, {1}", _database1Name, unusedTable.CombinedName);
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
                    _database1Name, sqlCol.ColumnSqlType, 
                    _database2Name, colToCheck.ColumnSqlType);
            
            if (sqlCol.IsNullable != colToCheck.IsNullable)
                status.AddSingleError(
                    "Column Nullable: SQL column {0}.{1} nullablity does not match. '{2}' db is {3}NULL, '{4}' db is {5}NULL.",
                    combinedName, sqlCol.ColumnName,
                    _database1Name, sqlCol.IsNullable ? "" : "NOT ",
                    _database2Name, colToCheck.IsNullable ? "" : "NOT ");
            
            if (sqlCol.MaxLength != colToCheck.MaxLength)
                status.AddSingleError(
                    "Column MaxLength: SQL column {0}.{1} MaxLength does not match. '{2}' db MaxLength = {2}, '{4}' db MaxLength = {5}.",
                    combinedName, sqlCol.ColumnName,
                    _database1Name, sqlCol.MaxLength,
                    _database2Name, colToCheck.MaxLength);

            if (sqlCol.IsPrimaryKey != colToCheck.IsPrimaryKey)
                status.AddSingleError(
                    "Primary Key: The SQL column {0}.{1} primary key settings don't match. '{2}' db says is {3}a key, '{4}' db says it is {5}a key.",
                    combinedName, sqlCol.ColumnName,
                    _database1Name, sqlCol.IsPrimaryKey ? "" : "NOT ",
                    _database2Name, colToCheck.IsPrimaryKey ? "" : "NOT ");
            else if (sqlCol.IsPrimaryKey && sqlCol.PrimaryKeyOrder != colToCheck.PrimaryKeyOrder)
                status.AddSingleError(
                    "Key Order: The SQL column {0}.{1} primary key order does not match. '{2}' db order = {3}, '{4}' db order = {5}.",
                    combinedName, sqlCol.ColumnName,
                    _database1Name, sqlCol.PrimaryKeyOrder,
                    _database2Name, colToCheck.PrimaryKeyOrder);

            return status;
        }
    }
}