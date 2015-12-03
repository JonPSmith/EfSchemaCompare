#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompareCore.Utils;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.SqlInfo
{
    public class SqlTableInfo
    {
        public string SchemaName { get; private set; }

        public string TableName { get; private set; }

        public IList<SqlColumnInfo> ColumnInfo { get; private set; }

        public string CombinedName { get { return FormatHelpers.FormCombinedSchemaTableName(SchemaName, TableName); } }

        internal SqlTableInfo() {}

        internal SqlTableInfo(string schemaName, string tableName, IList<SqlColumnInfo> columnInfo)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ColumnInfo = columnInfo;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}.{1}, Columns: {2}", SchemaName, TableName, ColumnInfo.Count);
        }

    }
}