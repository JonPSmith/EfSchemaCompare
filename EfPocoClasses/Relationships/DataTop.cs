#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataTop.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.Relationships
{
    public class DataTop
    {

        public int DataTopId { get; set; }

        [MaxLength(25)]
        [Column(TypeName = "varchar")]
        public string MyString { get; set; }

        public int LengthMyString { get { return MyString == null ? -1 : MyString.Length; } }

        //---------------------------------------------
        //relationships

        public DataSingleton SingletonNullable { get; set; }

        [ForeignKey("CompositeKeyData")]
        [Column(Order = 1)]
        public int Key1 { get; set; }

        [ForeignKey("CompositeKeyData")]
        [Column(Order = 2)]
        public Guid Key2 { get; set; }

        public DataCompKey CompositeKeyData { get; set; }

        public DataZeroOrOne ZeroOrOneData { get; set; }

        public ICollection<DataChild> Children { get; set; }

        public ICollection<DataManyChildren> ManyChildren { get; set; }

        public ICollection<DataManyCompKey> ManyCompKeys { get; set; } 
    }
}
