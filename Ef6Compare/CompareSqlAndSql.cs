#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareSqlAndSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Configuration;
using System.Linq;
using CompareCore.SqlInfo;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareEfSql
{
    public static class CompareSqlAndSql
    {
        /// <summary>
        /// This compares two SQL databases looking at each table, its columns, its keys and its foreign keys
        /// </summary>
        /// <param name="refDbConnectionOrConfig">Either a full connection string or a reference to connection string in Config file</param>
        /// <param name="toBeCheckDbConnectionOrConfig">Either a full connection string or a reference to connection string in Config file</param>
        /// <returns></returns>
        public static ISuccessOrErrors CompareSqlToSql(this string refDbConnectionOrConfig, string toBeCheckDbConnectionOrConfig)
        {
            var status = SuccessOrErrors.Success("All Ok");

            var refDbConnection = GetConfigurationOrActualString(refDbConnectionOrConfig);
            var toBeCheckDbConnection = GetConfigurationOrActualString(toBeCheckDbConnectionOrConfig);

            var sqlInfo1 = SqlTableInfo.GetAllSqlTablesWithColInfo(refDbConnection);
            var sqlInfo2 = SqlTableInfo.GetAllSqlTablesWithColInfo(toBeCheckDbConnection);

            var sqlTable2Dict = sqlInfo2.ToDictionary(x => x.CombinedName);

            foreach (var sqlTable in sqlInfo1)
            {
                if (!sqlTable2Dict.ContainsKey(sqlTable.CombinedName))
                    status.AddSingleError(
                        "Missing Table: The first SQL database has a table called {0}, which is missing in the second database.",
                        sqlTable.CombinedName);
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
                                "Missing Column: The SQL table {0} in second database does not contain a column called {1}",
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
                            status.AddWarning("The second database SQL table {0} has a column called {1} (type {2}), which the first db did not have.",
                                sqlTable.CombinedName, missingCol.ColumnName, missingCol.ColumnSqlType);
                        }
                    }

                    var foreignKeyDict =
                        sqlTable2Info.ForeignKeys.ToDictionary(x => x.ToString());
                    //now we check the foreign keys
                    foreach (var foreignKey in sqlTable.ForeignKeys)
                    {

                        if (!foreignKeyDict.ContainsKey(foreignKey.ToString()))
                            status.AddSingleError(
                                "Missing Foreign key: The first SQL database has a foreign key {0}, which is missing in the second database.",
                                foreignKey.ToString());
                        else
                        {
                            var foreignKey2 = foreignKeyDict[foreignKey.ToString()];
                            foreignKeyDict.Remove(foreignKey.ToString());
                            if (foreignKey.DeleteAction != foreignKey2.DeleteAction)
                                status.AddSingleError(
                                    "Foreign Key Delete Action: The first database has a foreign key {0} that has delete action of {0}. Second database was {1} ",
                                    foreignKey.ToString(), foreignKey.DeleteAction, foreignKey2.DeleteAction);
                        }
                    }
                    if (foreignKeyDict.Any())
                    {
                        foreach (var missingFKey in foreignKeyDict.Values)
                        {
                            status.AddWarning("The second database SQL table {0} has a foreign key {1}, which the first db did not have.",
                                sqlTable.CombinedName, missingFKey.ToString());
                        }
                    }
                }
            }

            //now see what SQL tables haven't been mentioned
            if (sqlTable2Dict.Any())
            {

                foreach (var unusedTable in sqlTable2Dict.Values)
                {
                    status.AddWarning("The second SQL table contained an extra table, {0}", unusedTable.CombinedName);
                }
            }
            
            return status;
        }



        //-------------------------------------------------------------------------------
        //private helpers


        private static string GetConfigurationOrActualString(string refDbConnection)
        {
            var connectionFromConfigFile = ConfigurationManager.ConnectionStrings[refDbConnection];
            return connectionFromConfigFile == null ? refDbConnection : connectionFromConfigFile.ConnectionString;
        }

        private static ISuccessOrErrors CheckSqlColumn(SqlColumnInfo sqlCol, SqlColumnInfo colToCheck, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (sqlCol.ColumnSqlType != colToCheck.ColumnSqlType)
                status.AddSingleError(
                    "Column Type: The first database had a SQL column {0}.{1} type does not match EF. First db type = {2}, second db type = {3}",
                    combinedName, sqlCol.ColumnName, sqlCol.ColumnSqlType, colToCheck.ColumnSqlType);
            
            if (sqlCol.IsNullable != colToCheck.IsNullable)
                status.AddSingleError(
                    "Column Nullable: The first database had a SQL column {0}.{1} nullable does not match EF. First db is {2}NULL, second db is {3}NULL",
                    combinedName, sqlCol.ColumnName,
                    sqlCol.IsNullable ? "" : "NOT ",
                    colToCheck.IsNullable ? "" : "NOT ");
            
            if (sqlCol.MaxLength != colToCheck.MaxLength)
                status.AddSingleError(
                    "Column MaxLength: The first database had a SQL column {0}.{1} type does not match EF. First db type = {2}, second db type = {3}",
                    combinedName, sqlCol.ColumnName, sqlCol.ColumnSqlType, colToCheck.ColumnSqlType);

            if (sqlCol.IsPrimaryKey != colToCheck.IsPrimaryKey)
                status.AddSingleError(
                    "Primary Key: The SQL column {0}.{1} primary key settings don't match.First db says is {2}a key, second db says it is {3}a key",
                    combinedName, sqlCol.ColumnName,
                    sqlCol.IsPrimaryKey ? "" : "NOT ",
                    colToCheck.IsPrimaryKey ? "" : "NOT ");
            else if (sqlCol.IsPrimaryKey && sqlCol.PrimaryKeyOrder != colToCheck.PrimaryKeyOrder)
                status.AddSingleError(
                    "Key Order: The SQL column {0}.{1} primary key order does not match. SQL order = {2}, EF order = {3}",
                    combinedName, sqlCol.ColumnName,
                    sqlCol.PrimaryKeyOrder, colToCheck.PrimaryKeyOrder);

            return status;
        }
    }
}