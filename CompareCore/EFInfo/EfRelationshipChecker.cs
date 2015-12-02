#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfRelationshipChecker.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareCore.EFInfo
{
    public class EfRelationshipChecker
    {
        private readonly Dictionary<Type, EfTableInfo> _efInfosDict;
        private readonly Dictionary<string, SqlTableInfo> _sqlInfoDict;
        private readonly SqlAllInfo _allSqlInfo;
        private readonly Dictionary<string, SqlTableInfo> _potentialManyToManyTablesDict;
        private readonly List<Tuple<string, List<string>>> _foreignKeysGroupByParentTableName;

        public EfRelationshipChecker(IEnumerable<EfTableInfo> efInfos, 
            SqlAllInfo allSqlInfo,
            ICollection<SqlTableInfo> potentialManyToManyTables)
        {
            _efInfosDict = efInfos.ToDictionary(x => x.ClrClassType);
            _allSqlInfo = allSqlInfo;
            _sqlInfoDict = allSqlInfo.TableInfos.ToDictionary(x => x.CombinedName);
            _potentialManyToManyTablesDict = potentialManyToManyTables.ToDictionary(x => x.CombinedName);
            //This dictionary allows us to backtrack the foreign keys to the correct many-to-many table
            _foreignKeysGroupByParentTableName = allSqlInfo.ForeignKeys
                .GroupBy(key => key.ParentTableNameWithScheme)
                .Select(x => new Tuple<string, List<string>>(x.Key, 
                    x.Select(y => FormatHelpers.CombineTableAndColumnNames(y.ReferencedTableName, y.ReferencedColName)).OrderBy(y => y).ToList())).ToList();
        }

        /// <summary>
        /// This checks that the EF relationship is mirrored in the SQL
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="relEfCol"></param>
        /// <returns>returns status with optional name of SQL many-many table. Used to mark that table as having been used</returns>
        public ISuccessOrErrors<string> CheckEfRelationshipToSql(EfTableInfo tableInfo, EfRelationshipInfo relEfCol)
        {
            var status = new SuccessOrErrors<string>();
            string manyToManyTableName = null;
            if (relEfCol.FromToRelationships.FromMultiplicity == EfRelationshipTypes.Many)
            {
                //handle from many

                if (relEfCol.FromToRelationships.ToMultiplicity == EfRelationshipTypes.Many)
                {
                    //many to many - look for a linking table in the list of _potentialManyToManyTablesDict that has the right foreignKeys

                    var fromEfTable = GetEfTableDataFromClass(tableInfo.ClrClassType);
                    var toEfTable = GetEfTableDataFromCollection(relEfCol);
                    var fromKeys = fromEfTable.NormalCols.Where(x => x.IsPrimaryKey);
                    var toKeys = toEfTable.NormalCols.Where(x => x.IsPrimaryKey);

                    var linkCombinedNames =
                        AllManyToManyTablesThatHaveTheRightForeignKeys(fromKeys, toKeys, fromEfTable.TableName, toEfTable.TableName).ToList();

                    if (!linkCombinedNames.Any())
                        status.AddSingleError(
                            "Missing Link Table: EF has a {0} relationship between {1}.{2} and {3} but we could not find a linking table with the right foreign keys.",
                            relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, toEfTable.TableName);
                    else
                    {
                        //now we check the entries in the linking tab
                        if (linkCombinedNames.Count > 1)
                            status.AddWarning(
                                "Ambigous Link Table: EF has a {0} relationship between {1}.{2} and {3}. This was ambigous so we may not have fully checked this.",
                                relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName,
                                toEfTable.TableName);

                        manyToManyTableName = linkCombinedNames.First();
                        //todo: check cascade deletes
                        foreach (var foreignKey in _allSqlInfo.ForeignKeys.Where(x => x.ParentTableNameWithScheme == manyToManyTableName)
                            .Where(foreignKey => !foreignKey.IsCascade))
                        {
                            status.AddSingleError(
                                "Cascade Delete: The {0} relationship between {1}.{2} and {3} has a foreign key {4} that is not CASCASE DELETE." +
                                "  All linking table foreign keys should have CASCASE DELETE.",
                                relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName,
                                toEfTable.TableName, foreignKey.ConstraintName);
                        }

                    }
                }
                else
                {
                    //many to one 
                    var toSqlTableStatus = GetSqlTableDataFromClass(relEfCol.ClrColumnType);
                    if (toSqlTableStatus.HasErrors)
                        return status.Combine(toSqlTableStatus);

                    var foreignKeys = GetForeignKeys(tableInfo.TableName, toSqlTableStatus.Result);
                    if (!foreignKeys.Any())
                        status.AddSingleError(
                            "Missing Foreign Key: EF has a {0} relationship between {1}.{2} and {3} but we don't find that in SQL",
                            relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTableStatus.Result.TableName);
                    else
                    {
                        //Look at cascase deletes
                        foreach (var foreignKey in foreignKeys.Where(foreignKey => relEfCol.FromToRelationships.ToIsCascadeDelete != foreignKey.IsCascade))
                        {
                            status.AddSingleError(
                                "Cascade Delete: The {0} relationship between {1}.{2} and {3} has different cascase delete value." +
                                " SQL foreign key say {4}, EF setting is {5}",
                                relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTableStatus.Result.TableName,
                                foreignKey.DeleteAction,
                                relEfCol.FromToRelationships.ToIsCascadeDelete ? "CASCADE" : "NO_ACTION");
                        }
                    }
                }
            }
            else
            {
                //The parent is single, which implies that the child holds the key

                if (relEfCol.FromToRelationships.ToMultiplicity == EfRelationshipTypes.Many)
                {
                    //typical one to many

                    var fromSqlTableStatus = GetSqlTableDataFromCollection(relEfCol);
                    if (fromSqlTableStatus.HasErrors)
                        return status.Combine( fromSqlTableStatus);
                    var toSqlTable = _sqlInfoDict[tableInfo.CombinedName];
                    var foreignKeys = GetForeignKeys(fromSqlTableStatus.Result.TableName, toSqlTable);

                    if (!foreignKeys.Any())
                        status.AddSingleError(
                            "Missing Foreign Key: EF has a {0} relationship between {1} and {2}.{3} but we don't find that in SQL",
                            relEfCol.FromToRelationships, fromSqlTableStatus.Result.TableName, tableInfo.TableName,
                            relEfCol.ClrColumnName);
                    else
                    {
                        //Look at cascase deletes
                        foreach (var foreignKey in foreignKeys.Where(foreignKey => relEfCol.FromToRelationships.FromIsCascadeDelete != foreignKey.IsCascade))
                            status.AddSingleError(
                            "Cascade Delete: The {0} relationship between {1}.{2} and {3} has different cascase delete value." +
                            " SQL foreign key say {4}, EF setting is {5}",
                            relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTable.TableName,
                            foreignKey.DeleteAction,
                            relEfCol.FromToRelationships.ToIsCascadeDelete ? "CASCADE" : "NO_ACTION");
                    }
                }
                else
                {
                    //one to one/OneOrZero or reverse
                    var sqlTableEnd1Status = GetSqlTableDataFromClass(relEfCol.ClrColumnType);
                    if (sqlTableEnd1Status.HasErrors)
                        return status.Combine(sqlTableEnd1Status);

                    var sqlTableEnd2 = _sqlInfoDict[tableInfo.CombinedName];

                    //There is one foreign key for both directions. Therefore we need to check both 

                    var foreignKey1 = GetForeignKeys(sqlTableEnd1Status.Result.TableName, sqlTableEnd2).SingleOrDefault();
                    var foreignKey2 = GetForeignKeys(sqlTableEnd2.TableName, sqlTableEnd1Status.Result).SingleOrDefault();
                   
                    if (foreignKey1 == null && foreignKey2 == null)
                        status.AddSingleError(
                            "Missing Foreign Key: EF has a {0} relationship between {1}.{2} and {3} but we don't find that in SQL",
                            relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, sqlTableEnd1Status.Result.TableName);
                    else if (foreignKey1 != null && relEfCol.FromToRelationships.FromIsCascadeDelete != foreignKey1.IsCascade)
                    {
                        //Look at cascase deletes
                        status.AddSingleError(
                            "Cascade Delete: The {0} relationship between {1}.{2} and {3} has different cascase delete value."+
                            " SQL foreign key say {4}, EF setting is {5}",
                            relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, sqlTableEnd1Status.Result.TableName,
                            foreignKey1.DeleteAction ,
                            relEfCol.FromToRelationships.ToIsCascadeDelete ? "CASCADE" : "NO_ACTION");
                    }
                    else if (foreignKey2 != null && relEfCol.FromToRelationships.ToIsCascadeDelete != foreignKey2.IsCascade)
                    {
                        //Look at cascase deletes
                        status.AddSingleError(
                            "Cascade Delete: The {0} relationship between {1}.{2} and {3} has different cascase delete value." +
                            " SQL foreign key say {4}, EF setting is {5}",
                            relEfCol.FromToRelationships, tableInfo.TableName, relEfCol.ClrColumnName, sqlTableEnd1Status.Result.TableName,
                            foreignKey1.DeleteAction,
                            relEfCol.FromToRelationships.ToIsCascadeDelete ? "CASCADE" : "NO_ACTION");
                    }
                }
            }

            return status.HasErrors ? status : status.SetSuccessWithResult(manyToManyTableName, "All Ok");
        }

        //------------------------------------------------------------------------------------------
        //private helpers

        /// <summary>
        /// This returns all the foreign keys linking the parent table to the primary keys on the referenced table
        /// </summary>
        /// <param name="parentTableName"></param>
        /// <param name="referencedTableAndKey"></param>
        /// <returns></returns>
        private ICollection<SqlForeignKey> GetForeignKeys(string parentTableName, SqlTableInfo referencedTableAndKey)
        {
            return _allSqlInfo.ForeignKeys.Where(x => x.ParentTableName == parentTableName
                                                      && x.ReferencedTableName == referencedTableAndKey.TableName
                                                      &&
                                                      referencedTableAndKey.ColumnInfo.Where(y => y.IsPrimaryKey)
                                                          .Select(y => y.ColumnName)
                                                          .Contains(x.ReferencedColName)).ToList();
        }

        /// <summary>
        /// This returns the names of tables that have a set of foreign keys which map to all of the primary keys of the from/to tables
        /// </summary>
        /// <param name="fromKeys"></param>
        /// <param name="toKeys"></param>
        /// <param name="fromTableName"></param>
        /// <param name="toTableName"></param>
        /// <returns></returns>
        private IEnumerable<string> AllManyToManyTablesThatHaveTheRightForeignKeys(
            IEnumerable<EfColumnInfo> fromKeys, IEnumerable<EfColumnInfo> toKeys, string fromTableName, string toTableName)
        {
            //we form all the keys that we expect to see in the ReferencedTable/Col part of the many-to-many table
            var allKeys = fromKeys.Select(x => FormatHelpers.CombineTableAndColumnNames(fromTableName, x.SqlColumnName)).ToList();
            allKeys.AddRange(toKeys
                    .Select(x => FormatHelpers.CombineTableAndColumnNames(toTableName, x.SqlColumnName)));

            return _foreignKeysGroupByParentTableName.Where(x => x.Item2.SequenceEqual(allKeys.OrderBy(y => y))).Select(x => x.Item1);
        }

        private ISuccessOrErrors<SqlTableInfo> GetSqlTableDataFromCollection(EfRelationshipInfo relEfCol)
        {
            if (!relEfCol.ClrColumnType.IsGenericType)
                throw new InvalidOperationException("I expected a generic list etc. here");
            var genArgs = relEfCol.ClrColumnType.GetGenericArguments();

            return GetSqlTableDataFromClass(genArgs[0]);
        }

        private ISuccessOrErrors<SqlTableInfo> GetSqlTableDataFromClass(Type efClassType)
        {
            ISuccessOrErrors<SqlTableInfo> status = new SuccessOrErrors<SqlTableInfo>();
            var combinedName = GetEfTableDataFromClass(efClassType).CombinedName;
            SqlTableInfo sqlTable;
            if (!_sqlInfoDict.TryGetValue(combinedName, out sqlTable))
                return status.AddSingleError("Missing SQL Table: Could not find the SQL table called {0}.", combinedName);
            return status.SetSuccessWithResult(sqlTable, "All OK");
        }

        private EfTableInfo GetEfTableDataFromCollection(EfRelationshipInfo relEfCol)
        {
            if (!relEfCol.ClrColumnType.IsGenericType)
                throw new InvalidOperationException("I expected a generic list etc. here");
            var genArgs = relEfCol.ClrColumnType.GetGenericArguments();

            return GetEfTableDataFromClass(genArgs[0]);
        }

        private EfTableInfo GetEfTableDataFromClass(Type efClassType)
        {
            EfTableInfo efData;
            if (!_efInfosDict.TryGetValue(efClassType, out efData))
                throw new InvalidOperationException("I could not find a EF class of the type " + efClassType.Name);
            return efData;
        }
    }
}