#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataChild.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.Relationships
{
    public class DataChild
    {
        public int DataChildId { get; set; }

        //This is set as an Index in the EF configuration
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