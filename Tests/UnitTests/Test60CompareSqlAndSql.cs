#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test50CompareEfAndSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Configuration;
using Ef6Compare;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test60CompareSqlAndSql
    {

        [Test]
        public void Test01CompareSqlToSqlSelfOk()
        {
            //SETUP
            var connection = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;
            var comparer = new CompareSqlSql();

            //EXECUTE
            var status = comparer.CompareSqlToSql(connection, connection);

            //VERIFY
            status.ShouldBeValid();
            status.HasWarnings.ShouldEqual(false, string.Join("\n", status.Warnings));
        }

        [Test]
        public void Test10CompareSqlToSqlDbUpOk()
        {
            //SETUP
            var connection1 = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;
            var connection2 = ConfigurationManager.ConnectionStrings[DatabaseHelpers.DbUpDatabaseConfigName].ConnectionString;
            var comparer = new CompareSqlSql();

            //EXECUTE
            var status = comparer.CompareSqlToSql( connection1, connection2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Table: The 'TestEfSchemaCompareDb' SQL database has a table called [dbo].[__MigrationHistory], which is missing in the 'TestDbUpSchemaCompareDb' database.");
            Console.WriteLine("ERRORS:\n {0}", status.GetAllErrors());
            Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
        }


        [Test]
        public void Test11CompareSqlToSqlDbUpWithConfigLookupOk()
        {
            //SETUP
            var comparer = new CompareSqlSql();

            //EXECUTE
            var status = comparer.CompareSqlToSql(DatabaseHelpers.EfDatabaseConfigName, DatabaseHelpers.DbUpDatabaseConfigName);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Table: The 'TestEfSchemaCompareDb' SQL database has a table called [dbo].[__MigrationHistory], which is missing in the 'TestDbUpSchemaCompareDb' database.");
            Console.WriteLine("ERRORS:\n {0}", status.GetAllErrors());
            Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
        }
    }
}