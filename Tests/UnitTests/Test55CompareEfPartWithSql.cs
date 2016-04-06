#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test55CompareEfPartWithSql.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using Ef6SchemaCompare;
using Ef6TestDbContext;
using EfPocoClasses.Relationships;
using GenericLibsBase;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test55CompareEfPartWithSql
    {
        //------------------------------------------------------
        //test each sub-DbContext on its own

        [Test]
        public void Test01CompareEfPartDataTypesDbOk()
        {
            using (var db = new TestEf6DataTypesDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                comparer.CompareEfPartStart(db);
                var status = comparer.CompareEfPartWithDb<DataTop>(db);

                //VERIFY
                status.ShouldBeValid();
                status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
            }
        }

        [Test]
        public void Test02CompareEfPartComplexDbOk()
        {
            using (var db = new TestEf6ComplexDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                comparer.CompareEfPartStart(db);
                var status = comparer.CompareEfPartWithDb<DataTop>(db);

                //VERIFY
                status.ShouldBeValid();
                status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
            }
        }

        [Test]
        public void Test03CompareEfPartPublicPrivateDbOk()
        {
            using (var db = new TestEf6PublicPrivateDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                comparer.CompareEfPartStart(db);
                var status = comparer.CompareEfPartWithDb<DataTop>(db);

                //VERIFY
                status.ShouldBeValid();
                status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
            }
        }

        [Test]
        public void Test04CompareEfPartRelationshipsDbOk()
        {
            using (var db = new TestEf6RelationshipsDb())
            {
                //SETUP
                var comparer = new CompareEfSql();

                //EXECUTE
                comparer.CompareEfPartStart(db);
                var status = comparer.CompareEfPartWithDb<DataTop>(db);

                //VERIFY
                status.ShouldBeValid();
                status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
            }
        }

        //-------------------------------------------------------
        //now test all together in sequence


        [Test]
        public void Test10CompareAllPartsNoFinishOk()
        {
            //SETUP
            var comparer = new CompareEfSql();
            ISuccessOrErrors status;

            //EXECUTE
            using (var db = new TestEf6DataTypesDb())
            {
                comparer.CompareEfPartStart(db);
                status = comparer.CompareEfPartWithDb<DataTop>(db);
            }
            using (var db = new TestEf6ComplexDb())
            {
                status.Combine(comparer.CompareEfPartWithDb<DataTop>(db));
            }
            using (var db = new TestEf6PublicPrivateDb())
            {
                status.Combine(comparer.CompareEfPartWithDb<DataTop>(db));
            }
            using (var db = new TestEf6RelationshipsDb())
            {
                status.Combine(comparer.CompareEfPartWithDb<DataTop>(db));
            }

            //VERIFY
            status.ShouldBeValid();
            status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
        }

        [Test]
        public void Test11CompareAllPartsWithFinishOk()
        {
            //SETUP
            var comparer = new CompareEfSql();
            ISuccessOrErrors status;

            //EXECUTE
            using (var db = new TestEf6DataTypesDb())
            {
                comparer.CompareEfPartStart(db);
                status = comparer.CompareEfPartWithDb<DataTop>(db);
            }
            using (var db = new TestEf6ComplexDb())
            {
                status.Combine(comparer.CompareEfPartWithDb<DataTop>(db));
            }
            using (var db = new TestEf6PublicPrivateDb())
            {
                status.Combine(comparer.CompareEfPartWithDb<DataTop>(db));
            }
            using (var db = new TestEf6RelationshipsDb())
            {
                status.Combine(comparer.CompareEfPartWithDb<DataTop>(db));
            }
            status.Combine(comparer.CompareEfPartFinalChecks());

            //VERIFY
            status.ShouldBeValid();
            status.Warnings.Count.ShouldEqual(0, string.Join("\n", status.Warnings));
        }


        //--------------------------------------------------------
        //now errors

        [Test]
        public void Test40CompareEfPartFinishHasMissingTablesBad()
        {
            using (var db = new TestEf6DataTypesDb())
            {
                //SETUP
                var comparer = new CompareEfSql();
                comparer.CompareEfPartStart(db);
                comparer.CompareEfPartWithDb<DataTop>(db).ShouldBeValid();

                //EXECUTE
                var status = comparer.CompareEfPartFinalChecks();

                //VERIFY
                status.ShouldBeValid();
                status.HasWarnings.ShouldEqual(true);
            }
        }

        [Test]
        public void Test45CompareEfPartCompareSameDbTwiceBad()
        {
            using (var db = new TestEf6PublicPrivateDb())
            {
                //SETUP
                var comparer = new CompareEfSql();
                comparer.CompareEfPartStart(db);

                //EXECUTE
                comparer.CompareEfPartWithDb<DataTop>(db).ShouldBeValid();
                var status = comparer.CompareEfPartWithDb<DataTop>(db);     //second time should fail as publicPrivate already used

                //VERIFY
                status.ShouldBeValid(false);
                status.GetAllErrors().ShouldEqual("Missing Table: The SQL database does not contain a table called [dbo].[DataPublicPrivate]. Needed by EF class DataPublicPrivate.");
            }
        }
    }
}