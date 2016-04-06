#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataComplex.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion
namespace EfPocoClasses.ComplexTypes
{
    public class DataComplex
    {
        public int DataComplexId { get; set; }

        public ComplexClass ComplexData { get; set; }

        /// <summary>
        /// This contains a nexted complex class
        /// </summary>
        public ComplexComplexClass ComplexComplexData { get; set; }
    }
}