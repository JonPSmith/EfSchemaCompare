#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: FromToMultiplicity.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

namespace CompareCore.EFInfo
{

    public enum EfRelationshipTypes { ZeroOrOne, One, Many }

    public class FromToMultiplicity
    {
        public EfRelationshipTypes FromMultiplicity { get; private set; }
        public EfRelationshipTypes ToMultiplicity { get; private set; }

        public FromToMultiplicity(EfRelationshipTypes fromMultiplicity, EfRelationshipTypes toMultiplicity)
        {
            FromMultiplicity = fromMultiplicity;
            ToMultiplicity = toMultiplicity;
        }

        public override string ToString()
        {
            return string.Format("{0}-to-{1}", FromMultiplicity, ToMultiplicity);
        }
    }
}