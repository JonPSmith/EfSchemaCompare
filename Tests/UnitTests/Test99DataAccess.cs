#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test99DataAccess.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.ObjectModel;
using Ef6TestDbContext;
using EfPocoClasses.Relationships;
using NUnit.Framework;

namespace Tests.UnitTests
{
    public class Test99DataAccess
    {
        [Test]
        [Ignore]
        public void Test01GetEfTableColumnInfo()
        {
            using (var db = new TestEf6SchemaCompareDb("DbUpSchemaCompareDb"))
            {
                //SETUP

                //EXECUTE
                var data = new DataTop
                {
                    MyString = "Hello",
                    ManyChildren = new Collection<DataManyChildren>
                    {
                        new DataManyChildren
                        {
                            MyInt = 1
                        }
                    }
                };
                db.DataTops.Add(data);
                db.SaveChanges();

                //VERIFY
            }

        }
    }
}