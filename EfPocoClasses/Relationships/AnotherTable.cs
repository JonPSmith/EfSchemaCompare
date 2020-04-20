// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: AnotherTable.cs
// Date Created: //
// © Copyright Selective Analytics 2020. All rights reserved
// =====================================================

using System.Collections.Generic;

namespace EfPocoClasses.Relationships
{
    public class AnotherTable
    {
        public int AnotherTableId { get; set; }
        public string MyString { get; set; }
        public List<DataTop> DataTops { get; set; }
    }
}