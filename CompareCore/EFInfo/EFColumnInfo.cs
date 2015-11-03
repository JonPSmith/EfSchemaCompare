#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EFColumnInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Reflection;

namespace CompareCore.EFInfo
{
    public class EfColumnInfo
    {
        private readonly PropertyInfo _clrProperty;

        public string SqlColumnName { get; private set; }

        public string ClrColumName { get { return _clrProperty.Name; } }

        public Type ClrColumnType { get { return _clrProperty.PropertyType; } }

        public bool IsPrimaryKey { get; private set; }

        public int PrimaryKeyOrder { get; private set; }

        public bool IsNullable { get; private set; }

        /// <summary>
        /// This holds the maxlength, or -1 if not set or invalid
        /// </summary>
        public int MaxLength { get; private set; }

        public EfColumnInfo(string sqlColumnName, bool isNullable, int? maxLength, EfKeyOrder primaryKeyOrder, PropertyInfo clrProperty)
        {
            SqlColumnName = sqlColumnName;
            if (primaryKeyOrder != null)
            {
                IsPrimaryKey = true;
                PrimaryKeyOrder = primaryKeyOrder.PrimaryKeyOrder;
            }
            IsNullable = isNullable;
            MaxLength = maxLength ?? -2;        //-2 means don't check it
            _clrProperty = clrProperty;
        }

        public override string ToString()
        {
            return string.Format("SqlColumnName: {0}, ClrColumName: {1}, ClrColumnType: {2}, IsPrimaryKey: {3}, IsNullable: {4}, MaxLength: {5}",
                SqlColumnName, ClrColumName, ClrColumnType, IsPrimaryKey, IsNullable, MaxLength);
        }
    }
}