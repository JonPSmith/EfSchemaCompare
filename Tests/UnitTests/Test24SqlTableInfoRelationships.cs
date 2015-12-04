#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test25SqlTableInfoRelationships.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test24SqlTableInfoRelationships
    {

        private IList<SqlTableInfo> _sqlInfos;
        private IList<SqlForeignKey> _sqlForeignKeys;
                   
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var connection = ConfigurationManager.ConnectionStrings[MiscConstants.EfDatabaseConfigName].ConnectionString;
            var allSqlInfo = SqlAllInfo.SqlAllInfoFactory(connection);
            _sqlInfos = allSqlInfo.TableInfos;
            _sqlForeignKeys = allSqlInfo.ForeignKeys;
        }

        [Test]
        public void Test01GetEfTableColumnInfo()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _sqlInfos.Count.ShouldEqualWithTolerance(15,1);      //we allow for the __MirgartionHistory
        }

        [Test]
        public void Test10DataTopNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataTop");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list.Count.ShouldEqual(4);
            list[i++].ToString().ShouldEqual("ColumnName: Key1, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: Key2, SqlTypeName: uniqueidentifier, IsPrimaryKey: False, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyString, SqlTypeName: varchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
        }

        [Test]
        public void Test20DataChildNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataChild");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(4);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataChildId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyString, SqlTypeName: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: -1");
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test30DataManyChildrenNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataManyChildren");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(2);
            sqlInfo.ColumnInfos.First().ToString().ShouldEqual("ColumnName: DataManyChildrenId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            sqlInfo.ColumnInfos.Last().ToString().ShouldEqual("ColumnName: MyInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test40DataSingletonNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataSingleton");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(2);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyDateTime, SqlTypeName: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
        }

        [Test]
        public void Test50NonStandardCompKeyTableNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "NonStandardCompKeyTable");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(3);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: Key1, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: Key2, SqlTypeName: uniqueidentifier, IsPrimaryKey: True, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: NonStandardColumnName, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test55DataManyCompKeyNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataManyCompKey");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(2);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: ManyKey1, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ManyKey2, SqlTypeName: uniqueidentifier, IsPrimaryKey: True, IsNullable: False, MaxLength: 16");
        }

        [Test]
        public void Test60DataZeroOrOneNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataZeroOrOne");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(2);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyBool, SqlTypeName: bit, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
        }

        [Test]
        public void Test90ForeignKeysOk()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _sqlForeignKeys.Count.ShouldEqual(10);
            var list = _sqlForeignKeys.Select(x => x.ToString()).ToList();
            var i = 0;
            list[i++].ShouldEqual("Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId");
            list[i++].ShouldEqual("Parent: DataSingleton.DataTopId, Referenced: DataTop.DataTopId");
            list[i++].ShouldEqual("Parent: DataZeroOrOne.DataTopId, Referenced: DataTop.DataTopId");
            list[i++].ShouldEqual("Parent: NonStandardManyToManyTableName.DataTopId, Referenced: DataTop.DataTopId");
            list[i++].ShouldEqual("Parent: DataManyCompKeyDataTop.DataTop_DataTopId, Referenced: DataTop.DataTopId");
            list[i++].ShouldEqual("Parent: DataTop.Key1, Referenced: NonStandardCompKeyTable.Key1");
            list[i++].ShouldEqual("Parent: DataTop.Key2, Referenced: NonStandardCompKeyTable.Key2");
            list[i++].ShouldEqual("Parent: NonStandardManyToManyTableName.DataManyChildrenId, Referenced: DataManyChildren.DataManyChildrenId");
            list[i++].ShouldEqual("Parent: DataManyCompKeyDataTop.DataManyCompKey_ManyKey1, Referenced: DataManyCompKey.ManyKey1");
            list[i++].ShouldEqual("Parent: DataManyCompKeyDataTop.DataManyCompKey_ManyKey2, Referenced: DataManyCompKey.ManyKey2");
        }
    }
}