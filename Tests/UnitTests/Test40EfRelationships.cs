#region licence
// =====================================================
// EfSchemeCompare Project - project-to-compare EF schema-to-SQL schema
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
using Tests.EfClasses.Relationships;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test40EfRelationships
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


        private Type GetClassFromCollection(EfRelationshipInfo relEfCol)
        {
            if (!relEfCol.ClrColumnType.IsGenericType)
                throw new InvalidOperationException("I expected a generic list etc. here");
            var genArgs = relEfCol.ClrColumnType.GetGenericArguments();
            if (genArgs.Length != 1)
                throw new InvalidOperationException("I expect only one class");

            return genArgs[0];
        }

        [Test]
        public void Test01GetEfTableColumnInfo()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _efInfos.Count.ShouldEqual(12);
            //foreach (var relCol in _efInfos.SelectMany(x => x.RelationshipCols))
            //{
            //    Console.WriteLine(relCol);
            //}
        }

        [Test]
        public void Test10DataTopOk()
        {
            //SETUP
            var classType = typeof(DataTop);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.RelationshipCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.RelationshipCols.Count.ShouldEqual(6);
            var list = efInfo.RelationshipCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ClrColumnName: Children, ClrColumnType: System.Collections.Generic.ICollection`1[Tests.EfClasses.Relationships.DataChild], FromToRelationships: One-to-Many");
            list[i++].ToString().ShouldEqual("ClrColumnName: CompositeKeyData, ClrColumnType: Tests.EfClasses.Relationships.DataCompKey, FromToRelationships: Many-to-One");
            list[i++].ToString().ShouldEqual("ClrColumnName: ManyChildren, ClrColumnType: System.Collections.Generic.ICollection`1[Tests.EfClasses.Relationships.DataManyChildren], FromToRelationships: Many-to-Many");
            list[i++].ToString().ShouldEqual("ClrColumnName: ManyCompKeys, ClrColumnType: System.Collections.Generic.ICollection`1[Tests.EfClasses.Relationships.DataManyCompKey], FromToRelationships: Many-to-Many");
            list[i++].ToString().ShouldEqual("ClrColumnName: SingletonNullable, ClrColumnType: Tests.EfClasses.Relationships.DataSingleton, FromToRelationships: One-to-ZeroOrOne");
            list[i++].ToString().ShouldEqual("ClrColumnName: ZeroOrOneData, ClrColumnType: Tests.EfClasses.Relationships.DataZeroOrOne, FromToRelationships: One-to-ZeroOrOne");

        }


        [Test]
        public void Test20DataChildOk()
        {
            //SETUP
            var classType = typeof(DataChild);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.RelationshipCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.RelationshipCols.Count.ShouldEqual(1);
            var list = efInfo.RelationshipCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ClrColumnName: Parent, ClrColumnType: Tests.EfClasses.Relationships.DataTop, FromToRelationships: Many-to-One");
        }

        [Test]
        public void Test20DataManyChildrenOk()
        {
            //SETUP
            var classType = typeof(DataManyChildren);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.RelationshipCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.RelationshipCols.Count.ShouldEqual(1);
            var list = efInfo.RelationshipCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ClrColumnName: ManyParents, ClrColumnType: System.Collections.Generic.ICollection`1[Tests.EfClasses.Relationships.DataTop], FromToRelationships: Many-to-Many");
        }


        [Test]
        public void Test30DataSingletonParentOk()
        {
            //SETUP
            var classType = typeof(DataSingleton);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.RelationshipCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.RelationshipCols.Count.ShouldEqual(1);
            var list = efInfo.RelationshipCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ClrColumnName: Parent, ClrColumnType: Tests.EfClasses.Relationships.DataTop, FromToRelationships: ZeroOrOne-to-One");
        }

        [Test]
        public void Test50DataCompKeyOk()
        {
            //SETUP
            var classType = typeof(DataCompKey);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual("NonStandardCompKeyTable");
            //foreach (var col in efInfo.RelationshipCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.RelationshipCols.Count.ShouldEqual(0);
        }

        [Test]
        public void Test60DataManyCompKeyOk()
        {
            //SETUP
            var classType = typeof(DataManyCompKey);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.RelationshipCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.RelationshipCols.Count.ShouldEqual(1);
            var list = efInfo.RelationshipCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ClrColumnName: ManyParents, ClrColumnType: System.Collections.Generic.ICollection`1[Tests.EfClasses.Relationships.DataTop], FromToRelationships: Many-to-Many");
        }


        [Test]
        public void Test70DataZeroOrOneRelationshipsOk()
        {
            //SETUP
            var classType = typeof(DataZeroOrOne);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            foreach (var col in efInfo.RelationshipCols)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            efInfo.RelationshipCols.Count.ShouldEqual(0);
            var list = efInfo.RelationshipCols.ToList();
            var i = 0;
        }

    }
}