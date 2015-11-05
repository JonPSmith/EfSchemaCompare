#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataChild.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.EfClasses
{
    public class DataChild
    {
        public int DataChildId { get; set; }

        public int MyInt { get; set; }

        public string MyString { get; set; }

        //public int EfOnlyProperty { get; set; }

        //---------------------------------------------
        //relationships

        public int DataTopId { get; set; }

        [ForeignKey("DataTopId")]
        public DataTop Parent { get; set; }
    }
}