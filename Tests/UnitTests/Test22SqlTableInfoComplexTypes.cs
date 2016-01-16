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
    public class Test22SqlTableInfoComplexTypes
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
        public void Test65DataComplexColsOk()
        {
            //SETUP

            //EXECUTE
            var sqlInfo = _sqlInfos.SingleOrDefault(x => x.TableName == "DataComplex");

            //VERIFY
            sqlInfo.ShouldNotEqualNull();
            sqlInfo.ColumnInfos.Count.ShouldEqual(7);
            //foreach (var col in sqlInfo.ColumnInfos)
            //{
            //    Console.WriteLine("list[i++].ToString().ShouldEqual(\"{0}\");", col);
            //}
            var list = sqlInfo.ColumnInfos.ToList();
            var i = 0;
            list[i++].ToString().ShouldEqual("ColumnName: DataComplexId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexData_ComplexInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexData_ComplexString, SqlTypeName: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexDateTime, SqlTypeName: datetime, IsPrimaryKey: False, IsNullable: False, MaxLength: 8");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexGuid, SqlTypeName: uniqueidentifier, IsPrimaryKey: False, IsNullable: False, MaxLength: 16");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexData_ComplexInt, SqlTypeName: int, IsPrimaryKey: False, IsNullable: False, MaxLength: 4");
            list[i++].ToString().ShouldEqual("ColumnName: ComplexComplexData_ComplexData_ComplexString, SqlTypeName: nvarchar, IsPrimaryKey: False, IsNullable: True, MaxLength: 50");
        }

    }
}