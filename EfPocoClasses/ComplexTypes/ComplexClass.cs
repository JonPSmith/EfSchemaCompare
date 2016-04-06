#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ComplexClass.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.ComplexTypes
{
    [ComplexType]
    public class ComplexClass
    {
        public int ComplexInt { get; set; }

        [MaxLength(25)]
        public string ComplexString { get; set; }
    }
}