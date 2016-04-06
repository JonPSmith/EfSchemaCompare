#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test33EfTableInfoPublicPrivateClasses.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompareCore.EFInfo;
using Ef6Compare.InternalEf6;
using Ef6TestDbContext;
using EfPocoClasses.PublicPrivate;
using EfPocoClasses.Relationships;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test33EfTableInfoPublicPrivateClasses
    {
        private ICollection<EfTableInfo> _efInfos;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            using (var db = new TestEf6SchemaCompareDb())
            {
                var decoder = new Ef6MetadataDecoder(Assembly.GetAssembly(typeof(DataTop)));
                _efInfos = decoder.GetAllEfTablesWithColInfo(db);
            }
        }

        [Test]
        public void Test70DataPublicPrivateColsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataPublicPrivate));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual("DataPublicPrivate");
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(11);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataPublicPrivateId, SqlTypeName: int, ClrColumName: DataPublicPrivateId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: PublicInt, SqlTypeName: int, ClrColumName: PublicInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: PublicProtectedSetInt, SqlTypeName: int, ClrColumName: PublicProtectedSetInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: PublicPrivateSetInt, SqlTypeName: int, ClrColumName: PublicPrivateSetInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: InternalInt, SqlTypeName: int, ClrColumName: InternalInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: InternalPrivateSetInt, SqlTypeName: int, ClrColumName: InternalPrivateSetInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: ProtectedInt, SqlTypeName: int, ClrColumName: ProtectedInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: ProtectedPrivateSetInt, SqlTypeName: int, ClrColumName: ProtectedPrivateSetInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: ProtectedInternalInt, SqlTypeName: int, ClrColumName: ProtectedInternalInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: ProtectedInternalPrivateSetInt, SqlTypeName: int, ClrColumName: ProtectedInternalPrivateSetInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: PrivateInt, SqlTypeName: int, ClrColumName: PrivateInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test71DataPublicPrivateRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataPublicPrivate));

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.RelationshipCols.Count().ShouldEqual(0);
        }
    }
}