#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfKeyOrder.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion
namespace CompareCore.EFInfo
{
    public class EfKeyOrder
    {
        public EfKeyOrder(string name, int primaryKeyOrder)
        {
            Name = name;
            PrimaryKeyOrder = primaryKeyOrder;
        }

        public string Name { get; private set; }

        public int PrimaryKeyOrder { get; private set; }
    }
}