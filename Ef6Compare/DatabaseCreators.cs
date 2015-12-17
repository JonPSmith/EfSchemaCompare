#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DatabaseCreators.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.SqlClient;
using CompareCore.SqlInfo;
using CompareCore.Utils;

namespace Ef6Compare
{
    public static class DatabaseCreators
    {

        /// <summary>
        /// Wipes out the existing database and creates a new one using your EF
        /// </summary>
        /// <typeparam name="T">The type of your own DbContext</typeparam>
        /// <param name="nameOrConnectionString">the name of a connection string in your .Config file, or a valid connection string</param>
        /// <param name="yesIReallyWantToWipeMyMainDatabase">Set this to true if you want to wipe the database that your normal DbContext points to</param>
        public static void DeleteAndCreateEfDatabase<T>(string nameOrConnectionString, bool yesIReallyWantToWipeMyMainDatabase = false) where T : DbContext, new()
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException("nameOrConnectionString", "You must provide the name of a connectionString in your .Config file, or a valif connection string.");
            try
            {
                using (var db = (DbContext) Activator.CreateInstance(typeof(T), new []{ nameOrConnectionString}))
                {
                    if (!yesIReallyWantToWipeMyMainDatabase)
                    {
                        using (var originalDb = (DbContext) Activator.CreateInstance(typeof(T)))
                        {
                            if (originalDb.Database.Connection.ConnectionString ==
                                nameOrConnectionString.GetConnectionStringAndCheckValid())
                            {
                                //attempt to wipe the actual database
                                throw new InvalidOperationException("You attempted to wipe the main database that your DbContext points to. " +
                                "To do this you must set the second parameter to this method to true.");
                            }
                        }
                    }
                    Database.SetInitializer<T>(null);

                    if (db.Database.Exists())
                    {
                        db.Database.Delete();
                    }
                    db.Database.Create();
                }
            }
            catch (MissingMethodException e)
            {
                throw new MissingMethodException(
                    "Could not find an contructor that take a connection string as an argument."+
                    "Please add public <YourDbContext>(string nameOrConnectionString) : base(nameOrConnectionString){} ");
            }
        }

        /// <summary>
        /// Wipes out the existing database and creates a new, empty one using Sql commands only
        /// Use this to create a database prior to running some sort of migration scripts to build it to the format you want.
        /// </summary>
        /// <param name="nameOrConnectionString">the name of a connection string in your .Config file, or a valid connection string</param>
        public static void DeleteAndNewSqlDatabase(string nameOrConnectionString)
        {
            var databaseConnectionString =
                nameOrConnectionString.GetConnectionStringAndCheckValid();
            var builder = new SqlConnectionStringBuilder(databaseConnectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "";          //remove database, as create database won't work with it

            var nonDatabaseConnectionString = builder.ToString();
            var adoNet = new BasicSqlCommands(nonDatabaseConnectionString);
            if (adoNet.ExecuteRowCount("sys.databases", String.Format("WHERE [Name] = '{0}'", databaseName)).Result == 1)
            {
                if (adoNet.ExecuteNonQuery("DROP DATABASE " + databaseName).HasErrors)
                    throw new InvalidOperationException("Could not drop the database. Is it in use?");
            }
            if (adoNet.ExecuteNonQuery("CREATE DATABASE " + databaseName).HasErrors)
                throw new InvalidOperationException("Failed to create a new, empty database.");
        }
    }
}