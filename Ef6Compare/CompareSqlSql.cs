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
        private string _sqlDbRefString;
        private readonly string _sqlTableNamesToIgnore;

        /// <summary>
        /// Creates the CompareEfSql comparer.
        /// </summary>
        /// <param name="sqlTableNamesToIgnore">You can supply a comma delimited list of table 
        /// names in the SQL database that you do not want reported as not used. 
        /// The default is EF's __MigrationHistory table and DbUp's SchemaVersions table</param>
        public CompareSqlSql(string sqlTableNamesToIgnore = "__MigrationHistory,SchemaVersions")
        {
            _sqlTableNamesToIgnore = sqlTableNamesToIgnore;
        }
        
        /// <summary>
        /// This compares two SQL databases looking at each table, its columns, its keys and its foreign keys.
        /// It assumes the first database connection is the reference and the second is the one that should match the reference
        /// </summary>
        /// <param name="refDbConnectionOrConfig">Either a full connection string or the name of a connection string in Config file</param>
        /// <param name="toBeCheckDbConnectionOrConfig">Either a full connection string or the name of a to connection string in Config file</param>
        /// <returns></returns>
        public ISuccessOrErrors CompareSqlToSql(string refDbConnectionOrConfig, string toBeCheckDbConnectionOrConfig)
        {
            var refDbConnection = refDbConnectionOrConfig.GetConfigurationOrActualString();
            var toBeCheckDbConnection = toBeCheckDbConnectionOrConfig.GetConfigurationOrActualString();
            var refDatabaseName = refDbConnection.GetDatabaseNameFromConnectionString();
            var toBeCheckDatabaseName = toBeCheckDbConnection.GetDatabaseNameFromConnectionString();

            var refSqlData = SqlAllInfo.SqlAllInfoFactory(refDbConnection);
            var toBeCheckSqlData = SqlAllInfo.SqlAllInfoFactory(toBeCheckDbConnection);

            var comparer = new SqlCompare(refDatabaseName, toBeCheckDatabaseName, _sqlTableNamesToIgnore);
            return comparer.CompareSqlToSql(refSqlData, toBeCheckSqlData);
        }
       
    }
}