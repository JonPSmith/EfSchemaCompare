#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataSingleton.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.EfClasses.Relationships
{
    /// <summary>
    /// Thsi can only take part in a one to one or ZeroOrOne to One relationship
    /// </summary>
    public class DataSingleton
    {
        [Key]
        public int DataTopId { get; set; }

        public DateTime MyDateTime { get; set; }

        //---------------------------------------------------------
        //relationships

        [ForeignKey("DataTopId")]
        [Column("NonStandardParent")]
        public DataTop Parent { get; set; }

        public DataSingleton()
        {
            MyDateTime = DateTime.Now;
        }
    }
}
