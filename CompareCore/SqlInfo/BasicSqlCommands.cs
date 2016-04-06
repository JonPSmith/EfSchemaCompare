#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: BasicSqlCommands.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Data.SqlClient;
using CompareCore.Utils;
using GenericLibsBase;
using GenericLibsBase.Core;

namespace CompareCore.SqlInfo
{
    public class BasicSqlCommands
    {
        private readonly string _connectionString;

        public BasicSqlCommands(string nameOrConnectionString)
        {
            _connectionString = nameOrConnectionString.GetConfigurationOrActualString();
        }

        public ISuccessOrErrors<int> ExecuteNonQuery(string command)
        {
            var status = new SuccessOrErrors<int>();

            using (var myConn = new SqlConnection(_connectionString))
            {
                var myCommand = new SqlCommand(command, myConn);
                try
                {
                    myConn.Open();
                    var numRows = myCommand.ExecuteNonQuery();
                    return status.SetSuccessWithResult(numRows, "Successfully ran sql non-query.");
                }
                catch (System.Exception ex)
                {
                    return status.AddSingleError("{0}: in sql non-query command. Message = {1}", ex.GetType().Name,
                        ex.Message);
                }
            }
        }

        public ISuccessOrErrors<int> ExecuteRowCount(string tableName, string whereClause = "")
        {
            var status = new SuccessOrErrors<int>();
            using (var myConn = new SqlConnection(_connectionString))
            {
                var command = "SELECT COUNT(*) FROM " + tableName + " " + whereClause;

                var myCommand = new SqlCommand(command, myConn);
                try
                {
                    myConn.Open();
                    return status.SetSuccessWithResult((int)myCommand.ExecuteScalar(), "Successfully ran sql row query count.");
                }
                catch (System.Exception ex)
                {
                    return status.AddSingleError("{0}: in sql query command. Message = {1}", ex.GetType().Name,
                        ex.Message);
                }
            }
        }
    }
}