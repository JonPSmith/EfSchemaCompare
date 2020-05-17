﻿#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlPrimaryKey.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.SqlInfo
{
    public class SqlPrimaryKey
    {
        public string DatabaseName { get; private set; }
        public string SchemaName	 { get; private set; }
        public string TableName	 { get; private set; }
        public string ColumnName { get; private set; }
        public Int16 KeyOrder { get; private set; }
        public string PrimaryKeyConstraintName { get; private set; }

        public static IList<SqlPrimaryKey> GetPrimaryKeysNames(string connectionString, string tableName, string schema)
        {
            var result = new Collection<SqlPrimaryKey>();
            using (var sqlcon = new SqlConnection(connectionString))
            {
                var command = sqlcon.CreateCommand();

                //see https://msdn.microsoft.com/en-us/library/ms190196.aspx
                command.CommandText = @"sp_pkeys @TableName, @table_owner";
                var param = new SqlParameter
                {
                    ParameterName = "@TableName",
                    Value = tableName
                };
                command.Parameters.Add(param);
                param = new SqlParameter
                {
                    ParameterName = "@table_owner",
                    Value = schema
                };
                command.Parameters.Add(param);
                sqlcon.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new SqlPrimaryKey();
                        var i = 0;
                        //for (int j = 0; j < reader.FieldCount; j++)
                        //{
                        //    object col = reader[j];
                        //    Console.WriteLine("{0}: {1}, type = {2}", j, col, col.GetType());
                        //}
                        row.DatabaseName = reader.GetString(i++);
                        row.SchemaName = reader.GetString(i++);
                        row.TableName = reader.GetString(i++);
                        row.ColumnName = reader.GetString(i++);
                        row.KeyOrder = reader.GetInt16(i++);
                        row.PrimaryKeyConstraintName = reader.GetString(i++);

                        result.Add(row);
                    }
                }
            }

            return result;
        }
    }
}