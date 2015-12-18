#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: MiscConstants.cs
// Date Created: 2015/12/01
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using Ef6TestDbContext;

namespace Tests.Helpers
{
    public class MiscConstants
    {
        public const string Ef6DatabaseConfigName = "TestEf6SchemaCompareDb";

        public static string GetEfDatabaseConfigName()
        {
            return Ef6DatabaseConfigName;
        }
        
        public const string DbUpDatabaseConfigName = "DbUpSchemaCompareDb"; 
    }
}