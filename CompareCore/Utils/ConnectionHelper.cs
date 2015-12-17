#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ConnectionHelper.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CompareCore.Utils
{
    public static class ConnectionHelper
    {
        public static string GetConfigurationOrActualString(this string refDbConnection)
        {
            var connectionFromConfigFile = ConfigurationManager.ConnectionStrings[refDbConnection];
            return connectionFromConfigFile == null ? refDbConnection : connectionFromConfigFile.ConnectionString;
        }

        public static string GetDatabaseNameFromConnectionString(this string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }

        public static string GetConnectionStringAndCheckValid(this string nameOrConnectionString)
        {
            var connectionFromConfigFile = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (connectionFromConfigFile != null)
                return connectionFromConfigFile.ConnectionString;

            try
            {
                var builder = new SqlConnectionStringBuilder(nameOrConnectionString);
            }
            catch (Exception e)
            {
                throw new ArgumentException("The nameOrConnectionString was neither a valid connection string name in the .Config file, or a valid connection string." +
                " The actual error message was " + e.Message);
            }

            return nameOrConnectionString;
        }
    }
}