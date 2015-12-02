#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: CompareSqlSql.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using CompareCore;
using CompareCore.SqlInfo;
using CompareCore.Utils;
using GenericLibsBase;

namespace Ef6Compare
{
    public class CompareSqlSql
    {
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
            var refDbConnection = refDbNameOrConnectionString.GetConfigurationOrActualString();
            var toBeCheckDbConnection = toBeCheckDbNameOrConnectionString.GetConfigurationOrActualString();
            var refDatabaseName = refDbConnection.GetDatabaseNameFromConnectionString();
            var toBeCheckDatabaseName = toBeCheckDbConnection.GetDatabaseNameFromConnectionString();

            var refSqlData = SqlAllInfo.SqlAllInfoFactory(refDbConnection);
            var toBeCheckSqlData = SqlAllInfo.SqlAllInfoFactory(toBeCheckDbConnection);

            var comparer = new SqlCompare(refDatabaseName, toBeCheckDatabaseName, _sqlTableNamesToIgnore, _showMismatchedIndexsAsErrors);
            return comparer.CompareSqlToSql(refSqlData, toBeCheckSqlData);
        }
       
    }
}