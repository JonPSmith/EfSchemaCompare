#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlColumnInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion
namespace CompareCore.SqlInfo
{
    public class SqlColumnInfo
    {
        public string ColumnName { get; set; }

        public string ColumnSqlType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public int PrimaryKeyOrder { get; set; }

        public bool IsNullable { get; set; }

        public int MaxLength { get; set; }

        public SqlColumnInfo(string columnName, string columnSqlType, SqlPrimaryKey sqlKeyInfo, bool isNullable, int maxLength)
        {
            ColumnName = columnName;
            ColumnSqlType = columnSqlType;
            if (sqlKeyInfo != null)
            {
                IsPrimaryKey = true;
                PrimaryKeyOrder = sqlKeyInfo.KEY_SEQ;
            }
            IsNullable = isNullable;
            MaxLength = maxLength;
        }

        public override string ToString()
        {
            return string.Format("ColumnName: {0}, ColumnSqlType: {1}, IsPrimaryKey: {2}, IsNullable: {3}, MaxLength: {4}",
                ColumnName, ColumnSqlType, IsPrimaryKey, IsNullable, MaxLength);
        }
    }
}