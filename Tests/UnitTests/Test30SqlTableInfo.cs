#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test30SqlTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
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
    public class Test30SqlTableInfo
    {

        private ICollection<SqlTableInfo> _sqlInfos;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var connection = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;
            _sqlInfos = SqlTableInfo.GetAllSqlTablesWithColInfo(connection);
        }

        [Test]
        public void Test01GetEfTableColumnInfo()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _sqlInfos.Count.ShouldEqualWithTolerance(8,1);      //we allow for the __MirgartionHistory
        }

        [Test]
        public void Test10DataTopNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataTop");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list.Count.ShouldEqual(3);
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyString, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataSingletonId, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: True, MaxLength: 4");
        }

        [Test]
        public void Test11DataTopForeignKeysOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataTop");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ForeignKeys.Count.ShouldEqual(0);
        }

        [Test]
        public void Test20DataChildNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataChild");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(3);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataChildId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test21DataChildRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataChild");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ForeignKeys.Count.ShouldEqual(1);
            sqlInfo.ForeignKeys.First().ToString().ShouldEqual("Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId");
        }

        [Test]
        public void Test30DataManyChildrenNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataManyChildren");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(2);
            sqlInfo.ColumnInfo.First().ToString().ShouldEqual("ColumnName: DataManyChildrenId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            sqlInfo.ColumnInfo.Last().ToString().ShouldEqual("ColumnName: MyInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test31DataManyChildrenRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataManyChildren");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ForeignKeys.Count.ShouldEqual(0);
        }



        [Test]
        public void Test40DataSingletonNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataSingleton");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(3);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataSingletonId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyDateTime, ColumnSqlType: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: True, MaxLength: 4");
        }

        [Test]
        public void Test41DataSingletonRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataManyChildren");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ForeignKeys.Count.ShouldEqual(0);
        }
    }
}