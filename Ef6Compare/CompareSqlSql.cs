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

            var comparer = new SqlCompare(refDatabaseName, toBeCheckDatabaseName);
            return comparer.CompareSqlToSql(refSqlData, toBeCheckSqlData);
        }
       
    }
}