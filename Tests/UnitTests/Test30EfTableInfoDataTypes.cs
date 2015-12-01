#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test30EfTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompareCore.EFInfo;
using Ef6Compare.Internal;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.EfClasses.ClassTypes;
using Tests.EfClasses.DataTypes;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test30EfTableInfoDataTypes
    {

        private ICollection<EfTableInfo> _efInfos;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            using (var db = new EfSchemaCompareDb())
            {
                var decoder = new Ef6MetadataDecoder(Assembly.GetAssembly(typeof(EfSchemaCompareDb)));
                _efInfos = decoder.GetAllEfTablesWithColInfo(db);
            }
        }

        [Test]
        public void Test01GetEfTableColumnInfo()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _efInfos.Count.ShouldEqual(12);
        }

        [Test]
        public void Test10DataIntDoubleOk()
        {
            //SETUP
            var classType = typeof (DataIntDouble);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            foreach (var col in efInfo.NormalCols)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            efInfo.NormalCols.Count.ShouldEqual(7);
            var list = efInfo.NormalCols.ToList();
            var i = 0;

        }

        [Test]
        public void Test20DataStringByteOk()
        {
            //SETUP
            var classType = typeof(DataStringByte);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            foreach (var col in efInfo.NormalCols)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            efInfo.NormalCols.Count.ShouldEqual(7);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
        }

        [Test]
        public void Test30DataDateOk()
        {
            //SETUP
            var classType = typeof(DataDate);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            foreach (var col in efInfo.NormalCols)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            efInfo.NormalCols.Count.ShouldEqual(7);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
        }

        [Test]
        public void Test40DataGuidEnumOk()
        {
            //SETUP
            var classType = typeof(DataGuidEnum);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            foreach (var col in efInfo.NormalCols)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            efInfo.NormalCols.Count.ShouldEqual(7);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
        }


    }
}