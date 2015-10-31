#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.Linq;
using CompareCore.Utils;

namespace CompareCore.SqlInfo
{
    public class SqlTableInfo
    {
        public string TableName { get; set; }

        public string SchemaName { get; set; }

        public string CombinedName { get { return FormatHelpers.FormCombinedName(SchemaName, TableName); } }

        public ICollection<SqlColumnInfo> ColumnInfo { get; set; }

        public ICollection<SqlForeignKeys> ForeignKeys { get; set; }

        public SqlTableInfo(string tableName, string schemaName, ICollection<SqlColumnInfo> columnInfo, ICollection<SqlForeignKeys> foreignKeys)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ColumnInfo = columnInfo;
            ForeignKeys = foreignKeys;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}.{1}, Columns: {2}, ForeignKeys: {3}", SchemaName, TableName, ColumnInfo.Count, ForeignKeys.Count);
        }


        public static ICollection<SqlTableInfo> GetAllSqlTablesWithColInfo(string connection)
        {
            var allTablesAndCol = SqlTableAndColumnData.GetSqlTablesAndColumns(connection);
            var allForeignKeys = SqlForeignKeys.GetForeignKeys(connection).GroupBy(x => x.ParentTableName);

            var result = from tableGroup in allTablesAndCol.GroupBy(x => x.TableName)
                         let schemaName = tableGroup.First().SchemaName
                         let foreignKeys = allForeignKeys.Where(x => x.Key == tableGroup.Key).SelectMany(x => x)
                         let primaryKey = SqlPrimaryKey.GetPrimaryKeysNames(connection, tableGroup.Key)
                         select (new SqlTableInfo(tableGroup.Key, schemaName,
                             tableGroup.Select(y => new SqlColumnInfo(y.ColumnName, y.ColumnSqlType,
                                 primaryKey.SingleOrDefault(z => z.COLUMN_NAME == y.ColumnName),
                                 y.IsNullable, y.MaxLength)).ToList(),
                             foreignKeys.ToList()));

            return result.ToList();
        }


    }
}