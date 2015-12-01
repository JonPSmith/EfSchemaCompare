#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataCompKey.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.EfClasses.Relationships
{
    public enum EnumTests { One, Two, Three}

    [Table("NonStandardCompKeyTable")]
    public class DataCompKey
    {
        [Key]
        [Column(Order = 1)]
        public int Key1 { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid Key2 { get; set; }

        [Column("NonStandardColumnName")]
        public EnumTests MyEnum { get; set; }

    }
}
