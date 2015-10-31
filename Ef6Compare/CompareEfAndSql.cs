#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareEfAndSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Data.Entity;
using System.Linq;
using CompareCore.EFInfo;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using Ef6Compare.Internal;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareEfSql
{
    public static class CompareEfAndSql
    {
        public static ISuccessOrErrors CompareEfWithDb(this DbContext db, string sqlTableNamesToIgnore = "__MigrationHistory")
        {
            var status = SuccessOrErrors.Success("All Ok");

            var efInfos = EfTableInfo.GetAllEfTablesWithColInfo(db);
            var sqlInfo = SqlTableInfo.GetAllSqlTablesWithColInfo(db.Database.Connection.ConnectionString);
            var relChecker = new EfRelationshipChecker(efInfos, sqlInfo);

            var sqlInfoDict = sqlInfo.ToDictionary(x => x.CombinedName);

            //first we compare the ef table columns with the SQL table
            foreach (var efInfo in efInfos)
            {

                if (!sqlInfoDict.ContainsKey(efInfo.CombinedName))
                    status.AddSingleError(
                        "The SQL database does not contain a table called {0}. Needed by EF class {1}",
                        efInfo.CombinedName, efInfo.ClrClassType.Name);
                else
                {
                    //has table, so compare the columns/properties
                    var sqlTableInfo = sqlInfoDict[efInfo.CombinedName];
                    sqlInfoDict.Remove(efInfo.CombinedName);

                    //we create a dict, which we check. As we find columns we remove them
                    var sqlColsDict = sqlTableInfo.ColumnInfo.ToDictionary(x => x.ColumnName);

                    foreach (var clrCol in efInfo.NormalCols)
                    {
                        if (!sqlColsDict.ContainsKey(clrCol.SqlColumnName))
                            status.AddSingleError(
                                "The SQL table {0} does not contain a column called {1}. Needed by EF class {2}",
                                efInfo.CombinedName, clrCol.SqlColumnName, efInfo.ClrClassType.Name);
                        else
                        {
                            //check the columns match
                            var sqlCol = sqlColsDict[clrCol.SqlColumnName];
                            sqlColsDict.Remove(clrCol.SqlColumnName);            //remove it as it has been used

                            status.Combine(CheckColumn(sqlCol, clrCol, efInfo.CombinedName));
                        }
                    }
                    //At the end we check if any sql columns are left
                    if (sqlColsDict.Any())
                    {
                        foreach (var missingCol in sqlColsDict.Values)
                        {
                            status.AddWarning(" SQL table {0} has a column called {1} (.NET type {2}) that EF does not access",
                                efInfo.CombinedName, missingCol.ColumnName, missingCol.ColumnSqlType.SqlToClrType(missingCol.IsNullable));
                        }
                    }

                    //----------------- Had to turn off as really complicated! ------------------------------
                    ////now we check the relationships
                    //foreach (var relationCol in efInfo.RelationshipCols)
                    //{
                    //    var relStatus = relChecker.CheckEfRelationshipToSql(efInfo, relationCol);
                    //    status.Combine(relStatus);
                    //    if (relStatus.IsValid && relStatus.Result != null)
                    //        //It has found a many-to-many table which we need to remove so that it does not show a warning at the end
                    //        sqlInfoDict.Remove(relStatus.Result);
                    //}
                }
            }

            //now see what SQL tables haven't been mentioned
            if (sqlInfoDict.Any())
            {
                var tablesToIgnore = sqlTableNamesToIgnore.Split(',').Select(x => x.Trim()).ToList();
                foreach (var unusedTable in sqlInfoDict.Values.Where(x => !tablesToIgnore.Contains(x.TableName)))
                {
                    status.AddWarning(" SQL table {0} was not used by EF", unusedTable.CombinedName);
                }
            }

            return status;
        }


        //-------------------------------------------------------------------------------
        //private helpers

        private static ISuccessOrErrors CheckColumn(SqlColumnInfo sqlCol, EfColumnInfo clrCol, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (SqlAndEfTypesDontMatch(sqlCol, clrCol))
                status.AddSingleError(
                    "The SQL column {0}.{1} type does not match EF. SQL type = {2}, EF type = {3}",
                    combinedName, clrCol.SqlColumnName, sqlCol.ColumnSqlType.SqlToClrType(sqlCol.IsNullable),
                    clrCol.ClrColumnType);

            if (sqlCol.IsPrimaryKey != clrCol.IsPrimaryKey)
                status.AddSingleError(
                    "The SQL column {0}.{1} primary key settings don't match. SQL says it is {2}a key, EF says it is {3}a key",
                    combinedName, clrCol.SqlColumnName,
                    sqlCol.IsPrimaryKey ? "" : "NOT ",
                    clrCol.IsPrimaryKey ? "" : "NOT ");
            else if (sqlCol.IsPrimaryKey && sqlCol.PrimaryKeyOrder != clrCol.PrimaryKeyOrder)
                status.AddSingleError(
                    "The SQL column {0}.{1} primary key order does not match. SQL order = {2}, EF order = {3}",
                    combinedName, clrCol.SqlColumnName,
                    sqlCol.PrimaryKeyOrder, clrCol.PrimaryKeyOrder);

            return status.Combine(CheckMaxLength(sqlCol, clrCol, combinedName));
        }

        private static bool SqlAndEfTypesDontMatch(SqlColumnInfo sqlCol, EfColumnInfo clrCol)
        {
            var clrTypeFromSql = sqlCol.ColumnSqlType.SqlToClrType(sqlCol.IsNullable);
            if (clrCol.ClrColumnType.IsEnum &&
                (clrTypeFromSql == typeof (byte) || clrTypeFromSql == typeof (Int16) || clrTypeFromSql == typeof (Int32)))
                //enum is stored as byte, int16 or int32 
                return false;

            return clrTypeFromSql != clrCol.ClrColumnType;

        }

        private static ISuccessOrErrors CheckMaxLength(SqlColumnInfo sqlCol, EfColumnInfo clrCol, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (sqlCol.MaxLength == -1 && clrCol.MaxLength != -1)
            {
                //SQL is at max length, but EF isn't
                return status.AddSingleError(
                    "The SQL column {0}.{1}, type {2}, is at max length, but EF length is at {3}",
                    combinedName, clrCol.SqlColumnName, clrCol.ClrColumnType, clrCol.MaxLength);
            }

            //GetClrMaxLength will return -2 if we should not check things
            var sqlModifiedMaxLength = sqlCol.ColumnSqlType.GetClrMaxLength(sqlCol.MaxLength);
            if (sqlModifiedMaxLength != -2 && sqlModifiedMaxLength != clrCol.MaxLength)
            {
                //
                return status.AddSingleError(
                    "The SQL column {0}.{1}, type {2}, length does not match EF. SQL length = {3}, EF length = {4}",
                    combinedName, clrCol.SqlColumnName, clrCol.ClrColumnType,
                    sqlCol.ColumnSqlType.GetClrMaxLength(sqlCol.MaxLength), clrCol.MaxLength);
            }

            return status.SetSuccessMessage("All Ok");
        }
    }


}