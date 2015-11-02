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
        public ICollection<SqlTableInfo> TableInfos { get; set; } 

        public ICollection<SqlForeignKeys> ForeignKeys { get; set; }

        private SqlAllInfo(ICollection<SqlTableInfo> tableInfos, ICollection<SqlForeignKeys> foreignKeys)
        {
            TableInfos = tableInfos;
            ForeignKeys = foreignKeys;
        }

        public static SqlAllInfo SqlAllInfoFactory(string connection)
        {
            var allTablesAndCol = SqlTableAndColumnData.GetSqlTablesAndColumns(connection);
            var allForeignKeys = SqlForeignKeys.GetForeignKeys(connection);

            var tableInfos = from tableGroup in allTablesAndCol.GroupBy(x => x.TableName)
                let schemaName = tableGroup.First().SchemaName
                let primaryKey = SqlPrimaryKey.GetPrimaryKeysNames(connection, tableGroup.Key)
                select (new SqlTableInfo(tableGroup.Key, schemaName,
                    tableGroup.Select(y => new SqlColumnInfo(y.ColumnName, y.ColumnSqlType,
                        primaryKey.SingleOrDefault(z => z.COLUMN_NAME == y.ColumnName),
                        y.IsNullable, y.MaxLength)).ToList()));

            return new SqlAllInfo(tableInfos.ToList(), allForeignKeys);
        }
    }
}