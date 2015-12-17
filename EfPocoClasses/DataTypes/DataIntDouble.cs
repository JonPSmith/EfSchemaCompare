#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataIntDouble.cs
// Date Created: 2015/12/01
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

namespace EfPocoClasses.DataTypes
{
    public class DataIntDouble
    {
        public int DataIntDoubleId { get; set; }


        public bool DataBool { get; set; }
        public byte DataByte { get; set; }
        public short DataShort { get; set; }
        public int DataInt { get; set; }
        public long DataLong { get; set; }

        public float DataFloat { get; set; }
        public double DataDouble { get; set; }
        public decimal DataDecimalSmallPrecision { get; set; }
        public decimal DataDecimalNormal { get; set; }

    }
}