#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FromToMultiplicity.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Data.Entity.Core.Metadata.Edm;

namespace Ef6Compare.Internal
{

    internal class FromToMultiplicity
    {
        public RelationshipMultiplicity FromMultiplicity { get; private set; }
        public RelationshipMultiplicity ToMultiplicity { get; private set; }

        public FromToMultiplicity(RelationshipMultiplicity fromMultiplicity, RelationshipMultiplicity toMultiplicity)
        {
            FromMultiplicity = fromMultiplicity;
            ToMultiplicity = toMultiplicity;
        }

        public override string ToString()
        {
            return string.Format("{0} to {1}", FromMultiplicity, ToMultiplicity);
        }
    }
}