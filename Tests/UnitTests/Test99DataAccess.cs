#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test99DataAccess.cs
// Date Created: 2015/11/04
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.ObjectModel;
using NUnit.Framework;
using Tests.EfClasses;

namespace Tests.UnitTests
{
    public class Test99DataAccess
    {
        [Test]
        [Ignore]
        public void Test01GetEfTableColumnInfo()
        {
            using (var db = new EfSchemaCompareDb("DbUpSchemaCompareDb"))
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