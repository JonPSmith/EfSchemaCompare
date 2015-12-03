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
using System.Runtime.CompilerServices;
using CompareCore.Utils;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.EFInfo
{
    public class EfColumnInfo
    {

        public string SqlColumnName { get; private set; }

        public string SqlTypeName { get; private set; }

        public string ClrColumName { get; private set; }

        public Type ClrColumnType { get; private set; }

        public bool IsPrimaryKey { get; private set; }

        public int PrimaryKeyOrder { get; private set; }

        public bool IsNullable { get; private set; }

        /// <summary>
        /// This holds the maxlength in the sql format
        /// </summary>
        public int MaxLength { get; private set; }

        //used by json
        [JsonConstructor]
        public EfColumnInfo(string sqlColumnName, string sqlTypeName, string clrColumName, Type clrColumnType, bool isPrimaryKey, int primaryKeyOrder, bool isNullable, int maxLength)
        {
            SqlColumnName = sqlColumnName;
            SqlTypeName = sqlTypeName;
            ClrColumName = clrColumName;
            ClrColumnType = clrColumnType;
            IsPrimaryKey = isPrimaryKey;
            PrimaryKeyOrder = primaryKeyOrder;
            IsNullable = isNullable;
            MaxLength = maxLength;
        }

        public EfColumnInfo(string sqlColumnName, string sqlTypeName, bool isNullable, int? maxLength, byte? precision, EfKeyOrder primaryKeyOrder, PropertyInfo clrProperty)
        {
            const string maxTypeEnding = "(max)";

            SqlColumnName = sqlColumnName;
            //types with a max length need 
            SqlTypeName = sqlTypeName.EndsWith(maxTypeEnding)
                ? sqlTypeName.Substring(0, sqlTypeName.Length - maxTypeEnding.Length)
                : sqlTypeName;
            ClrColumName = clrProperty.Name;
            ClrColumnType = clrProperty.PropertyType;
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
        }

        public override string ToString()
        {
            return string.Format("SqlColumnName: {0}, SqlTypeName: {1}, ClrColumName: {2}, ClrColumnType: {3}, IsPrimaryKey: {4}, PrimaryKeyOrder: {5}, IsNullable: {6}, MaxLength: {7}", 
                SqlColumnName, SqlTypeName, ClrColumName, ClrColumnType, IsPrimaryKey, PrimaryKeyOrder, IsNullable, MaxLength);
        }
    }
}