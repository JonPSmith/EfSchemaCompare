#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataSingleton.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;

namespace Tests.EfClasses
{
    /// <summary>
    /// Thsi can only take part in a one to one or ZeroOrOne to One relationship
    /// </summary>
    public class DataSingleton
    {
        public int DataSingletonId { get; set; }

        public DateTime MyDateTime { get; set; }

        public DataSingleton()
        {
            MyDateTime = DateTime.Now;
        }
    }
}
