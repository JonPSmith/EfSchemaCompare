#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataCompKey.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.Relationships
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
