#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfRelationshipInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;

namespace CompareCore.EFInfo
{

    public class EfRelationshipInfo
    {
        public string ClrColumnName { get; private set; }

        public Type ClrColumnType { get; private set; }

        public FromToRelationship FromToRelationships { get; private set; }

        public EfRelationshipInfo(FromToRelationship efRelationshipTypes, string clrColumnName, Type clrColumnType)
        {
            ClrColumnName = clrColumnName;
            ClrColumnType = clrColumnType;
            FromToRelationships = efRelationshipTypes;
        }

        public override string ToString()
        {
            return string.Format("ClrColumnName: {0}, ClrColumnType: {1}, FromToRelationships: {2}", ClrColumnName, ClrColumnType, FromToRelationships);
        }
    }
}