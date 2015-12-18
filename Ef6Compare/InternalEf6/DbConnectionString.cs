// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DbConnectionString.cs
// Date Created: 2015/12/18
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================

using System;
using System.Data.Entity;

namespace Ef6Compare.InternalEf6
{
    internal static class DbConnectionString
    {
        public static string GetConnectionStringFromDbType<T>(this T db) where T : DbContext, new()
        {
            using (var originalDb = (DbContext) Activator.CreateInstance(typeof (T)))
            {
                return originalDb.Database.Connection.ConnectionString;
            }
        }
    }
}