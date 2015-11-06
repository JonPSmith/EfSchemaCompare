#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfRelationshipInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Reflection;

namespace CompareCore.EFInfo
{

    public class EfRelationshipInfo
    {
        private readonly PropertyInfo _clrProperty;

        public string ClrColumnName { get { return _clrProperty.Name; } }

        public Type ClrColumnType { get { return _clrProperty.PropertyType; } }

        public string SqlColumnName { get; private set; }

        public FromToRelationship FromToRelationships { get; private set; }

        public EfRelationshipInfo(FromToRelationship efRelationshipTypes, PropertyInfo clrProperty, string sqlColumnName)
        {
            FromToRelationships = efRelationshipTypes;
            _clrProperty = clrProperty;
            SqlColumnName = sqlColumnName;
        }

        public override string ToString()
        {
            return string.Format("ClrColumnName: {0}, ClrColumnType: {1}, FromToRelationships: {2}", ClrColumnName, ClrColumnType, FromToRelationships);
        }
    }
}