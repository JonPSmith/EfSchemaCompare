#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FormatHelpers.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
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