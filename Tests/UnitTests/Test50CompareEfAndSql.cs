#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test60CompareSqlAndSql.cs
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
    public class Test50CompareEfAndSql
    {

        [Test]
        public void Test01CompareEfWithSelfSqlOk()
        {
            using (var db = new EfSchemaCompareDb())
            {
                //SETUP
                var comparer = new CompareEfAndSql();

                //EXECUTE
                var status = comparer.CompareEfWithDb(db);

                //VERIFY
                status.ShouldBeValid();
                Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
            }
        }

        [Test]
        public void Test10CompareEfWithDbUpSqlOk()
        {
            using (var db = new EfSchemaCompareDb())
            {
                //SETUP
                var comparer = new CompareEfAndSql();

                //EXECUTE
                var status = comparer.CompareEfWithDb(db, DatabaseHelpers.DbUpDatabaseConfigName);

                //VERIFY
                status.ShouldBeValid();
                Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
            }
        }

       
    }
}