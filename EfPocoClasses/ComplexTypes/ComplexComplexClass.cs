#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ComplexComplexClass.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.ComplexTypes
{
    /// <summary>
    /// Nested complex class
    /// </summary>
    [ComplexType]
    public class ComplexComplexClass
    {
        public DateTime ComplexDateTime { get; set; }

        public Guid ComplexGuid { get; set; }

        public ComplexClass ComplexData { get; set; }
    }
}