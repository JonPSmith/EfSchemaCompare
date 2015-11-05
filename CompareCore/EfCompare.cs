#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareEfSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CompareCore.EFInfo;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareCore
{
    public class EfCompare
    {
        private readonly string _sqlDbRefString;
        private readonly string _sqlTableNamesToIgnore;

        public EfCompare(string sqlDbRefString, string sqlTableNamesToIgnore)
        {
            _sqlDbRefString = sqlDbRefString;
            _sqlTableNamesToIgnore = sqlTableNamesToIgnore;
        }

        public ISuccessOrErrors CompareEfWithSql(ICollection<EfTableInfo> efInfos, SqlAllInfo allSqlInfo)
        {

            var status = SuccessOrErrors.Success("All Ok");
            var sqlInfoDict = allSqlInfo.TableInfos.ToDictionary(x => x.CombinedName);

            //first we compare the ef table columns with the SQL table columns
            foreach (var efInfo in efInfos)
            {
                if (!sqlInfoDict.ContainsKey(efInfo.CombinedName))
                    status.AddSingleError(
                        "Missing Table: The SQL {0} does not contain a table called {1}. Needed by EF class {2}.",
                        _sqlDbRefString, efInfo.CombinedName, efInfo.ClrClassType.Name);
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
                                "Missing Column: The SQL {0} table {1} does not contain a column called {2}. Needed by EF class {3}.",
                                _sqlDbRefString, efInfo.CombinedName, clrCol.SqlColumnName, efInfo.ClrClassType.Name);
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
                            status.AddWarning("SQL {0} table {1} has a column called {2} (.NET type {3}) that EF does not access.",
                                _sqlDbRefString, efInfo.CombinedName, missingCol.ColumnName, missingCol.ColumnSqlType.SqlToClrType(missingCol.IsNullable));
                        }
                    }
                }
            }

            //now we compare the EF relationships with the SQL foreign keys
            //we do this here because we now have the tables that wren't mentioned in EF,
            //which are the tables that EF will automatically add to handle many-many relationships.
            var relChecker = new EfRelationshipChecker(efInfos, allSqlInfo, sqlInfoDict.Values);
            foreach (var efInfo in efInfos)
            {      
                //now we check the relationships
                foreach (var relationCol in efInfo.RelationshipCols)
                {
                    var relStatus = relChecker.CheckEfRelationshipToSql(efInfo, relationCol);
                    status.Combine(relStatus);
                    if (relStatus.IsValid && relStatus.Result != null)
                        //It has found a many-to-many table which we need to remove so that it does not show a warning at the end
                        sqlInfoDict.Remove(relStatus.Result);
                }
            }

            //now see what SQL tables haven't been mentioned
            if (sqlInfoDict.Any())
            {
                var tablesToIgnore = _sqlTableNamesToIgnore.Split(',').Select(x => x.Trim()).ToList();
                foreach (var unusedTable in sqlInfoDict.Values.Where(x => !tablesToIgnore.Contains(x.TableName)))
                {
                    status.AddWarning("SQL {0} table {1} was not used by EF", _sqlDbRefString, unusedTable.CombinedName);
                }
            }

            return status;
        }

        private ISuccessOrErrors CheckColumn(SqlColumnInfo sqlCol, EfColumnInfo clrCol, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (SqlAndEfTypesDontMatch(sqlCol, clrCol))
                status.AddSingleError(
                    "Column Type: The SQL {0} column {1}.{2} type does not match EF. SQL type = {3}, EF type = {4}.",
                     _sqlDbRefString, combinedName, clrCol.SqlColumnName, sqlCol.ColumnSqlType.SqlToClrType(sqlCol.IsNullable),
                    clrCol.ClrColumnType);

            if (sqlCol.IsPrimaryKey != clrCol.IsPrimaryKey)
                status.AddSingleError(
                    "The SQL {0}  column {1}.{2} primary key settings don't match. SQL says it is {3}a key, EF says it is {4}a key.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName,
                    sqlCol.IsPrimaryKey ? "" : "NOT ",
                    clrCol.IsPrimaryKey ? "" : "NOT ");
            else if (sqlCol.IsPrimaryKey && sqlCol.PrimaryKeyOrder != clrCol.PrimaryKeyOrder)
                status.AddSingleError(
                    "The SQL {0}  column {1}.{2} primary key order does not match. SQL order = {3}, EF order = {4}.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName,
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

        private ISuccessOrErrors CheckMaxLength(SqlColumnInfo sqlCol, EfColumnInfo clrCol, string combinedName)
        {
            var status = SuccessOrErrors.Success("All ok");

            //if clrCol.MaxLength == -2 if we should not check the max length because it is a base class and EF does not hold the MaxLength
            if (clrCol.MaxLength == -2) return status;

            if (sqlCol.MaxLength == -1 && clrCol.MaxLength != -1)
            {
                //SQL is at max length, but EF isn't
                return status.AddSingleError(
                    "The  SQL {0}   column {1}.{2}, type {3}, is at max length, but EF length is at {4}.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName, clrCol.ClrColumnType, clrCol.MaxLength);
            }

            var sqlModifiedMaxLength = sqlCol.ColumnSqlType.GetClrMaxLength(sqlCol.MaxLength);
            if (sqlModifiedMaxLength != clrCol.MaxLength)
            {
                return status.AddSingleError(
                    "The  SQL {0}  column {1}.{2}, type {3}, length does not match EF. SQL length = {4}, EF length = {5}",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName, clrCol.ClrColumnType,
                    sqlCol.ColumnSqlType.GetClrMaxLength(sqlCol.MaxLength), clrCol.MaxLength);
            }

            return status.SetSuccessMessage("All Ok");
        }
    }


}