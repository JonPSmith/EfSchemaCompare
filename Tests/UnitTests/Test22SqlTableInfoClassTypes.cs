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
    public class Test22SqlTableInfoClassTypes
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

    }
}