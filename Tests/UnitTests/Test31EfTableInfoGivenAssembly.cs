#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test30EfTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Reflection;
using CompareCore;
using Ef6Compare.Internal;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test31EfTableInfoGivenAssembly
    {

        [Test]
        public void Test01GetEfDateDirect()
        {
            using (var db = new EfSchemaCompareDb())
            {
                //SETUP
                var efInfos = Ef6MetadataDecoder.GetAllEfTablesWithColInfo(db, Assembly.GetExecutingAssembly());

                //EXECUTE

                //VERIFY
                efInfos.Count.ShouldEqual(6);
            }
        }

        [Test]
        public void Test02GetEfDataViaDbContext()
        {
            using (var db = new EfSchemaCompareDb())
            {
                //SETUP
                var efInfos = Ef6MetadataDecoder.GetAllEfTablesWithColInfo(db, null);

                //EXECUTE

                //VERIFY
                efInfos.Count.ShouldEqual(6);
            }
        }

        [Test]
        public void Test40GetEfDataBad()
        {
            using (var db = new EfSchemaCompareDb())
            {
                //SETUP

                //EXECUTE
                var ex =
                    Assert.Throws<InvalidOperationException>(
                        () => Ef6MetadataDecoder.GetAllEfTablesWithColInfo(db, Assembly.GetAssembly(typeof (EfCompare))));

                //VERIFY
                ex.Message.ShouldStartWith("Could not find the EF data class");
            }
        }

    }
}