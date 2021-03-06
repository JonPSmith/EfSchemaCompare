﻿#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlForeignKey.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using CompareCore.Utils;

namespace CompareCore.SqlInfo
{
    public class SqlForeignKey
    {
        private const string DeleteActionCascade = "CASCADE";

        public string ConstraintName { get; private set; }

        public string SchemaName { get; private set; }

        public string ParentTableName { get; private set; }

        public string ParentTableNameWithScheme 
        {
            get { return FormatHelpers.FormCombinedSchemaTableName(SchemaName, ParentTableName); }
        }

        public string ParentColName { get; private set; }

        public string ReferencedTableName { get; private set; }

        public string ReferencedColName { get; private set; }

        public string DeleteAction { get; private set; }

        public bool IsCascade { get { return DeleteAction == DeleteActionCascade; } }

        public override string ToString()
        {
            return string.Format("Parent: {0}.{1}, Referenced: {2}.{3}", ParentTableName, ParentColName, ReferencedTableName, ReferencedColName);
        }

        //-------------------------------------------------------------------
        //

        public static IList<SqlForeignKey> GetForeignKeys(string connectionString)
        {

            var result = new Collection<SqlForeignKey>();
            using (var sqlcon = new SqlConnection(connectionString))
            {
                var command = sqlcon.CreateCommand();

                //see https://msdn.microsoft.com/en-us/library/ms190196.aspx
                command.CommandText = @"SELECT 
    OBJECT_NAME(constraint_object_id) AS ConstraintName
   ,OBJECT_SCHEMA_NAME(f.parent_object_id) AS SchemaName
   ,OBJECT_NAME(f.parent_object_id) AS ParentTableName
   ,COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ParentColName
   ,OBJECT_NAME (f.referenced_object_id) AS ReferencedTableName
   ,COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColName
   ,delete_referential_action_desc AS DeleteAction
   --,update_referential_action_desc
FROM sys.foreign_keys AS f
INNER JOIN sys.foreign_key_columns AS fc 
   ON f.object_id = fc.constraint_object_id 
--WHERE f.parent_object_id = OBJECT_ID('<schema_name.table_name>')";

                sqlcon.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new SqlForeignKey();
                        var i = 0;
                        //for (int j = 0; j < reader.FieldCount; j++)
                        //{
                        //    object col = reader[j];
                        //    Console.WriteLine("{0}: {1}, type = {2}", j, col, col.GetType());
                        //}
                        row.ConstraintName = reader.GetString(i++);
                        row.SchemaName = reader.GetString(i++);
                        row.ParentTableName = reader.GetString(i++);
                        row.ParentColName = reader.GetString(i++);
                        row.ReferencedTableName = reader.GetString(i++);
                        row.ReferencedColName = reader.GetString(i++);
                        row.DeleteAction = reader.GetString(i++);

                        result.Add(row);
                    }
                }
            }

            return result;
        }
    }
}