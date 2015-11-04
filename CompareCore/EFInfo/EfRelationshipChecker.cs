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
        private readonly ICollection<SqlTableInfo> _potentialManyToManyTables;
        private readonly List<Tuple<string, List<string>>> _foreignKeysGroupByParentTableName;

        public EfRelationshipChecker(IEnumerable<EfTableInfo> efInfos, 
            SqlAllInfo allSqlInfo,
            ICollection<SqlTableInfo> potentialManyToManyTables)
        {
            _efInfosDict = efInfos.ToDictionary(x => x.ClrClassType);
            _allSqlInfo = allSqlInfo;
            _sqlInfoDict = allSqlInfo.TableInfos.ToDictionary(x => x.CombinedName);
            _potentialManyToManyTables = potentialManyToManyTables;
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
            if (relEfCol.FromToMultiplicities.FromMultiplicity == EfRelationshipTypes.Many)
            {
                //handle from many

                if (relEfCol.FromToMultiplicities.ToMultiplicity == EfRelationshipTypes.Many)
                {
                    //many to many - look for a linking table in the list of _potentialManyToManyTables that has the right foreignKeys

                    var fromEfTable = GetEfTableDataFromClass(tableInfo.ClrClassType);
                    var toEfTable = GetEfTableDataFromCollection(relEfCol);

                    var linkCombinedNames =
                        AllManyToManyTablesThatHaveTheRightForeignKeys(fromEfTable, toEfTable).ToList();

                    if (!linkCombinedNames.Any())
                        status.AddSingleError(
                            "Missing Link Table: EF has a {0} relationship between {1}.{2} and {3} but we could not find a linking table with the right foreign keys.",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toEfTable.TableName);
                    else
                    {
                        //now we check the entries in the linking tab
                        if (linkCombinedNames.Count > 1)
                            status.AddWarning("Ambigous Link Table: EF has a {0} relationship between {1}.{2} and {3}. This was ambigous so we may not have fully checked this.",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toEfTable.TableName);

                        manyToManyTableName = linkCombinedNames.First();
                        //todo: check cascade deletes
                    }
                }
                else
                {
                    //many to one 
                    var fromSqlTable = _sqlInfoDict[tableInfo.CombinedName];
                    var toSqlTable = GetSqlTableDataFromClass(relEfCol.ClrColumnType);

                    if (!_allSqlInfo.ForeignKeys.Any(x => x.ParentTableName == tableInfo.TableName
                                                             && x.ReferencedTableName == toSqlTable.TableName))
                        status.AddSingleError(
                            "Missing Foreign Key: EF has a {0} relationship between {1}.{2} and {3} but we don't find that in SQL",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTable.TableName);
                    //todo: check cascade deletes
                }
            }
            else
            {
                //The parent is single, which implies that the child holds the key

                if (relEfCol.FromToMultiplicities.ToMultiplicity == EfRelationshipTypes.Many)
                {
                    //typical one to many

                    var fromSqlTable = GetSqlTableDataFromCollection(relEfCol);

                    //we match just the table parts sql relational data. We could add the name of the parent primary key, but left out for now
                    if (!_allSqlInfo.ForeignKeys.Any(x => x.ParentTableName == fromSqlTable.TableName
                                                             && x.ReferencedTableName == tableInfo.TableName))
                        status.AddSingleError(
                            "Missing Foreign Key: EF has a {0} relationship between {1} and {2}.{3} but we don't find that in SQL",
                            relEfCol.FromToMultiplicities, fromSqlTable.TableName, tableInfo.TableName,
                            relEfCol.ClrColumnName);
                    //todo: check cascade deletes
                }
                else
                {
                    //one to one/OneOrZero or reverse
                    var fromSqlTable = _sqlInfoDict[tableInfo.CombinedName];
                    var toSqlTable = GetSqlTableDataFromClass(relEfCol.ClrColumnType);

                    //There is one foreign key for both directions. Therefore we need to check both 

                    if (!_allSqlInfo.ForeignKeys.Any(x => x.ParentTableName == toSqlTable.TableName
                                         && x.ReferencedTableName == tableInfo.TableName)
                        && !_allSqlInfo.ForeignKeys.Any(x => x.ParentTableName == tableInfo.TableName
                                         && x.ReferencedTableName == toSqlTable.TableName))
                        status.AddSingleError(
                            "Missing Foreign Key: EF has a {0} relationship between {1}.{2} and {3} but we don't find that in SQL",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTable.TableName);
                }
            }

            return status.HasErrors ? status : status.SetSuccessWithResult(manyToManyTableName, "All Ok");
        }

        //------------------------------------------------------------------------------------------
        //private helpers

        /// <summary>
        /// This returns the names of tables that have a set of foreign keys which map to all of the primary keys of the from/to tables
        /// </summary>
        /// <param name="fromEfTable"></param>
        /// <param name="toEfTable"></param>
        /// <returns></returns>
        private IEnumerable<string> AllManyToManyTablesThatHaveTheRightForeignKeys(
            EfTableInfo fromEfTable, EfTableInfo toEfTable)
        {
            //we form all the keys that we expect to see in the ReferencedTable/Col part of the many-to-many table
            var allKeys =
                fromEfTable.NormalCols.Where(x => x.IsPrimaryKey)
                    .Select(x => FormatHelpers.CombineTableAndColumnNames(fromEfTable.TableName, x.SqlColumnName)).ToList();
            allKeys.AddRange(toEfTable.NormalCols.Where(x => x.IsPrimaryKey)
                    .Select(x => FormatHelpers.CombineTableAndColumnNames(toEfTable.TableName, x.SqlColumnName)));

            return _foreignKeysGroupByParentTableName.Where(x => x.Item2.SequenceEqual(allKeys.OrderBy(y => y))).Select(x => x.Item1);
        }



        private SqlTableInfo GetSqlTableDataFromCollection(EfRelationshipInfo relEfCol)
        {
            if (!relEfCol.ClrColumnType.IsGenericType)
                throw new InvalidOperationException("I expected a generic list etc. here");
            var genArgs = relEfCol.ClrColumnType.GetGenericArguments();

            return GetSqlTableDataFromClass(genArgs[0]);
        }

        private SqlTableInfo GetSqlTableDataFromClass(Type efClassType)
        {
            EfTableInfo efData;
            if (!_efInfosDict.TryGetValue(efClassType, out efData))
                throw new InvalidOperationException("I could not find a EF class of the type " + efClassType.Name);
            return _sqlInfoDict[efData.CombinedName];
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