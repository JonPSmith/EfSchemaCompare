#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareEfSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Data.Entity;
using System.Linq;
using CompareCore;
using CompareCore.EFInfo;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using Ef6Compare.Internal;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace Ef6Compare
{
    public class CompareEfSql
    {
        private string _sqlDbRefString;
        private readonly string _sqlTableNamesToIgnore;

        /// <summary>
        /// Creates the CompareEfSql comparer.
        /// </summary>
        /// <param name="sqlTableNamesToIgnore">You can supply a comma delimited list of table 
        /// names in the SQL database that you do not want reported as not used. 
        /// The default is EF's __MigrationHistory table and DbUp's SchemaVersions table</param>
        public CompareEfSql(string sqlTableNamesToIgnore = "__MigrationHistory,SchemaVersions")
        {
            _sqlTableNamesToIgnore = sqlTableNamesToIgnore;
        }

        /// <summary>
        /// This will compare the EF schema definition with the database schema it is linked to
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfWithDb(DbContext db)
        {
            _sqlDbRefString = "database";
            return CompareEfWithSql(db, db.Database.Connection.ConnectionString);
        }

        /// <summary>
        /// This will compare the EF schema definition with another SQL database schema
        /// </summary>
        /// <param name="db"></param>
        /// <param name="configOrConnectionString">Either a full connection string or the name of a connection string in Config file</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareEfWithDb(DbContext db, string configOrConnectionString)
        {
            var sqlConnectionString = configOrConnectionString.GetConfigurationOrActualString();
            _sqlDbRefString = string.Format("database '{0}',", sqlConnectionString.GetDatabaseNameFromConnectionString());

            return CompareEfWithSql(db, sqlConnectionString);
        }

        //---------------------------------------------------------------------------
        //private methods

        private ISuccessOrErrors CompareEfWithSql(DbContext db, string sqlConnectionString)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            if (sqlConnectionString == null)
                throw new ArgumentNullException("sqlConnectionString");

            var efInfos = Ef6MetadataDecoder.GetAllEfTablesWithColInfo(db);
            var allSqlInfo = SqlAllInfo.SqlAllInfoFactory(sqlConnectionString);

            var comparer = new EfCompare(_sqlDbRefString, _sqlTableNamesToIgnore);
            return comparer.CompareEfWithSql(efInfos, allSqlInfo);
        }    
    }


}