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
using Tests.EfClasses.DataTypes;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test20SqlTableInfoDataTypes
    {

        private ICollection<SqlTableInfo> _sqlInfos;
                   
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var connection = ConfigurationManager.ConnectionStrings[DatabaseHelpers.EfDatabaseConfigName].ConnectionString;
            var allSqlInfo = SqlAllInfo.SqlAllInfoFactory(connection);
            _sqlInfos = allSqlInfo.TableInfos;
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
        public void Test10DataIntDoubleOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataIntDouble).Name);

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
        public void Test20DataStringByteOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataStringByte).Name);

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
        public void Test30DataDateOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataDate).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(2);
            sqlInfo.ColumnInfo.First().ToString().ShouldEqual("ColumnName: DataManyChildrenId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            sqlInfo.ColumnInfo.Last().ToString().ShouldEqual("ColumnName: MyInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
        }

        [Test]
        public void Test40DataGuidEnumOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataGuidEnum).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfo.Count.ShouldEqual(3);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataSingletonId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: MyDateTime, ColumnSqlType: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: NonStandardForeignKeyName, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: True, MaxLength: 4");
        }

 
    }
}