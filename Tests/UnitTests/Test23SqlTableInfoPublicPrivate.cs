#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test23SqlTableInfoPublicPrivate.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test23SqlTableInfoPublicPrivate
    {
        private ICollection<SqlTableInfo> _sqlInfos;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var connection = ConfigurationManager.ConnectionStrings[MiscConstants.GetEfDatabaseConfigName()].ConnectionString;
            var allSqlInfo = SqlAllInfo.SqlAllInfoFactory(connection);
            _sqlInfos = allSqlInfo.TableInfos;
        }

        [Test]
        public void Test01DataPublicPrivateColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataPublicPrivate");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            //foreach (var col in sqlInfo.ColumnInfos)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            sqlInfo.ColumnInfos.Count.ShouldEqual(11);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataPublicPrivateId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: PublicInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: PublicProtectedSetInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: PublicPrivateSetInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: InternalInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: InternalPrivateSetInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ProtectedInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ProtectedPrivateSetInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ProtectedInternalInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ProtectedInternalPrivateSetInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: PrivateInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            }
    }
    }