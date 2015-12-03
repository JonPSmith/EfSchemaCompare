#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FromToRelationship.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.EFInfo
{

    public enum EfRelationshipTypes { ZeroOrOne, One, Many }

    public class FromToRelationship
    {
        public EfRelationshipTypes FromMultiplicity { get; private set; }
        public bool FromIsCascadeDelete { get; private set; }
        public EfRelationshipTypes ToMultiplicity { get; private set; }
        public bool ToIsCascadeDelete { get; private set; }

        internal FromToRelationship()
        {
        }

        public FromToRelationship(EfRelationshipTypes fromMultiplicity, bool fromIsCascadeDelete, EfRelationshipTypes toMultiplicity, bool toIsCascadeDelete)
        {
            FromMultiplicity = fromMultiplicity;
            FromIsCascadeDelete = fromIsCascadeDelete;
            ToMultiplicity = toMultiplicity;
            ToIsCascadeDelete = toIsCascadeDelete;
        }

        public override string ToString()
        {
            return string.Format("{0}-to-{1}", FromMultiplicity, ToMultiplicity);
        }
    }
}