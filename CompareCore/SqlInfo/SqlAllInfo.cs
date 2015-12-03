#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlAllInfo.cs
// Date Created: 2015/11/02
// © Copyright Selective Analytics 2015. All rights reserved
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
        public IList<SqlTableInfo> TableInfos { get; private set; } 

        public IList<SqlForeignKey> ForeignKeys { get; private set; }

        public IList<SqlIndex> Indexes { get; private set; }

        internal SqlAllInfo() { }

        private SqlAllInfo(IList<SqlTableInfo> tableInfos, IList<SqlForeignKey> foreignKeys, IList<SqlIndex> indexes)
        {
            TableInfos = tableInfos;
            ForeignKeys = foreignKeys;
            Indexes = indexes;
        }

        public static SqlAllInfo SqlAllInfoFactory(string connection)
        {
            var allTablesAndCol = SqlTableAndColumnData.GetSqlTablesAndColumns(connection);
            var allForeignKeys = SqlForeignKey.GetForeignKeys(connection);

            var tableInfos = from tableGroup in allTablesAndCol.GroupBy(x => x.TableName)
                let schemaName = tableGroup.First().SchemaName
                let primaryKey = SqlPrimaryKey.GetPrimaryKeysNames(connection, tableGroup.Key)
                select (new SqlTableInfo(schemaName,
                    tableGroup.Key, tableGroup.Select(y => new SqlColumnInfo(y.ColumnName, y.ColumnSqlType,
                        primaryKey.SingleOrDefault(z => z.ColumnName == y.ColumnName),
                        y.IsNullable, y.MaxLength)).ToList()));

            var allIndexes = SqlIndex.GetAllIndexes(connection);

            return new SqlAllInfo(tableInfos.ToList(), allForeignKeys, allIndexes);
        }
    }
}