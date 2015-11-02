#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FormatHelpers.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;

namespace CompareCore.Utils
{
    public static class FormatHelpers
    {
        public static string FormCombinedSchemaTableName(string schemaName, string tableName)
        {
            return String.Format("[{0}].[{1}]", schemaName, tableName);
        }

        public static string CombineTableAndColumnNames(string tableName, string columnName)
        {
            return tableName + "." + columnName;
        }
    }
}