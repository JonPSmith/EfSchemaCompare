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
using Tests.EfClasses.DataTypes;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test30EfTableInfoDataTypes
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
        public void Test10DataIntDoubleOk()
        {
            //SETUP
            var classType = typeof (DataIntDouble);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(10);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataIntDoubleId, SqlTypeName: int, ClrColumName: DataIntDoubleId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataBool, SqlTypeName: bit, ClrColumName: DataBool, ClrColumnType: System.Boolean, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataByte, SqlTypeName: tinyint, ClrColumName: DataByte, ClrColumnType: System.Byte, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataShort, SqlTypeName: smallint, ClrColumName: DataShort, ClrColumnType: System.Int16, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 2");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataInt, SqlTypeName: int, ClrColumName: DataInt, ClrColumnType: System.Int32, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataLong, SqlTypeName: bigint, ClrColumName: DataLong, ClrColumnType: System.Int64, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataFloat, SqlTypeName: real, ClrColumName: DataFloat, ClrColumnType: System.Single, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDouble, SqlTypeName: float, ClrColumName: DataDouble, ClrColumnType: System.Double, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDecimalSmallPrecision, SqlTypeName: decimal, ClrColumName: DataDecimalSmallPrecision, ClrColumnType: System.Decimal, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 5");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDecimalNormal, SqlTypeName: decimal, ClrColumName: DataDecimalNormal, ClrColumnType: System.Decimal, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 9");
        }

        [Test]
        public void Test20DataStringByteOk()
        {
            //SETUP
            var classType = typeof(DataStringByte);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(13);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringByteId, SqlTypeName: int, ClrColumName: DataStringByteId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringNvarchar, SqlTypeName: nvarchar, ClrColumName: DataStringNvarchar, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: -1");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringNvarchar25, SqlTypeName: nvarchar, ClrColumName: DataStringNvarchar25, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringNchar25Fixed, SqlTypeName: nchar, ClrColumName: DataStringNchar25Fixed, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringNvarchar25NotNull, SqlTypeName: nvarchar, ClrColumName: DataStringNvarchar25NotNull, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 50");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringVarchar, SqlTypeName: varchar, ClrColumName: DataStringVarchar, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 8000");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringVarchar25, SqlTypeName: varchar, ClrColumName: DataStringVarchar25, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringChar25Fixed, SqlTypeName: char, ClrColumName: DataStringChar25Fixed, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataStringVarchar25NotNull, SqlTypeName: varchar, ClrColumName: DataStringVarchar25NotNull, ClrColumnType: System.String, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 25");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataByteBinary25, SqlTypeName: binary, ClrColumName: DataByteBinary25, ClrColumnType: System.Byte[], IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataByteVarbinary, SqlTypeName: varbinary, ClrColumName: DataByteVarbinary, ClrColumnType: System.Byte[], IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 8000");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataByteVarbinary25, SqlTypeName: varbinary, ClrColumName: DataByteVarbinary25, ClrColumnType: System.Byte[], IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: True, MaxLength: 25");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataByteVarbinary25NotNull, SqlTypeName: varbinary, ClrColumName: DataByteVarbinary25NotNull, ClrColumnType: System.Byte[], IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 25");
        }

        [Test]
        public void Test30DataDateOk()
        {
            //SETUP
            var classType = typeof(DataDate);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(7);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDateId, SqlTypeName: int, ClrColumName: DataDateId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataSqlDate, SqlTypeName: date, ClrColumName: DataSqlDate, ClrColumnType: System.DateTime, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 3");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDateTime, SqlTypeName: datetime, ClrColumName: DataDateTime, ClrColumnType: System.DateTime, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDateTime2, SqlTypeName: datetime2, ClrColumName: DataDateTime2, ClrColumnType: System.DateTime, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataSmallDateTime, SqlTypeName: smalldatetime, ClrColumName: DataSmallDateTime, ClrColumnType: System.DateTime, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataDateTimeOffset, SqlTypeName: datetimeoffset, ClrColumName: DataDateTimeOffset, ClrColumnType: System.DateTimeOffset, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 10");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataTimeSpan, SqlTypeName: time, ClrColumName: DataTimeSpan, ClrColumnType: System.TimeSpan, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 5");
        }

        [Test]
        public void Test40DataGuidEnumOk()
        {
            //SETUP
            var classType = typeof(DataGuidEnum);

            //EXECUTE
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == classType);

            //VERIFY
            efInfo.ShouldNotEqualNull();
            efInfo.TableName.ShouldEqual(classType.Name);
            //foreach (var col in efInfo.NormalCols)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            efInfo.NormalCols.Count.ShouldEqual(6);
            var list = efInfo.NormalCols.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("SqlColumnName: DataGuidEnumId, SqlTypeName: int, ClrColumName: DataGuidEnumId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: DataGuid, SqlTypeName: uniqueidentifier, ClrColumName: DataGuid, ClrColumnType: System.Guid, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("SqlColumnName: ByteEnum, SqlTypeName: tinyint, ClrColumName: ByteEnum, ClrColumnType: Tests.EfClasses.DataTypes.ByteEnum, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 1");
            list[i++].ToString().ShouldEqual("SqlColumnName: ShortEnum, SqlTypeName: smallint, ClrColumName: ShortEnum, ClrColumnType: Tests.EfClasses.DataTypes.ShortEnum, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 2");
            list[i++].ToString().ShouldEqual("SqlColumnName: NormalEnum, SqlTypeName: int, ClrColumName: NormalEnum, ClrColumnType: Tests.EfClasses.DataTypes.NormalEnum, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("SqlColumnName: LongEnum, SqlTypeName: bigint, ClrColumName: LongEnum, ClrColumnType: Tests.EfClasses.DataTypes.LongEnum, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 8");
        }


    }
}