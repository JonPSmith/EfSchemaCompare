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
using CompareEfSql;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test50CompareEfAndSql
    {

        //----------------------------------------------------------
        //EF to SQL

        [Test]
        public void Test01CompareEfWithSelfSqlOk()
        {
            using (var db = new EfSchemaCompareDb())
            {
                //SETUP

                //EXECUTE
                var status = db.CompareEfWithDb();

                //VERIFY
                status.ShouldBeValid();
                Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
            }
        }

        //----------------------------------------------------
        //SQL to SQL

        [Test]
        public void Test50CompareSqlToSqlSelfOk()
        {
            //SETUP
            var connection = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;

            //EXECUTE
            var status = connection.CompareSqlToSql(connection);

            //VERIFY
            status.ShouldBeValid();
            status.HasWarnings.ShouldEqual(false, string.Join("\n", status.Warnings));
        }

        [Test]
        public void Test51CompareSqlToSqlSelfOk()
        {
            //SETUP

            //EXECUTE
            var status = DatabaseHelpers.EfDatabaseConfigName.CompareSqlToSql(DatabaseHelpers.EfDatabaseConfigName);

            //VERIFY
            status.ShouldBeValid();
            status.HasWarnings.ShouldEqual(false, string.Join("\n", status.Warnings));
        }

        [Test]
        public void Test60CompareSqlToSqlDbUpOk()
        {
            //SETUP
            var connection1 = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;
            var connection2 = ConfigurationManager.ConnectionStrings[DatabaseHelpers.DbUpDatabaseConfigName].ConnectionString;

            //EXECUTE
            var status = connection1.CompareSqlToSql(connection2);

            //VERIFY
            status.ShouldBeValid(false);
            Console.WriteLine("ERRORS:\n {0}", status.GetAllErrors());
            Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
        }


        [Test]
        public void Test61CompareSqlToSqlDbUpOk()
        {
            //SETUP

            //EXECUTE
            var status = DatabaseHelpers.EfDatabaseConfigName.CompareSqlToSql(DatabaseHelpers.DbUpDatabaseConfigName);

            //VERIFY
            status.ShouldBeValid(false);
            Console.WriteLine("ERRORS:\n {0}", status.GetAllErrors());
            Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
        }
    }
}