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

namespace CompareCore.SqlInfo
{
    public class SqlAllInfo
    {
        public ICollection<SqlTableInfo> TableInfos { get; private set; } 

        public ICollection<SqlForeignKey> ForeignKeys { get; private set; }

        public ICollection<SqlIndex> Indexes { get; private set; }

        private SqlAllInfo(ICollection<SqlTableInfo> tableInfos, ICollection<SqlForeignKey> foreignKeys, ICollection<SqlIndex> indexes)
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
                select (new SqlTableInfo(tableGroup.Key, schemaName,
                    tableGroup.Select(y => new SqlColumnInfo(y.ColumnName, y.ColumnSqlType,
                        primaryKey.SingleOrDefault(z => z.ColumnName == y.ColumnName),
                        y.IsNullable, y.MaxLength)).ToList()));

            var allIndexes = SqlIndex.GetAllIndexes(connection);

            return new SqlAllInfo(tableInfos.ToList(), allForeignKeys, allIndexes);
        }
    }
}