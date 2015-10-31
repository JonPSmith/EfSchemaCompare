#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataTop.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tests.EfClasses
{
    public class DataTop
    {

        public int DataTopId { get; set; }

        [MaxLength(25)]
        public string MyString { get; set; }

        public int LengthMyString { get { return MyString == null ? -1 : MyString.Length; } }

        //---------------------------------------------
        //relationships

        public DataSingleton SingletonNullable { get; set; }

        public ICollection<DataChild> Children { get; set; }

        public ICollection<DataManyChildren> ManyChildren { get; set; }
    }
}
