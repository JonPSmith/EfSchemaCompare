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
using EfPocoClasses.DataTypes;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test20SqlTableInfoDataTypes
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
            //foreach (var col in sqlInfo.ColumnInfos)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list.Count.ShouldEqual(10);
            list[i++].ToString().ShouldEqual("ColumnName: DataIntDoubleId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataBool, SqlTypeName: bit, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("ColumnName: DataByte, SqlTypeName: tinyint, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("ColumnName: DataShort, SqlTypeName: smallint, IsPrimaryKey: False, IsNullable: False, MaxLength: 2");
            list[i++].ToString().ShouldEqual("ColumnName: DataInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataLong, SqlTypeName: bigint, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataFloat, SqlTypeName: real, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataDouble, SqlTypeName: float, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataDecimalSmallPrecision, SqlTypeName: decimal, IsPrimaryKey: False, IsNullable: False, MaxLength: 5");
            list[i++].ToString().ShouldEqual("ColumnName: DataDecimalNormal, SqlTypeName: decimal, IsPrimaryKey: False, IsNullable: False, MaxLength: 9");

        }

        [Test]
        public void Test20DataStringByteOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataStringByte).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            foreach (var col in sqlInfo.ColumnInfos)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            sqlInfo.ColumnInfos.Count.ShouldEqual(13);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataStringByteId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNvarchar, SqlTypeName: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: -1");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNvarchar25, SqlTypeName: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNchar25Fixed, SqlTypeName: nchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNvarchar25NotNull, SqlTypeName: nvarchar, IsPrimaryKey: False, IsNullable: False, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringVarchar, SqlTypeName: varchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 8000");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringVarchar25, SqlTypeName: varchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringChar25Fixed, SqlTypeName: char, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringVarchar25NotNull, SqlTypeName: varchar, IsPrimaryKey: False, IsNullable: False, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteBinary25, SqlTypeName: binary, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteVarbinary, SqlTypeName: varbinary, IsPrimaryKey: False, IsNullable: True, MaxLength: 8000");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteVarbinary25, SqlTypeName: varbinary, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteVarbinary25NotNull, SqlTypeName: varbinary, IsPrimaryKey: False, IsNullable: False, MaxLength: 25");
        }

        [Test]
        public void Test30DataDateOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataDate).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            //foreach (var col in sqlInfo.ColumnInfos)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            sqlInfo.ColumnInfos.Count.ShouldEqual(7);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataDateId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataSqlDate, SqlTypeName: date, IsPrimaryKey: False, IsNullable: False, MaxLength: 3");
            list[i++].ToString().ShouldEqual("ColumnName: DataDateTime, SqlTypeName: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataDateTime2, SqlTypeName: datetime2, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataSmallDateTime, SqlTypeName: smalldatetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataDateTimeOffset, SqlTypeName: datetimeoffset, IsPrimaryKey: False, IsNullable: False, MaxLength: 10");
            list[i++].ToString().ShouldEqual("ColumnName: DataTimeSpan, SqlTypeName: time, IsPrimaryKey: False, IsNullable: False, MaxLength: 5");

        }

        [Test]
        public void Test40DataGuidEnumOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataGuidEnum).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            foreach (var col in sqlInfo.ColumnInfos)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            sqlInfo.ColumnInfos.Count.ShouldEqual(6);
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataGuidEnumId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataGuid, SqlTypeName: uniqueidentifier, IsPrimaryKey: False, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: ByteEnum, SqlTypeName: tinyint, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("ColumnName: ShortEnum, SqlTypeName: smallint, IsPrimaryKey: False, IsNullable: False, MaxLength: 2");
            list[i++].ToString().ShouldEqual("ColumnName: NormalEnum, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: LongEnum, SqlTypeName: bigint, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
        }

 
    }
}