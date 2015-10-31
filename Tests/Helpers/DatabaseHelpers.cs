#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DatabaseHelpers.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Configuration;
using System.Data.SqlClient;
using DbUpHelper;
using Tests.EfClasses;
using Tests.SqlCommands;

namespace Tests.Helpers
{
    public static class DatabaseHelpers
    {
        public const string EfDatabaseConfigName = "EfSchemaCompareDb";
        public const string DbUpDatabaseConfigName = "DbUpSchemaCompareDb";

        /// <summary>
        /// Wipes out the existing database and creates a new one using EF
        /// </summary>
        /// <param name="db"></param>
        public static void EfWipeCreateDatabase()
        {
            using (var db = new EfSchemaCompareDb(EfDatabaseConfigName))
            {
                if (db.Database.Exists())
                {
                    db.Database.Delete();
                }
                db.Database.Create();
            }
        }

        /// <summary>
        /// Wipes out the existing database and creates a new one using DbUp
        /// </summary>
        public static void DbUpWipeCreateDatabase()
        {
            var databaseConnectionString = ConfigurationManager.ConnectionStrings[DbUpDatabaseConfigName].ConnectionString;
            var builder = new SqlConnectionStringBuilder(databaseConnectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "";          //remove database, as create database won't work with it

            var nonDatabaseConnectionString = builder.ToString();
            var adoNet = new BasicSqlCommands(nonDatabaseConnectionString);
            if (adoNet.ExecuteRowCount("sys.databases", string.Format("WHERE [Name] = '{0}'", databaseName)).Result == 1)
            {
                adoNet.ExecuteNonQuery("DROP DATABASE " + databaseName).ShouldBeValid();
            }
            adoNet.ExecuteNonQuery("CREATE DATABASE " + databaseName).ShouldBeValid();

            var dbup = new DbUpRunner();
            dbup.ApplyMigrations(databaseConnectionString).ShouldBeValid();
        }
    }
}