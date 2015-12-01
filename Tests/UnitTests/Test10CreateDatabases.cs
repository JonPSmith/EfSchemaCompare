#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test10CreateDatabases.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using DbUpHelper;
using Ef6Compare;
using Ef6Compare.Internal;
using NUnit.Framework;
using Tests.EfClasses;
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
                MiscConstants.EfDatabaseConfigName.GetConnectionStringFromConfigOrCheckItIsValidConnectionString();

            //VERIFY
            conStr.ShouldNotEqualNull();
        }


        [Test]
        public void Test10CreateNewEfDatabase()
        {
            //SETUP

            //EXECUTE
            DatabaseCreators.DeleteAndCreateEfDatabase<EfSchemaCompareDb>(MiscConstants.EfDatabaseConfigName, true);

            //VERIFY
        }

        [Test]
        public void Test20SqlCreatedDatabaseExists()
        {
            //SETUP
            var connectionString = MiscConstants.DbUpDatabaseConfigName.GetConnectionStringFromConfigOrCheckItIsValidConnectionString();

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
                "Bad connection name and not connection string".GetConnectionStringFromConfigOrCheckItIsValidConnectionString());

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
                        DatabaseCreators.DeleteAndCreateEfDatabase<EfSchemaCompareDb>(MiscConstants.EfDatabaseConfigName));

            //VERIFY
            ex.Message.ShouldEqual(
                "You attempted to wipe the main database that your DbContext points to. To do this you must set the second parameter to this method to true.");
        }

    }
}