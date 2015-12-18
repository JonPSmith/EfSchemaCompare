#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareSqlSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Data.Entity;
using System.Data.SqlClient;
using CompareCore;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;

namespace Ef6Compare
{
    public class CompareSqlSql
    {
        public const string EfGeneratedDatabasePrefix = ".EfGenerated";

        private readonly bool _showMismatchedIndexsAsErrors;
        private readonly string _sqlTableNamesToIgnore;

        /// <summary>
        /// Creates the CompareEfSql comparer.
        /// </summary>
        /// <param name="showMismatchedIndexsAsErrors">If true then any mismatched, non primary-key indexes will be added to errors, 
        /// otherwise they show up as warnings</param>
        /// <param name="sqlTableNamesToIgnore">You can supply a comma delimited list of table 
        /// names in the SQL database that you do not want reported as not used. 
        /// The default is EF's __MigrationHistory table and DbUp's SchemaVersions table</param>
        public CompareSqlSql(bool showMismatchedIndexsAsErrors = true, string sqlTableNamesToIgnore = "__MigrationHistory,SchemaVersions")
        {
            _showMismatchedIndexsAsErrors = showMismatchedIndexsAsErrors;
            _sqlTableNamesToIgnore = sqlTableNamesToIgnore;
        }

        /// <summary>
        /// This compares two SQL databases looking at each table, its columns, its keys and its foreign keys.
        /// It assumes the first database connection is the reference and the second is the one that should match the reference
        /// </summary>
        /// <param name="refDbNameOrConnectionString">Either a full connection string or the name of a connection string in Config file</param>
        /// <param name="toBeCheckDbNameOrConnectionString">Either a full connection string or the name of a to connection string in Config file</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareSqlToSql(string refDbNameOrConnectionString, string toBeCheckDbNameOrConnectionString)
        {
            var refDbConnection = refDbNameOrConnectionString.GetConnectionStringAndCheckValid();
            var toBeCheckDbConnection = toBeCheckDbNameOrConnectionString.GetConnectionStringAndCheckValid();
            var refDatabaseName = refDbConnection.GetDatabaseNameFromConnectionString();
            var toBeCheckDatabaseName = toBeCheckDbConnection.GetDatabaseNameFromConnectionString();

            var refSqlData = SqlAllInfo.SqlAllInfoFactory(refDbConnection);
            var toBeCheckSqlData = SqlAllInfo.SqlAllInfoFactory(toBeCheckDbConnection);

            var comparer = new SqlCompare(refDatabaseName, toBeCheckDatabaseName, _sqlTableNamesToIgnore, _showMismatchedIndexsAsErrors);
            return comparer.CompareSqlToSql(refSqlData, toBeCheckSqlData);
        }

        /// <summary>
        /// This creates a new database based on the DbContext you give it, but with a new name consisting of the orginial name with
        /// ".EfGenerated" on the end. It then proceeds to check your SQL database against the EF Generated database
        /// NOTE: This sets a null database initializer on the database. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refDbNameOrConnectionString"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public ISuccessOrErrors CompareSqlToEfGeneratedSql<T>(string refDbNameOrConnectionString, T dbContext) where T : DbContext, new()
        {
            var refDbConnection = refDbNameOrConnectionString.GetConnectionStringAndCheckValid();
            var toBeCheckDbConnection = FormEfGeneratedConnectionString(dbContext);
            //This creates the EF database with the new name
            DatabaseCreators.DeleteAndCreateEfDatabase<T>(toBeCheckDbConnection);

            var refDatabaseName = refDbConnection.GetDatabaseNameFromConnectionString();
            var toBeCheckDatabaseName = toBeCheckDbConnection.GetDatabaseNameFromConnectionString();

            var refSqlData = SqlAllInfo.SqlAllInfoFactory(refDbConnection);
            var toBeCheckSqlData = SqlAllInfo.SqlAllInfoFactory(toBeCheckDbConnection);

            var comparer = new SqlCompare(refDatabaseName, toBeCheckDatabaseName, _sqlTableNamesToIgnore, _showMismatchedIndexsAsErrors);
            return comparer.CompareSqlToSql(refSqlData, toBeCheckSqlData);
        }

        //----------------------------------------------------------------------
        //private methods

        private static string FormEfGeneratedConnectionString(DbContext dbContext)
        {
            var builder = new SqlConnectionStringBuilder(dbContext.Database.Connection.ConnectionString);
            builder.InitialCatalog = builder.InitialCatalog + EfGeneratedDatabasePrefix;
            return builder.ToString();
        }
    }
}