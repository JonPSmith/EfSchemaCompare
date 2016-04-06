#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: MiscConstants.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

namespace Tests.Helpers
{
    public class MiscConstants
    {
        public const string Ef6DatabaseConfigName = "TestEf6SchemaCompareDb";

        public const string DbUpDatabaseConfigName = "DbUpSchemaCompareDb";

        public static string GetEfDatabaseConfigName()
        {
            return Ef6DatabaseConfigName;
        }
    }
}