#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test25SqlTableInfo.cs
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
    public class Test25SqlTableInfo
    {

        private ICollection<SqlTableInfo> _sqlInfos;
        private ICollection<SqlForeignKeys> _sqlForeignKeys;
                   
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var connection = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;
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
            _sqlInfos.Count.ShouldEqualWithTolerance(11,1);      //we allow for the __MirgartionHistory
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
            list.Count.ShouldEqual(5);
            list[i++].ToString().ShouldEqual("ColumnName: Key1, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: Key2, ColumnSqlType: uniqueidentifier, IsPrimaryKey: False, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyString, ColumnSqlType: varchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataSingletonId, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: True, MaxLength: 4");
        }

        [Test]
        public void Test20DataChildNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataChild");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(4);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataChildId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyString, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: -1");
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
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
            list[i++].ToString().ShouldEqual("ColumnName: NonStandardForeignKeyName, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: True, MaxLength: 4");
        }

        [Test]
        public void Test50NonStandardCompKeyTableNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "NonStandardCompKeyTable");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(3);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: Key1, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: Key2, ColumnSqlType: uniqueidentifier, IsPrimaryKey: True, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: NonStandardColumnName, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test55DataManyCompKeyNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataManyCompKey");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(2);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: ManyKey1, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ManyKey2, ColumnSqlType: uniqueidentifier, IsPrimaryKey: True, IsNullable: False, MaxLength: 16");
        }

        [Test]
        public void Test60DataZeroOrOneNormalColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataZeroOrOne");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(2);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyBool, ColumnSqlType: bit, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
        }

        [Test]
        public void Test65DataComplexColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataComplex");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(7);
            //foreach (var col in sqlInfo.ColumnInfo)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataComplexId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexData_ComplexInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexData_ComplexString, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexDateTime, ColumnSqlType: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexGuid, ColumnSqlType: uniqueidentifier, IsPrimaryKey: False, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexData_ComplexInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexData_ComplexString, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
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
            list[i++].ShouldEqual("Parent: DataSingleton.DataSingletonId, Referenced: DataTop.DataTopId");
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