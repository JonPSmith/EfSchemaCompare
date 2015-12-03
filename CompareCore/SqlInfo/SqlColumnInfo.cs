#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlColumnInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.SqlInfo
{
    public class SqlColumnInfo
    {
        public string ColumnName { get; private set; }

        public string ColumnSqlType { get; private set; }

        public bool IsPrimaryKey { get; private set; }

        public int PrimaryKeyOrder { get; private set; }

        public bool IsNullable { get; private set; }

        public int MaxLength { get; private set; }

        internal SqlColumnInfo()
        {
        }

        internal SqlColumnInfo(string columnName, string columnSqlType, SqlPrimaryKey sqlKeyInfo, bool isNullable, int maxLength)
        {
            ColumnName = columnName;
            ColumnSqlType = columnSqlType;
            if (sqlKeyInfo != null)
            {
                IsPrimaryKey = true;
                PrimaryKeyOrder = sqlKeyInfo.KeyOrder;
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