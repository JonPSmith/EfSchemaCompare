#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: SqlColumnInfo.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.SqlInfo
{
    public class SqlColumnInfo
    {
        internal SqlColumnInfo()
        {
        }

        internal SqlColumnInfo(string columnName, string sqlTypeName, SqlPrimaryKey sqlKeyInfo, bool isNullable, int maxLength)
        {
            ColumnName = columnName;
            SqlTypeName = sqlTypeName;
            if (sqlKeyInfo != null)
            {
                IsPrimaryKey = true;
                PrimaryKeyOrder = sqlKeyInfo.KeyOrder;
            }
            IsNullable = isNullable;
            MaxLength = maxLength;
        }

        public string ColumnName { get; private set; }

        public string SqlTypeName { get; private set; }

        public bool IsPrimaryKey { get; private set; }

        public int PrimaryKeyOrder { get; private set; }

        public bool IsNullable { get; private set; }

        public int MaxLength { get; private set; }

        public override string ToString()
        {
            return string.Format("ColumnName: {0}, SqlTypeName: {1}, IsPrimaryKey: {2}, IsNullable: {3}, MaxLength: {4}",
                ColumnName, SqlTypeName, IsPrimaryKey, IsNullable, MaxLength);
        }
    }
}