#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FormatHelpers.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion
namespace CompareCore.Utils
{
    public static class FormatHelpers
    {
        public static string FormCombinedName(string schemaName, string tableName)
        {
            return string.Format("[{0}].[{1}]", schemaName, tableName);
        }
    }
}