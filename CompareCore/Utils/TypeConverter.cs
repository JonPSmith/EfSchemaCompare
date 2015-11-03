#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TypeConverter.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;

namespace CompareCore.Utils
{
    public static class TypeConverter
    {
        //with thanks to jared.g - see http://stackoverflow.com/questions/1058322/anybody-got-a-c-sharp-function-that-maps-the-sql-datatype-of-a-column-to-its-clr

        private class TypeLenMul
        {
            public Type ClrType { get; private set; }
            public int ClrMaxLenDiv { get; private set; }

            public TypeLenMul(Type clrType, int clrMaxLenDiv)
            {
                ClrType = clrType;
                ClrMaxLenDiv = clrMaxLenDiv;
            }
        }

        /// <summary>
        /// The dictionary contains:
        /// a) The CLR type for the sql type
        /// b) A number to modify the SQL MaxLength to match the CLR/EF MaxLength
        ///    a) 1 if they are the same
        ///    b) 2 if CLR/EF MaxLength is half the value
        /// </summary>
        private static readonly Dictionary<string, TypeLenMul> Mappings = new Dictionary<string, TypeLenMul>
        {
            {"bigint", new TypeLenMul( typeof (Int64), 1)},
            {"binary", new TypeLenMul( typeof (Byte[]), 1)},
            {"bit", new TypeLenMul( typeof (Boolean), 1)},
            {"char", new TypeLenMul( typeof (String), 1)},
            {"date", new TypeLenMul( typeof (DateTime), 1)},
            {"datetime", new TypeLenMul( typeof (DateTime), 1)},
            {"datetime2", new TypeLenMul( typeof (DateTime), 1)},
            {"datetimeoffset", new TypeLenMul( typeof (DateTimeOffset), 1)},
            {"decimal", new TypeLenMul( typeof (Decimal), 1)},
            {"float", new TypeLenMul( typeof (Double), 1)},
            {"image", new TypeLenMul( typeof (Byte[]), 1)},
            {"int", new TypeLenMul( typeof (Int32), 1)},
            {"money", new TypeLenMul( typeof (Decimal), 1)},
            {"nchar", new TypeLenMul( typeof (String), 2)},
            {"ntext", new TypeLenMul( typeof (String), 2)},
            {"numeric", new TypeLenMul( typeof (Decimal), 1)},
            {"nvarchar", new TypeLenMul( typeof (String), 2)},
            {"real", new TypeLenMul( typeof (Single), 1)},
            {"rowversion", new TypeLenMul( typeof (Byte[]), 1)},
            {"smalldatetime", new TypeLenMul( typeof (DateTime), 1)},
            {"smallint", new TypeLenMul( typeof (Int16), 1)},
            {"smallmoney", new TypeLenMul( typeof (Decimal), 1)},
            {"text", new TypeLenMul( typeof (String), 1)},
            {"time", new TypeLenMul( typeof (TimeSpan), 1)},
            {"timestamp", new TypeLenMul( typeof (Byte[]), 1)},
            {"tinyint", new TypeLenMul( typeof (Byte), 1)},
            {"uniqueidentifier", new TypeLenMul( typeof (Guid), 1)},
            {"varbinary", new TypeLenMul( typeof (Byte[]), 1)},
            {"varchar", new TypeLenMul( typeof (String), 1)}
        };
        
        public static Type SqlToClrType(this string sqlType, bool isNullable)
        {
            TypeLenMul dictValue = null;
            if (Mappings.TryGetValue(sqlType, out dictValue))
                return isNullable && dictValue.ClrType != typeof(string) ? typeof(Nullable<>).MakeGenericType(dictValue.ClrType) : dictValue.ClrType;
            throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));
        }

        public static int GetClrMaxLength(this string sqlType, int sqlMaxLength)
        {
            if (sqlMaxLength == -1) return sqlMaxLength;          //-1 means max length

            TypeLenMul dictValue = null;
            if (Mappings.TryGetValue(sqlType, out dictValue))
                return sqlMaxLength / dictValue.ClrMaxLenDiv;
            throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));
        }
    }
}