#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataComplex.cs
// Date Created: 2015/11/17
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

namespace EfPocoClasses.ClassTypes
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