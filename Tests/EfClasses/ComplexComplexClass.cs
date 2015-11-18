#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ComplexClass.cs
// Date Created: 2015/11/17
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.EfClasses
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