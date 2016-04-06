#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlAllInfo.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.SqlInfo
{
    public class SqlAllInfo
    {
        internal SqlAllInfo() { }

        private SqlAllInfo(IList<SqlTableInfo> tableInfos, IList<SqlForeignKey> foreignKeys, IList<SqlIndex> indexes)
        {
            TableInfos = tableInfos;
            ForeignKeys = foreignKeys;
            Indexes = indexes;
        }

        public IList<SqlTableInfo> TableInfos { get; private set; }

        public IList<SqlForeignKey> ForeignKeys { get; private set; }

        public IList<SqlIndex> Indexes { get; private set; }

        public static SqlAllInfo SqlAllInfoFactory(string connection)
        {
            var allTablesAndCol = SqlTableAndColumnData.GetSqlTablesAndColumns(connection);
            var allForeignKeys = SqlForeignKey.GetForeignKeys(connection);

            var tableInfos = from tableGroup in allTablesAndCol.GroupBy(x => x.TableName)
                let schemaName = tableGroup.First().SchemaName
                let primaryKey = SqlPrimaryKey.GetPrimaryKeysNames(connection, tableGroup.Key)
                select (new SqlTableInfo(schemaName,
                    tableGroup.Key, tableGroup.Select(y => new SqlColumnInfo(y.ColumnName, y.SqlTypeName,
                        primaryKey.SingleOrDefault(z => z.ColumnName == y.ColumnName),
                        y.IsNullable, y.MaxLength)).ToList()));

            var allIndexes = SqlIndex.GetAllIndexes(connection);

            return new SqlAllInfo(tableInfos.ToList(), allForeignKeys, allIndexes);
        }
    }
}