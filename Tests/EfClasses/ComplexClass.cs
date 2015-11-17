#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ComplexClass.cs
// Date Created: 2015/11/17
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations;

namespace Tests.EfClasses
{
    public class ComplexClass
    {
        public int ComplexInt { get; set; }

        [MaxLength(25)]
        public string ComplexString { get; set; }
    }
}