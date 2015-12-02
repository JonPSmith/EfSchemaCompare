#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlIndexe.cs
// Date Created: 2015/12/02
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.SqlInfo
{
    public class SqlIndex
    {
        public string SchemaName { get; private set; } 
        public string TableName	 { get; private set; } 
        public string ColumnName { get; private set; }
        public string IndexName { get; private set; }
        public bool IsPrimaryIndex { get; private set; }
        public bool Clustered { get; private set; }
        public bool IsUnique { get; private set; }

        public string CombinedName { get { return string.Format("[{0}].[{1}].{2}", SchemaName, TableName, ColumnName); } }

        public override string ToString()
        {
            return string.Format("{0}: ({1}primary key, {2}clustered, {3}unique)",
                CombinedName, 
                IsPrimaryIndex ? "" : "not ", 
                Clustered ? "" : "not ",
                IsUnique ? "" : "not ");
        }

        public static ICollection<SqlIndex> GetAllIndexes(string connectionString)
        {
            var result = new Collection<SqlIndex>();
            using (var sqlcon = new SqlConnection(connectionString))
            {
                var command = sqlcon.CreateCommand();

                //see http://stackoverflow.com/questions/765867/list-of-all-index-index-columns-in-sql-server-db
                //Note: we order everything so that we can easily deal with multiple indexes on the same table/column
                command.CommandText = @"SELECT 
     SCHEMA_NAME(t.schema_id) AS SchemaName,
     t.name AS TableName,
	 col.name AS ColumnName,
     ind.name AS IndexName,
     ind.is_primary_key AS PrimaryKey,
	 ind.index_id AS IndexType,
	 ind.is_unique AS IsUnique
FROM 
     sys.indexes ind 
INNER JOIN 
     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
INNER JOIN 
     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
INNER JOIN 
     sys.tables t ON ind.object_id = t.object_id 
WHERE 
     t.is_ms_shipped = 0 
	 AND ind.index_id <> 0	-- No heap
ORDER BY 
     t.schema_id, t.name, ind.is_primary_key, ind.index_id, ind.is_unique, ind.name";


                sqlcon.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new SqlIndex();
                        var i = 0;
                        //for (int j = 0; j < reader.FieldCount; j++)
                        //{
                        //    object col = reader[j];
                        //    Console.WriteLine("{0}: {1}, type = {2}", j, col, col.GetType());
                        //}
                        row.SchemaName = reader.GetString(i++);
                        row.TableName = reader.GetString(i++);
                        row.ColumnName = reader.GetString(i++);
                        row.IndexName = reader.GetString(i++);
                        row.IsPrimaryIndex = reader.GetBoolean(i++);
                        row.Clustered = reader.GetInt32(i++) == 1;
                        row.IsUnique = reader.GetBoolean(i++);

                        result.Add(row);
                    }
                }
            }

            return result;
        }
    }
}