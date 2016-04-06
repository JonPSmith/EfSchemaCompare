#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfRelationshipInfo.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;

namespace CompareCore.EFInfo
{

    public class EfRelationshipInfo
    {
        public EfRelationshipInfo(FromToRelationship efRelationshipTypes, string clrColumnName, Type clrColumnType)
        {
            ClrColumnName = clrColumnName;
            ClrColumnType = clrColumnType;
            FromToRelationships = efRelationshipTypes;
        }

        public string ClrColumnName { get; private set; }

        public Type ClrColumnType { get; private set; }

        public FromToRelationship FromToRelationships { get; private set; }

        public override string ToString()
        {
            return string.Format("ClrColumnName: {0}, ClrColumnType: {1}, FromToRelationships: {2}", ClrColumnName, ClrColumnType, FromToRelationships);
        }
    }
}