#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataZeroOrOne.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations;

namespace EfPocoClasses.Relationships
{
    public class DataZeroOrOne
    {
        [Key]
        public int DataTopId { get; set; }

        public bool MyBool { get; set; }
    }
}