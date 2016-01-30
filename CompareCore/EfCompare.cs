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
        private readonly Dictionary<string, SqlTableInfo> _sqlInfoDict;

        public EfCompare(string sqlDbRefString, Dictionary<string, SqlTableInfo> sqlInfoDict)
        {
            _sqlDbRefString = sqlDbRefString;
            _sqlInfoDict = sqlInfoDict;
        }

        public ISuccessOrErrors CompareEfWithSql(IList<EfTableInfo> efInfos, SqlAllInfo allSqlInfo)
        {

            var status = SuccessOrErrors.Success("All Ok");

            //first we compare the ef table columns with the SQL table columns
            foreach (var efInfo in efInfos)
            {
                if (!_sqlInfoDict.ContainsKey(efInfo.CombinedName))
                    status.AddSingleError(
                        "Missing Table: The SQL {0} does not contain a table called {1}. Needed by EF class {2}.",
                        _sqlDbRefString, efInfo.CombinedName, efInfo.ClrClassType.Name);
                else
                {
                    //has table, so compare the columns/properties
                    var sqlTableInfo = _sqlInfoDict[efInfo.CombinedName];
                    _sqlInfoDict.Remove(efInfo.CombinedName);

                    //we create a dict, which we check. As we find columns we remove them
                    var sqlColsDict = sqlTableInfo.ColumnInfos.ToDictionary(x => x.ColumnName);

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
                                _sqlDbRefString, efInfo.CombinedName, missingCol.ColumnName, missingCol.SqlTypeName.SqlToClrType(missingCol.IsNullable));
                        }
                    }
                }
            }

            //now we compare the EF relationships with the SQL foreign keys
            //we do this here because we now have the tables that wren't mentioned in EF,
            //which are the tables that EF will automatically add to handle many-many relationships.
            var relChecker = new EfRelationshipChecker(efInfos, allSqlInfo, _sqlInfoDict.Values.ToList());
            foreach (var efInfo in efInfos)
            {      
                //now we check the relationships
                foreach (var relationCol in efInfo.RelationshipCols)
                {
                    var relStatus = relChecker.CheckEfRelationshipToSql(efInfo, relationCol);
                    status.Combine(relStatus);
                    if (relStatus.IsValid && relStatus.Result != null)
                        //It has found a many-to-many table which we need to remove so that it does not show a warning at the end
                        _sqlInfoDict.Remove(relStatus.Result);
                }
            }

            return status;
        }

        private ISuccessOrErrors CheckColumn(SqlColumnInfo sqlCol, EfColumnInfo clrCol, string combinedName)
        {
            var status = new SuccessOrErrors();
            if (sqlCol.SqlTypeName != clrCol.SqlTypeName)
                status.AddSingleError(
                    "Column Type: The SQL {0} column {1}.{2} type does not match EF. SQL type = {3}, EF type = {4}.",
                     _sqlDbRefString, combinedName, clrCol.SqlColumnName, sqlCol.SqlTypeName, clrCol.SqlTypeName);

            if (sqlCol.IsNullable != clrCol.IsNullable)
                status.AddSingleError(
                    "Column Nullable: SQL {0} column {1}.{2} nullablity does not match. SQL is {3}NULL, EF is {4}NULL.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName,
                    sqlCol.IsNullable ? "" : "NOT ",
                    clrCol.IsNullable ? "" : "NOT ");

            if (sqlCol.IsPrimaryKey != clrCol.IsPrimaryKey)
                status.AddSingleError(
                    "Primary Key: The SQL {0}  column {1}.{2} primary key settings don't match. SQL says it is {3}a key, EF says it is {4}a key.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName,
                    sqlCol.IsPrimaryKey ? "" : "NOT ",
                    clrCol.IsPrimaryKey ? "" : "NOT ");
            else if (sqlCol.IsPrimaryKey && sqlCol.PrimaryKeyOrder != clrCol.PrimaryKeyOrder)
                status.AddSingleError(
                    "Primary Key Order: The SQL {0}  column {1}.{2} primary key order does not match. SQL order = {3}, EF order = {4}.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName,
                    sqlCol.PrimaryKeyOrder, clrCol.PrimaryKeyOrder);

            return status.Combine(CheckMaxLength(sqlCol, clrCol, combinedName));
        }

        private ISuccessOrErrors CheckMaxLength(SqlColumnInfo sqlCol, EfColumnInfo clrCol, string combinedName)
        {
            var status = SuccessOrErrors.Success("All ok");

            if (sqlCol.MaxLength == -1 && clrCol.MaxLength != -1)
            {
                //SQL is at max length, but EF isn't
                return status.AddSingleError(
                    "MaxLength: The SQL {0} column {1}.{2}, type {3}, is at max length, but EF length is at {4}.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName, clrCol.ClrColumnType, clrCol.MaxLength);
            }

            if (sqlCol.MaxLength != clrCol.MaxLength)
            {
                return status.AddSingleError(
                    "MaxLength: The  SQL {0}  column {1}.{2}, type {3}, length does not match EF. SQL length = {4}, EF length = {5}.",
                    _sqlDbRefString, combinedName, clrCol.SqlColumnName, clrCol.ClrColumnType,
                    sqlCol.MaxLength, 
                    sqlCol.SqlTypeName.EfLengthIdHalfThis() ? clrCol.MaxLength / 2 : clrCol.MaxLength);
            }

            return status.SetSuccessMessage("All Ok");
        }
    }


}