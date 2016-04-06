#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareEfSql.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.Data.Entity;
using System.Reflection;
using CompareCore.Utils;
using Ef6SchemaCompare.InternalEf6;
using GenericLibsBase;

namespace Ef6SchemaCompare
{
    public class CompareEfSql
    {
        private readonly Ef6CompareParts _partComparer;
        private string _sqlDbRefString;

        /// <summary>
        /// Creates the CompareEfSql comparer.
        /// </summary>
        /// <param name="sqlTableNamesToIgnore">You can supply a comma delimited list of table 
        /// names in the SQL database that you do not want reported as not used. 
        /// The default is EF's __MigrationHistory table and DbUp's SchemaVersions table</param>
        public CompareEfSql(string sqlTableNamesToIgnore = "__MigrationHistory,SchemaVersions")
        {
            _partComparer = new Ef6CompareParts(sqlTableNamesToIgnore);
        }

        /// <summary>
        /// This will compare the EF schema definition with the database schema it is linked to
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfWithDb(DbContext db)
        {
            _sqlDbRefString = "database";
            return CompareEfFullWithSql(db, db.Database.Connection.ConnectionString, Assembly.GetAssembly(db.GetType()));
        }

        /// <summary>
        /// This will compare the EF context definition with the database schema it is linked to
        /// Use this version when data classes are in a different assembly to DbContext
        /// </summary>
        /// <typeparam name="T">Should be a EF data class</typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfWithDb<T>(DbContext db) where T : class 
        {
            _sqlDbRefString = "database";
            return CompareEfFullWithSql(db, db.Database.Connection.ConnectionString, typeof(T).Assembly);
        }

        /// <summary>
        /// This will compare the EF context definition with another SQL database schema
        /// </summary>
        /// <param name="db"></param>
        /// <param name="configOrConnectionString">Either a full connection string or the name of a connection string in Config file</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfWithDb(DbContext db, string configOrConnectionString)
        {
            var sqlConnectionString = configOrConnectionString.GetConfigurationOrActualString();
            _sqlDbRefString = string.Format("database '{0}',", sqlConnectionString.GetDatabaseNameFromConnectionString());

            return CompareEfFullWithSql(db, sqlConnectionString, Assembly.GetAssembly(db.GetType()));
        }

        /// <summary>
        /// This will compare the EF context definition with another SQL database schema
        /// Use this version when data classes are in a different assembly to DbContext
        /// </summary>
        /// <typeparam name="T">Should be a EF data class</typeparam>
        /// <param name="db"></param>
        /// <param name="configOrConnectionString">Either a full connection string or the name of a connection string in Config file</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfWithDb<T>(DbContext db, string configOrConnectionString) where T : class
        {
            var sqlConnectionString = configOrConnectionString.GetConfigurationOrActualString();
            _sqlDbRefString = string.Format("database '{0}',", sqlConnectionString.GetDatabaseNameFromConnectionString());

            return CompareEfFullWithSql(db, sqlConnectionString, typeof(T).Assembly);
        }

        //---------------------------------------------------------------------------------
        //These handle the situation where you use multiple DbContexts to cover one database

        /// <summary>
        /// This sets things up for multiple DbContexts covering the same database
        /// </summary>
        /// <param name="db">Uses the connection string from the DbContext to find the SQL database</param>
        /// <returns></returns>
        public void CompareEfPartStart(DbContext db)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            _sqlDbRefString = "database";
            _partComparer.CompareStart(_sqlDbRefString, db.Database.Connection.ConnectionString);
        }

        /// <summary>
        /// This sets things up for multiple DbContexts covering the same database
        /// </summary>
        /// <param name="configOrConnectionString">Either a full connection string or the name of a connection string in Config file</param>
        /// <returns></returns>
        public void CompareEfPartStart(string configOrConnectionString)
        {

            var sqlConnectionString = configOrConnectionString.GetConfigurationOrActualString();
            _sqlDbRefString = string.Format("database '{0}',", sqlConnectionString.GetDatabaseNameFromConnectionString());
            _partComparer.CompareStart(_sqlDbRefString, sqlConnectionString);
        }

        /// <summary>
        /// This will compare the EF context which only covers part of the database schema it is linked to
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfPartWithDb(DbContext db)
        {
            if (!_partComparer.CompareStartCalled)
                throw new InvalidOperationException("You must call CompareEfPartStart before calling CompareEfPartWithDb.");

            return _partComparer.CompareEfPart(db, Assembly.GetAssembly(db.GetType()));
        }

        /// <summary>
        /// This will compare the EF context which only covers part of the database schema it is linked to
        /// Use this version when data classes are in a different assembly to DbContext
        /// </summary>
        /// <typeparam name="T">Should be a EF data class</typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfPartWithDb<T>(DbContext db) where T : class
        {
            if (!_partComparer.CompareStartCalled)
                throw new InvalidOperationException("You must call CompareEfPartStart before calling CompareEfPartWithDb.");

            return _partComparer.CompareEfPart(db, typeof(T).Assembly);
        }

        public ISuccessOrErrors CompareEfPartFinalChecks()
        {
            if (!_partComparer.CompareStartCalled)
                throw new InvalidOperationException("You must call CompareEfPartStart before calling CompareEfPartWithDb or CompareFinish.");

            return _partComparer.CompareFinish();
        }

        //---------------------------------------------------------------------------
        //private methods

        private ISuccessOrErrors CompareEfFullWithSql(DbContext db, string sqlConnectionString, Assembly classesAssembly)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            if (sqlConnectionString == null)
                throw new ArgumentNullException("sqlConnectionString");

            _partComparer.CompareStart(_sqlDbRefString, sqlConnectionString);
            var status = _partComparer.CompareEfPart(db, classesAssembly);
            return status.IsValid 
                ? status.Combine(_partComparer.CompareFinish())
                : status;           //Don't do warnings if there were errors
        }
    }


}