#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataManyChildren.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;

namespace Tests.EfClasses
{
    public class DataManyChildren
    {
        public int DataManyChildrenId { get; set; }

        public int MyInt { get; set; }

        //---------------------------------------------
        //relationships

        public ICollection<DataTop> ManyParents { get; set; }
    }
}