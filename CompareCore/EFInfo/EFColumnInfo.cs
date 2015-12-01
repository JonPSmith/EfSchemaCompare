#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EFColumnInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using CompareCore.Utils;

namespace CompareCore.EFInfo
{
    public class EfColumnInfo
    {
        private readonly PropertyInfo _clrProperty;

        public string SqlColumnName { get; private set; }

        public string SqlTypeName { get; private set; }

        public string ClrColumName { get { return _clrProperty.Name; } }

        public Type ClrColumnType { get { return _clrProperty.PropertyType; } }

        public bool IsPrimaryKey { get; private set; }

        public int PrimaryKeyOrder { get; private set; }

        public bool IsNullable { get; private set; }

        /// <summary>
        /// This holds the maxlength, or -1 if not set or invalid
        /// </summary>
        public int MaxLength { get; private set; }

        public EfColumnInfo(string sqlColumnName, string sqlTypeName, bool isNullable, int? maxLength, byte? precision, EfKeyOrder primaryKeyOrder, PropertyInfo clrProperty)
        {
            const string maxTypeEnding = "(max)";

            SqlColumnName = sqlColumnName;
            //types with a max length need 
            SqlTypeName = sqlTypeName.EndsWith(maxTypeEnding)
                ? sqlTypeName.Substring(0, sqlTypeName.Length - maxTypeEnding.Length)
                : sqlTypeName;
            if (primaryKeyOrder != null)
            {
                IsPrimaryKey = true;
                PrimaryKeyOrder = primaryKeyOrder.PrimaryKeyOrder;
            }
            IsNullable = isNullable;
            MaxLength = precision != null
                //if looks like presion is only set to non-null on decimal (and numeric?)
                ? ((byte)precision).GetLengthFromPrecision()
                : SqlTypeName.GetSqlMaxLengthFromEfMaxLength(maxLength);
            _clrProperty = clrProperty;
        }

        public override string ToString()
        {
            return string.Format("SqlColumnName: {0}, SqlTypeName: {1}, ClrColumName: {2}, ClrColumnType: {3}, IsPrimaryKey: {4}, PrimaryKeyOrder: {5}, IsNullable: {6}, MaxLength: {7}", 
                SqlColumnName, SqlTypeName, ClrColumName, ClrColumnType, IsPrimaryKey, PrimaryKeyOrder, IsNullable, MaxLength);
        }
    }
}