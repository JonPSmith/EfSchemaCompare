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
using Tests.EfClasses.Relationships;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test34EfTableInfoRelationships
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
        public void Test10DataTopNormalColsOk()
        {
            //SETUP
            var classType = typeof (DataTop);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(5);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataTopId, SqlTypeName: int, ClrColumName: DataTopId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: MyString, SqlTypeName: varchar, ClrColumName: MyString, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataSingletonId, SqlTypeName: int, ClrColumName: DataSingletonId, ClrColumnType: System.Nullable`1[System.Int32], IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: Key1, SqlTypeName: int, ClrColumName: Key1, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: Key2, SqlTypeName: uniqueidentifier, ClrColumName: Key2, ClrColumnType: System.Guid, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 16");
        }

        [Test]
        public void Test11DataTopRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            CollectionAssert.AreEquivalent(new[] { "Children", "CompositeKeyData", "ManyChildren", "ManyCompKeys", "SingletonNullable", "ZeroOrOneData" }, efInfo.RelationshipCols.Select(x => x.ClrColumnName));
            CollectionAssert.AreEquivalent(new[] { typeof(ICollection<DataChild>), typeof(DataCompKey), typeof(ICollection<DataManyChildren>), typeof(ICollection<DataManyCompKey>), typeof(DataSingleton), typeof(DataZeroOrOne) }, 
                efInfo.RelationshipCols.Select(x => x.ClrColumnType));
        }

        [Test]
        public void Test20DataChildNormalColsOk()
        {
            //SETUP
            var classType = typeof(DataChild);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(4);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataChildId, SqlTypeName: int, ClrColumName: DataChildId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: MyInt, SqlTypeName: int, ClrColumName: MyInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: MyString, SqlTypeName: nvarchar, ClrColumName: MyString, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: -1");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataTopId, SqlTypeName: int, ClrColumName: DataTopId, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test21DataChildRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataChild));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            CollectionAssert.AreEquivalent(new[] { "Parent" }, efInfo.RelationshipCols.Select(x => x.ClrColumnName));
            CollectionAssert.AreEquivalent(new[] { typeof(DataTop) }, efInfo.RelationshipCols.Select(x => x.ClrColumnType));
        }


        [Test]
        public void Test30DataManyChildrenNormalColsOk()
        {
            //SETUP
            var classType = typeof(DataManyChildren);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(2);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataManyChildrenId, SqlTypeName: int, ClrColumName: DataManyChildrenId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: MyInt, SqlTypeName: int, ClrColumName: MyInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test31DataManyChildrenRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataManyChildren));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            CollectionAssert.AreEquivalent(new[] { "ManyParents" }, efInfo.RelationshipCols.Select(x => x.ClrColumnName));
            CollectionAssert.AreEquivalent(new[] { typeof(ICollection<DataTop>) }, efInfo.RelationshipCols.Select(x => x.ClrColumnType));
        }

        [Test]
        public void Test40DataSingletonNormalColsOk()
        {
            //SETUP
            var classType = typeof(DataSingleton);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(3);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataSingletonId, SqlTypeName: int, ClrColumName: DataSingletonId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: MyDateTime, SqlTypeName: datetime, ClrColumName: MyDateTime, ClrColumnType: System.DateTime, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("SqlColumnName: NonStandardForeignKeyName, SqlTypeName: int, ClrColumName: NonStandardForeignKeyName, ClrColumnType: System.Nullable`1[System.Int32], IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 4");
        }

        [Test]
        public void Test41DataSingletonRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataSingleton));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            CollectionAssert.AreEquivalent(new[] { "Parent" }, efInfo.RelationshipCols.Select(x => x.ClrColumnName));
            CollectionAssert.AreEquivalent(new[] { typeof(DataTop) }, efInfo.RelationshipCols.Select(x => x.ClrColumnType));
        }

        [Test]
        public void Test50DataCompKeyNormalColsOk()
        {
            //SETUP
            var classType = typeof(DataCompKey);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual("NonStandardCompKeyTable");
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(3);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: Key1, SqlTypeName: int, ClrColumName: Key1, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: Key2, SqlTypeName: uniqueidentifier, ClrColumName: Key2, ClrColumnType: System.Guid, IsPrimaryKey: True, PrimaryKeyOrder: 2, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("SqlColumnName: NonStandardColumnName, SqlTypeName: int, ClrColumName: MyEnum, ClrColumnType: Tests.EfClasses.Relationships.EnumTests, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test51DataCompKeyRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataCompKey));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.RelationshipCols.Count().ShouldEqual(0);
        }

        [Test]
        public void Test60DataManyCompKeyNormalColsOk()
        {
            //SETUP
            var classType = typeof(DataManyCompKey);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(2);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: ManyKey1, SqlTypeName: int, ClrColumName: ManyKey1, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: ManyKey2, SqlTypeName: uniqueidentifier, ClrColumName: ManyKey2, ClrColumnType: System.Guid, IsPrimaryKey: True, PrimaryKeyOrder: 2, IsNullable: False, MaxLength: 16");
        }

        [Test]
        public void Test61DataCompKeyRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataManyCompKey));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.RelationshipCols.Count().ShouldEqual(1);
            CollectionAssert.AreEquivalent(new[] { "ManyParents" }, efInfo.RelationshipCols.Select(x => x.ClrColumnName));
            CollectionAssert.AreEquivalent(new[] { typeof(ICollection<DataTop>) }, efInfo.RelationshipCols.Select(x => x.ClrColumnType));
        }

        [Test]
        public void Test80DataZeroOrOneColsOk()
        {
            //SETUP
            //SETUP
            var classType = typeof(DataManyCompKey);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(2);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: ManyKey1, SqlTypeName: int, ClrColumName: ManyKey1, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: ManyKey2, SqlTypeName: uniqueidentifier, ClrColumName: ManyKey2, ClrColumnType: System.Guid, IsPrimaryKey: True, PrimaryKeyOrder: 2, IsNullable: False, MaxLength: 16");
            
        }

        [Test]
        public void Test81DataZeroOrOneRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataZeroOrOne));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.RelationshipCols.Count().ShouldEqual(0);
        }
    }
}