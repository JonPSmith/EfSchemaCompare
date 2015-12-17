#region licence
// =====================================================
// Label Project - Application to allow labels to be created and sold online
// Filename: ConnectionHelper.cs
// Date Created: 2015/10/29
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Ef6Compare.Internal
{
    internal static class ConnectionHelper
    {
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
                throw new ArgumentException("The nameOrConnectionString was neither a valid connection string name in the .Config file, or a valid connection string."+
                " The actual error message was " + e.Message);
            }

            return nameOrConnectionString;
        } 
    }
}