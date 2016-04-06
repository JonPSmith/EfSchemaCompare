#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test10CreateDatabases.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using CompareCore.Utils;
using DbUpHelper;
using Ef6Compare;
using Ef6TestDbContext;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test10CreateDatabases
    {
        [Test]
        public void Test01CheckDatabaseString()
        {
            //SETUP

            //EXECUTE
            var conStr =
                MiscConstants.GetEfDatabaseConfigName().GetConnectionStringAndCheckValid();

            //VERIFY
            conStr.ShouldNotEqualNull();
        }


        [Test]
        public void Test10CreateNewEfDatabase()
        {
            //SETUP

            //EXECUTE
            DatabaseCreators.DeleteAndCreateEfDatabase<TestEf6SchemaCompareDb>(MiscConstants.GetEfDatabaseConfigName(), true);

            //VERIFY
        }

        [Test]
        public void Test20SqlCreatedDatabaseExists()
        {
            //SETUP
            var connectionString = MiscConstants.DbUpDatabaseConfigName.GetConnectionStringAndCheckValid();

            //EXECUTE
            DatabaseCreators.DeleteAndNewSqlDatabase(connectionString);
            var dbup = new DbUpRunner();
            dbup.ApplyMigrations(connectionString).ShouldBeValid();

            //VERIFY
        }

        //-------------------------------------------------------------
        //errors

        [Test]
        public void Test50CheckDatabaseString()
        {
            //SETUP

            //EXECUTE
             var ex =
                Assert.Throws<ArgumentException>(
                    () =>
                "Bad connection name and not connection string".GetConnectionStringAndCheckValid());

            //VERIFY
            ex.Message.ShouldStartWith(
                "The nameOrConnectionString was neither a valid connection string name in the .Config file, or a valid connection string.");
        }

        [Test]
        public void Test60EfCreatedDatabaseExists()
        {
            //SETUP

            //EXECUTE
            var ex =
                Assert.Throws<InvalidOperationException>(
                    () =>
                        DatabaseCreators.DeleteAndCreateEfDatabase<TestEf6SchemaCompareDb>(MiscConstants.GetEfDatabaseConfigName()));

            //VERIFY
            ex.Message.ShouldEqual(
                "You attempted to wipe the main database that your DbContext points to. To do this you must set the second parameter to this method to true.");
        }
    }
}