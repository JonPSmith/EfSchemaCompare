#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlPrimaryKey.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
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
        public string TABLE_QUALIFIER { get; private set; } 
        public string TABLE_OWNER	 { get; private set; } 
        public string TABLE_NAME	 { get; private set; } 
        public string COLUMN_NAME { get; private set; }
        public Int16 KEY_SEQ { get; private set; } 
        public string PK_NAME { get; private set; } 
        
        public static ICollection<SqlPrimaryKey> GetPrimaryKeysNames(string connectionString, string tableName)
        {
            var result = new Collection<SqlPrimaryKey>();
            using (var sqlcon = new SqlConnection(connectionString))
            {
                var command = sqlcon.CreateCommand();

                //see https://msdn.microsoft.com/en-us/library/ms190196.aspx
                command.CommandText = @"sp_pkeys @TableName";
                var param = new SqlParameter();
                param.ParameterName = "@TableName";
                param.Value = tableName;
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
                        row.TABLE_QUALIFIER = reader.GetString(i++);
                        row.TABLE_OWNER = reader.GetString(i++);
                        row.TABLE_NAME = reader.GetString(i++);
                        row.COLUMN_NAME = reader.GetString(i++);
                        row.KEY_SEQ = reader.GetInt16(i++);
                        row.PK_NAME = reader.GetString(i++);

                        result.Add(row);
                    }
                }
            }

            return result;
        }
    }
}