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
        public static string GetEfDatabaseConfigName()
        {
            return TestEf6SchemaCompareDb.EfDatabaseConfigName;
        }
        


        public const string DbUpDatabaseConfigName = "DbUpSchemaCompareDb"; 
    }
}