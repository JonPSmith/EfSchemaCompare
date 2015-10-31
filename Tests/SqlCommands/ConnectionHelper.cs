#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ConnectionHelper.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Configuration;

namespace Tests.SqlCommands
{
    public static class ConnectionHelper
    {
        public static string GetConfigurationOrActualString(this string connectionStringOrConfig)
        {
            var connectionFromConfigFile = ConfigurationManager.ConnectionStrings[connectionStringOrConfig];
            return connectionFromConfigFile == null ? connectionStringOrConfig : connectionFromConfigFile.ConnectionString;
        }
    }


}