#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlTableAndColumnData.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace CompareCore.SqlInfo
{
    public class SqlTableAndColumnData
    {

        public string TableName { get; set; }

        public string SchemaName { get; set; }

        public string ColumnName { get; set; }

        public string ColumnSqlType { get; set; }

        public bool IsNullable { get; set; }

        public Int16 MaxLength { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}].[{1}].{2}, Type: {3}, IsNullable: {4}, MaxLength: {5}", 
                TableName, SchemaName, ColumnName, ColumnSqlType, IsNullable, MaxLength);
        }


        public static ICollection<SqlTableAndColumnData> GetSqlTablesAndColumns(string connectionString)
        {
            var result = new Collection<SqlTableAndColumnData>();
            using (var sqlcon = new SqlConnection(connectionString))
            {
                var command = sqlcon.CreateCommand();
                command.CommandText = @"SELECT t.name AS TableName,
SCHEMA_NAME(t.schema_id) AS SchemaName,
c.name AS ColumnName,
types.name AS ColumnSqlType,
c.is_nullable AS IsNullable,
c.max_length AS MaxLength
FROM sys.tables AS t
INNER JOIN sys.columns c ON t.OBJECT_ID = c.OBJECT_ID
INNER JOIN sys.types types ON c.system_type_id = types.system_type_id 
                           AND types.name <> 'sysname'
ORDER BY SchemaName, TableName";

                sqlcon.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var row = new SqlTableAndColumnData();
                    var i = 0;
                    //for (int j = 0; j < reader.FieldCount; j++)
                    //{
                    //    object col = reader[j];
                    //    Console.WriteLine("{0}: {1}, type = {2}", j, col, col.GetType());
                    //}
                    row.TableName = reader.GetString(i++);
                    row.SchemaName = reader.GetString(i++);
                    row.ColumnName = reader.GetString(i++);
                    row.ColumnSqlType = reader.GetString(i++);
                    row.IsNullable = reader.GetBoolean(i++);// reader[i++] as bool? ?? false;
                    row.MaxLength = reader.GetInt16(i++);

                    result.Add(row);
                }
                reader.Close();
            }

            return result;
        }
    }
}