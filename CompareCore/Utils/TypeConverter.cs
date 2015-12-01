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
            /// <summary>
            /// This is the type that the sql type maps to
            /// </summary>
            public Type ClrType { get; private set; }

            /// <summary>
            /// If EF has a length of null then this is the default length
            /// </summary>
            public int DefaultLength { get; private set; }

            /// <summary>
            /// If true then the EF length needs to be multipled by 2 to get the sql length
            /// </summary>
            public bool MultiplyBy2 { get; private set; }

            public TypeLenMul(Type clrType, int defaultLength, bool multiplyBy2)
            {
                ClrType = clrType;
                DefaultLength = defaultLength;
                MultiplyBy2 = multiplyBy2;
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
            {"bigint", new TypeLenMul( typeof (Int64), 8, false)},
            {"binary", new TypeLenMul( typeof (Byte[]), 0, false)},
            {"bit", new TypeLenMul( typeof (Boolean), 1, false)},
            {"char", new TypeLenMul( typeof (String), 1, false)},
            {"date", new TypeLenMul( typeof (DateTime), 3, false)},
            {"datetime", new TypeLenMul( typeof (DateTime), 8, false)},
            {"datetime2", new TypeLenMul( typeof (DateTime), 8, false)},
            {"datetimeoffset", new TypeLenMul( typeof (DateTimeOffset), 10, false)},
            {"decimal", new TypeLenMul( typeof (Decimal), 9, false)},
            {"float", new TypeLenMul( typeof (Double), 8, false)},
            //{"image", new TypeLenMul( typeof (Byte[]), 1, false)},
            {"int", new TypeLenMul( typeof (Int32), 4, false)},
            {"money", new TypeLenMul( typeof (Decimal), 8, false)},
            {"nchar", new TypeLenMul( typeof (String), 2, true)},
            //{"ntext", new TypeLenMul( typeof (String), 1, true)},
            {"numeric", new TypeLenMul( typeof (Decimal), 9, false)},
            {"nvarchar", new TypeLenMul( typeof (String), -1, true)},
            {"real", new TypeLenMul( typeof (Single), 4, false)},
            {"rowversion", new TypeLenMul( typeof (Byte[]), 8, false)},
            {"smalldatetime", new TypeLenMul( typeof (DateTime), 4, false)},
            {"smallint", new TypeLenMul( typeof (Int16), 2, false)},
            {"smallmoney", new TypeLenMul( typeof (Decimal), 4, false)},
            //{"text", new TypeLenMul( typeof (String), 1, false)},
            {"time", new TypeLenMul( typeof (TimeSpan), 5, false)},
            {"timestamp", new TypeLenMul( typeof (Byte[]), 1, false)},
            {"tinyint", new TypeLenMul( typeof (Byte), 1, false)},
            {"uniqueidentifier", new TypeLenMul( typeof (Guid), 16, false)},
            {"varbinary", new TypeLenMul( typeof (Byte[]), 8000, false)},
            {"varchar", new TypeLenMul( typeof (String), 8000, false)}
        };
        
        public static Type SqlToClrType(this string sqlType, bool isNullable)
        {
            TypeLenMul dictValue = null;
            if (Mappings.TryGetValue(sqlType, out dictValue))
                return isNullable && dictValue.ClrType != typeof(string) ? typeof(Nullable<>).MakeGenericType(dictValue.ClrType) : dictValue.ClrType;
            throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));
        }

        /// <summary>
        /// You need to work out the MaxLength of a decimal using the precision
        /// See https://msdn.microsoft.com/en-GB/library/ms187746.aspx
        /// </summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static int GetLengthFromPrecision(this byte precision)
        {
            if (precision <= 9) return 5;
            if (precision <= 19) return 9;
            if (precision <= 28) return 13;
            if (precision <= 38) return 17;

            throw new InvalidOperationException("The decimal precision should not exceed 38.");
        }

        /// <summary>
        /// Clr uses a MaxLength for unicode data, i.e. SQL nvarchar, nchar, ntext, which is half what is should be
        /// </summary>
        /// <param name="sqlType"></param>
        /// <param name="efMaxLength"></param>
        /// <returns></returns>
        public static int GetSqlMaxLengthFromEfMaxLength(this string sqlType, int? efMaxLength)
        {
            TypeLenMul dictValue = null;
            if (!Mappings.TryGetValue(sqlType, out dictValue))
                throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));

            if (efMaxLength != null)
                //it has a MaxLength set
                return (int) (dictValue.MultiplyBy2 ? efMaxLength*2 : efMaxLength);

            //otherwise we return the default length that EF applies for this sql type 
            return dictValue.DefaultLength;
        }

        public static bool EfLengthIdHalfThis(this string sqlType)
        {
            TypeLenMul dictValue = null;
            if (!Mappings.TryGetValue(sqlType, out dictValue))
                throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));

            return dictValue.MultiplyBy2;
        }
    }
}