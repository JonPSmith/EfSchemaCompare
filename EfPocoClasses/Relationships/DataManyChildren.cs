#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataManyChildren.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.Generic;

namespace EfPocoClasses.Relationships
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