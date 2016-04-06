#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataIntDouble.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
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