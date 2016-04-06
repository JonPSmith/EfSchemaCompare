#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlTableInfo.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
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
        internal SqlTableInfo() {}

        internal SqlTableInfo(string schemaName, string tableName, IList<SqlColumnInfo> columnInfos)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ColumnInfos = columnInfos;
        }

        public string SchemaName { get; private set; }

        public string TableName { get; private set; }

        public IList<SqlColumnInfo> ColumnInfos { get; private set; }

        public string CombinedName { get { return FormatHelpers.FormCombinedSchemaTableName(SchemaName, TableName); } }

        public override string ToString()
        {
            return string.Format("Name: {0}.{1}, Columns: {2}", SchemaName, TableName, ColumnInfos.Count);
        }
    }
}