#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataGuidEnum.cs
// Date Created: 2015/12/01
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;

namespace Tests.EfClasses.DataTypes
{
    public enum ByteEnum : byte { AllOk, Byte}
    public enum ShortEnum : short { AllOk, Short }
    public enum NormalEnum { AllOk, Int }
    public enum LongEnum : long { AllOk, Long }


    public class DataGuidEnum
    {
        public int DataGuidEnumId { get; set; }

        public Guid DataGuid { get; set; }

        public ByteEnum ByteEnum { get; set; }
        public ShortEnum ShortEnum { get; set; }
        public NormalEnum NormalEnum { get; set; }
        public LongEnum LongEnum { get; set; }

    }
}