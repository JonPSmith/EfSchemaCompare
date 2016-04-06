#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataSingleton.cs
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
    /// <summary>
    /// Thsi can only take part in a one to one or ZeroOrOne to One relationship
    /// </summary>
    public class DataSingleton
    {
        public DataSingleton()
        {
            MyDateTime = DateTime.Now;
        }

        [Key]
        public int DataTopId { get; set; }

        public DateTime MyDateTime { get; set; }

        //---------------------------------------------------------
        //relationships

        [ForeignKey("DataTopId")]
        [Column("NonStandardParent")]
        public DataTop Parent { get; set; }
    }
}
