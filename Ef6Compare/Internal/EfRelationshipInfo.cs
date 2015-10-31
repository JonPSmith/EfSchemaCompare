#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfRelationshipInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;

namespace Ef6Compare.Internal
{

    internal class EfRelationshipInfo
    {
        private readonly NavigationProperty _navProperty;
        private readonly PropertyInfo _clrProperty;

        public string ClrColumnName { get { return _clrProperty.Name; } }

        public Type ClrColumnType { get { return _clrProperty.PropertyType; } }

        public string SqlColumnName { get; private set; }

        public RelationshipType RelationshipData { get { return _navProperty.RelationshipType; } }

        public FromToMultiplicity FromToMultiplicities
        {
            get { return new FromToMultiplicity ( _navProperty.FromEndMember.RelationshipMultiplicity, _navProperty.ToEndMember.RelationshipMultiplicity ); }
        }

        public EfRelationshipInfo(NavigationProperty navProperty, PropertyInfo clrProperty)
        {
            _navProperty = navProperty;
            _clrProperty = clrProperty;
            var columnArr = _clrProperty.GetCustomAttribute<ColumnAttribute>();
            SqlColumnName = columnArr == null ? _clrProperty.Name : columnArr.Name;
        }

        public override string ToString()
        {
            return string.Format("ClrColumnName: {0}, ClrColumnType: {1}, RelationshipData: {2}", ClrColumnName, ClrColumnType, RelationshipData);
        }
    }
}