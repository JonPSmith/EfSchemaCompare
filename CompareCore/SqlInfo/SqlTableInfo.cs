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
        public string TableName { get; private set; }

        public string SchemaName { get; private set; }

        public string CombinedName { get { return FormatHelpers.FormCombinedSchemaTableName(SchemaName, TableName); } }

        public ICollection<SqlColumnInfo> ColumnInfo { get; private set; }

        public SqlTableInfo(string tableName, string schemaName, ICollection<SqlColumnInfo> columnInfo)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ColumnInfo = columnInfo;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}.{1}, Columns: {2}, ForeignKeys: {3}", SchemaName, TableName, ColumnInfo.Count);
        }

    }
}