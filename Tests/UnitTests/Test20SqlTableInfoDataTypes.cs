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
            //foreach (var col in sqlInfo.ColumnInfo)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list.Count.ShouldEqual(10);
            list[i++].ToString().ShouldEqual("ColumnName: DataIntDoubleId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataBool, ColumnSqlType: bit, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("ColumnName: DataByte, ColumnSqlType: tinyint, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("ColumnName: DataShort, ColumnSqlType: smallint, IsPrimaryKey: False, IsNullable: False, MaxLength: 2");
            list[i++].ToString().ShouldEqual("ColumnName: DataInt, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataLong, ColumnSqlType: bigint, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataFloat, ColumnSqlType: real, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataDouble, ColumnSqlType: float, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataDecimalSmallPrecision, ColumnSqlType: decimal, IsPrimaryKey: False, IsNullable: False, MaxLength: 5");
            list[i++].ToString().ShouldEqual("ColumnName: DataDecimalNormal, ColumnSqlType: decimal, IsPrimaryKey: False, IsNullable: False, MaxLength: 9");

        }

        [Test]
        public void Test20DataStringByteOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataStringByte).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            foreach (var col in sqlInfo.ColumnInfo)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            sqlInfo.ColumnInfo.Count.ShouldEqual(13);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataStringByteId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNvarchar, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: -1");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNvarchar25, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNchar25Fixed, ColumnSqlType: nchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringNvarchar25NotNull, ColumnSqlType: nvarchar, IsPrimaryKey: False, IsNullable: False, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringVarchar, ColumnSqlType: varchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 8000");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringVarchar25, ColumnSqlType: varchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringChar25Fixed, ColumnSqlType: char, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataStringVarchar25NotNull, ColumnSqlType: varchar, IsPrimaryKey: False, IsNullable: False, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteBinary25, ColumnSqlType: binary, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteVarbinary, ColumnSqlType: varbinary, IsPrimaryKey: False, IsNullable: True, MaxLength: 8000");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteVarbinary25, ColumnSqlType: varbinary, IsPrimaryKey: False, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("ColumnName: DataByteVarbinary25NotNull, ColumnSqlType: varbinary, IsPrimaryKey: False, IsNullable: False, MaxLength: 25");
        }

        [Test]
        public void Test30DataDateOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataDate).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            //foreach (var col in sqlInfo.ColumnInfo)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            sqlInfo.ColumnInfo.Count.ShouldEqual(7);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataDateId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataSqlDate, ColumnSqlType: date, IsPrimaryKey: False, IsNullable: False, MaxLength: 3");
            list[i++].ToString().ShouldEqual("ColumnName: DataDateTime, ColumnSqlType: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataDateTime2, ColumnSqlType: datetime2, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: DataSmallDateTime, ColumnSqlType: smalldatetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataDateTimeOffset, ColumnSqlType: datetimeoffset, IsPrimaryKey: False, IsNullable: False, MaxLength: 10");
            list[i++].ToString().ShouldEqual("ColumnName: DataTimeSpan, ColumnSqlType: time, IsPrimaryKey: False, IsNullable: False, MaxLength: 5");

        }

        [Test]
        public void Test40DataGuidEnumOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == typeof(DataGuidEnum).Name);

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            foreach (var col in sqlInfo.ColumnInfo)
            {
                Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            }
            sqlInfo.ColumnInfo.Count.ShouldEqual(6);
            var list = sqlInfo.ColumnInfo.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataGuidEnumId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: DataGuid, ColumnSqlType: uniqueidentifier, IsPrimaryKey: False, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: ByteEnum, ColumnSqlType: tinyint, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("ColumnName: ShortEnum, ColumnSqlType: smallint, IsPrimaryKey: False, IsNullable: False, MaxLength: 2");
            list[i++].ToString().ShouldEqual("ColumnName: NormalEnum, ColumnSqlType: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: LongEnum, ColumnSqlType: bigint, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
        }

 
    }
}