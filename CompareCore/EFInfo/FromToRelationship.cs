#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FromToRelationship.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.EFInfo
{

    public enum EfRelationshipTypes { ZeroOrOne, One, Many }

    public class FromToRelationship
    {
        public FromToRelationship(
            EfRelationshipTypes fromMultiplicity,
            bool fromIsCascadeDelete,
            EfRelationshipTypes toMultiplicity,
            bool toIsCascadeDelete,
            IEnumerable<string> fromColumns,
            IEnumerable<string> toColumns)
        {
            FromMultiplicity = fromMultiplicity;
            FromIsCascadeDelete = fromIsCascadeDelete;
            ToMultiplicity = toMultiplicity;
            ToIsCascadeDelete = toIsCascadeDelete;
            FromColumns = fromColumns;
            ToColumns = toColumns;
        }

        public EfRelationshipTypes FromMultiplicity { get; private set; }
        public bool FromIsCascadeDelete { get; private set; }
        public EfRelationshipTypes ToMultiplicity { get; private set; }
        public bool ToIsCascadeDelete { get; private set; }
        public IEnumerable<string> FromColumns { get; }
        public IEnumerable<string> ToColumns { get; }

        public override string ToString()
        {
            return string.Format("{0}-to-{1}", FromMultiplicity, ToMultiplicity);
        }
    }
}