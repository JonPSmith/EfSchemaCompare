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
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace Ef6Compare.Internal
{
    internal class EfRelationshipChecker
    {
        private readonly ICollection<EfTableInfo> _efInfos;
        private readonly Dictionary<string, SqlTableInfo> _sqlInfoDict;

        public EfRelationshipChecker(ICollection<EfTableInfo> efInfos, ICollection<SqlTableInfo> sqlInfo)
        {
            _efInfos = efInfos;
            _sqlInfoDict = sqlInfo.ToDictionary(x => x.CombinedName);
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
            if (relEfCol.FromToMultiplicities.FromMultiplicity == RelationshipMultiplicity.Many)
            {
                //handle from many

                if (relEfCol.FromToMultiplicities.ToMultiplicity == RelationshipMultiplicity.Many)
                {
                    //many to many - look for a linking table

                    var toSqlTable = GetSqlTableDataFromCollection(relEfCol);

                    var linkTableStatus = FindLinkingTable(tableInfo, relEfCol, toSqlTable, out manyToManyTableName);
                    status.Combine(linkTableStatus);

                    if (linkTableStatus.Result != null && !linkTableStatus.Result.ForeignKeys.Any(x => x.ParentTableName == manyToManyTableName
                                                             && x.ReferencedTableName == toSqlTable.TableName))
                        status.AddSingleError(
                            "EF has a {0} relationship between {1}.{2} and {3} and we found the linking table but it did not have the right foreign keys in SQL",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTable.TableName);
                    //todo: check cascade deletes
                }
                else
                {
                    //many to one 
                    var fromSqlTable = _sqlInfoDict[tableInfo.CombinedName];
                    var toSqlTable = GetSqlTableDataFromClass(relEfCol.ClrColumnType);

                    if (!fromSqlTable.ForeignKeys.Any(x => x.ParentTableName == tableInfo.TableName
                                                             && x.ReferencedTableName == toSqlTable.TableName))
                        status.AddSingleError(
                            "EF has a {0} relationship between {1}.{2} and {3} but we don't find that in SQL",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTable.TableName);
                    //todo: check cascade deletes
                }
            }
            else
            {
                //The parent is single, which implies that the child holds the key

                if (relEfCol.FromToMultiplicities.ToMultiplicity == RelationshipMultiplicity.Many)
                {
                    //typical one to many

                    var fromSqlTable = GetSqlTableDataFromCollection(relEfCol);

                    //we match just the table parts sql relational data. We could add the name of the parent primary key, but left out for now
                    if (!fromSqlTable.ForeignKeys.Any(x => x.ParentTableName == fromSqlTable.TableName
                                                             && x.ReferencedTableName == tableInfo.TableName))
                        status.AddSingleError(
                            "EF has a {0} relationship between {1} and {2}.{3} but we don't find that in SQL",
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

                    if (!toSqlTable.ForeignKeys.Any(x => x.ParentTableName == toSqlTable.TableName
                                         && x.ReferencedTableName == tableInfo.TableName)
                        && !fromSqlTable.ForeignKeys.Any(x => x.ParentTableName == tableInfo.TableName
                                         && x.ReferencedTableName == toSqlTable.TableName))
                        status.AddSingleError(
                            "EF has a {0} relationship between {1}.{2} and {3} but we don't find that in SQL",
                            relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName, toSqlTable.TableName);
                }
            }

            return status.HasErrors ? status : status.SetSuccessWithResult(
                manyToManyTableName == null ? null : FormatHelpers.FormCombinedName(tableInfo.SchemaName, manyToManyTableName), 
                "All Ok");
        }

        //------------------------------------------------------------------------------------------
        //private helpers

        /// <summary>
        /// This nasty method tries two names for the linking table, either TableA+TableB or TableB+TableA
        /// </summary>
        /// <param name="tableInfo"></param>
        /// <param name="relEfCol"></param>
        /// <param name="toEfTable"></param>
        /// <param name="toSqlTable"></param>
        /// <param name="manyToManyTableName"></param>
        /// <returns></returns>
        private ISuccessOrErrors<SqlTableInfo> FindLinkingTable(EfTableInfo tableInfo, EfRelationshipInfo relEfCol, 
                        SqlTableInfo toSqlTable, out string manyToManyTableName)
        {
            var status = new SuccessOrErrors<SqlTableInfo>();

            SqlTableInfo linkingTable;
            manyToManyTableName = toSqlTable.TableName + tableInfo.TableName;
            var parentTableCombinedName1 = FormatHelpers.FormCombinedName(tableInfo.SchemaName, manyToManyTableName);
            if (!_sqlInfoDict.TryGetValue(parentTableCombinedName1, out linkingTable))
            {
                manyToManyTableName = tableInfo.TableName + toSqlTable.TableName;
                var parentTableCombinedName2 = FormatHelpers.FormCombinedName(tableInfo.SchemaName, manyToManyTableName);
                if (!_sqlInfoDict.TryGetValue(parentTableCombinedName2, out linkingTable))
                    return status.AddSingleError(
                        "EF has a {0} relationship between {1}.{2} and {3} but we could not find the special EF linking table {4} in the SQL",
                        relEfCol.FromToMultiplicities, tableInfo.TableName, relEfCol.ClrColumnName,
                        toSqlTable.TableName, manyToManyTableName);
            }

            return status.SetSuccessWithResult(linkingTable, "All Ok");
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
            var childEfData = _efInfos.SingleOrDefault(x => x.ClrClassType == efClassType);
            if (childEfData == null)
                throw new InvalidOperationException("I could not find a EF class of the type " + efClassType.Name);
            return _sqlInfoDict[childEfData.CombinedName];
        }
    }
}