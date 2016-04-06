#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test50CompareEfFullWithSql.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using CompareCore;
using Ef6Compare;
using Ef6TestDbContext;
using EfPocoClasses.Relationships;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test50CompareEfFullWithSql
    {
        [Test]
        public void Test02CompareEfWithSqlTOk()
        {
            using (var db = new TestEf6SchemaCompareDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                var status = comparer.CompareEfWithDb<DataTop>(db);

                //VERIFY
                status.ShouldBeValid();
                Console.WriteLine("WARNINGS:\n {0}", string.Join("\n", status.Warnings));
            }
        }

        [Test]
        public void Test11CompareEfWithDbUpSqlTOk()
        {
            using (var db = new TestEf6SchemaCompareDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                var status = comparer.CompareEfWithDb<DataTop>(db, MiscConstants.DbUpDatabaseConfigName);

                //VERIFY
                status.ShouldBeValid();
                status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
            }
        }


        //----------------------------------------------------------------
        //errors

        [Test]
        public void Test40GetEfDataBad()
        {
            using (var db = new TestEf6SchemaCompareDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                var ex =
                    Assert.Throws<InvalidOperationException>(
                        () => comparer.CompareEfWithDb<EfCompare>(db, MiscConstants.DbUpDatabaseConfigName));

                //VERIFY
                ex.Message.ShouldStartWith("Could not find the EF data class");
            }
        }
    }
}