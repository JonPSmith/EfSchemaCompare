#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfKeyOrder.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion
namespace CompareCore.EFInfo
{
    public class EfKeyOrder
    {
        public string Name { get; set; }

        public int PrimaryKeyOrder { get; set; }

        public EfKeyOrder(string name, int primaryKeyOrder)
        {
            Name = name;
            PrimaryKeyOrder = primaryKeyOrder;
        }
    }
}