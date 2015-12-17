#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataZeroOrOne.cs
// Date Created: 2015/11/19
// © Copyright Selective Analytics 2015. All rights reserved
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