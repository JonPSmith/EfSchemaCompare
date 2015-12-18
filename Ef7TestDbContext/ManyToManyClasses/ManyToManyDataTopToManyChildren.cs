// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataTopToManyChildren.cs
// Date Created: 2015/12/18
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================

using EfPocoClasses.Relationships;

namespace Ef7TestDbContext.ManyToManyClasses
{
    public class DataTopToManyChildren
    {
        public int DataTopId { get; set; }
        public DataTop DataTop { get; set; }

        public int DataManyChildrenId { get; set; }
        public DataManyChildren DataManyChildren { get; set; }
    }
}