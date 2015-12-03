#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test40LoadJsonHelpers.cs
// Date Created: 2015/12/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using CompareCore.EFInfo;
using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test40LoadJsonHelpers
    {
        //----------------------------------------
        //SqlAllInfo

        [Test]
        public void Test01DecodeJsonToSqlDataCheckTopLayerOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //VERIFY
            sqlData.TableInfos.Count.ShouldEqual(2);
            sqlData.ForeignKeys.Count.ShouldEqual(1);
            sqlData.Indexes.Count.ShouldEqual(3);
        }

        [Test]
        public void Test02DecodeJsonToSqlDataCheckTablesOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //VERIFY
            sqlData.TableInfos.Count.ShouldEqual(2);
            sqlData.TableInfos[0].ToString().ShouldEqual("Name: dbo.DataTop, Columns: 2");
            sqlData.TableInfos[1].ToString().ShouldEqual("Name: dbo.DataChild, Columns: 3");
        }

        [Test]
        public void Test03DecodeJsonToSqlDataCheckTableDataTopOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //VERIFY
            sqlData.TableInfos.Count.ShouldEqual(2);
            sqlData.TableInfos[0].TableName.ShouldEqual("DataTop");
            sqlData.TableInfos[0].ColumnInfos[0].ToString().ShouldEqual("ColumnName: DataTopId, SqlTypeName: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            sqlData.TableInfos[0].ColumnInfos[1].ToString().ShouldEqual("ColumnName: MyBool, SqlTypeName: bit, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
        }

        [Test]
        public void Test05DecodeJsonToSqlDataCheckForeignKeysOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //VERIFY
            sqlData.ForeignKeys.Count.ShouldEqual(1);
            sqlData.ForeignKeys[0].ToString().ShouldEqual("Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId");
        }

        [Test]
        public void Test06DecodeJsonToSqlDataCheckIndexesOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //VERIFY
            sqlData.Indexes.Count.ShouldEqual(3);
            sqlData.Indexes[0].ToString().ShouldEqual("[dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique)");
            sqlData.Indexes[1].ToString().ShouldEqual("[dbo].[DataChild].DataChildId: (primary key, clustered, unique)");
            sqlData.Indexes[2].ToString().ShouldEqual("[dbo].[DataTop].DataTopId: (primary key, clustered, unique)");
        }


        //----------------------------------------
        //EfTableInfo

        [Test]
        public void Test20CheckDecodeTypeOk()
        {
            //SETUP

            //EXECUTE
            var decodedType = Type.GetType("Tests.EfClasses.Relationships.DataTop");

            //VERIFY
            decodedType.ShouldNotEqualNull();
        }

        [Test]
        public void Test21DecodeJsonToEfDataCheckTopLayerOk()
        {
            //SETUP

            //EXECUTE
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");

            //VERIFY
            efData.Count.ShouldEqual(2);
            efData[0].ToString().ShouldEqual("Name: dbo.DataTop, NormalCols: 2, Relationships: 0");
            efData[1].ToString().ShouldEqual("Name: dbo.DataChild, NormalCols: 3, Relationships: 1");
        }

        [Test]
        public void Test22DecodeJsonToEfDataCheckFirstTableColumnsOk()
        {
            //SETUP

            //EXECUTE
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");

            //VERIFY
            efData[0].NormalCols.Count.ShouldEqual(2);
            efData[0].NormalCols[0].ToString().ShouldEqual("SqlColumnName: DataTopId, SqlTypeName: int, ClrColumName: DataTopId, ClrColumnType: System.Int32, IsPrimaryKey: True, PrimaryKeyOrder: 1, IsNullable: False, MaxLength: 4");
            efData[0].NormalCols[1].ToString().ShouldEqual("SqlColumnName: MyBool, SqlTypeName: bit, ClrColumName: DataTopId, ClrColumnType: System.Boolean, IsPrimaryKey: False, PrimaryKeyOrder: 0, IsNullable: False, MaxLength: 1");
        }

        [Test]
        public void Test23DecodeJsonToEfDataCheckFirstTableRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");

            //VERIFY
            efData[0].RelationshipCols.Count.ShouldEqual(0);
        }

        [Test]
        public void Test25DecodeJsonToEfDataCheckSecondTableColumnsOk()
        {
            //SETUP

            //EXECUTE
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");

            //VERIFY
            efData[0].NormalCols.Count.ShouldEqual(2);
            efData[0].NormalCols[0].ToString().ShouldEqual("xxx");
            efData[0].NormalCols[1].ToString().ShouldEqual("xxx");
        }

        [Test]
        public void Test26DecodeJsonToEfDataCheckSecondTableRelationshipsOk()
        {
            //SETUP

            //EXECUTE
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");

            //VERIFY
            efData[1].RelationshipCols.Count.ShouldEqual(1);
            efData[1].RelationshipCols[0].ToString().ShouldEqual("xxx");
        }

        //-----------------------------------------------------------------
        //try altering data

        [Test]
        public void Test40DecodeJsonToSqlDataWithSingleAlterationOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "NewDataName", "TableInfos", 0, "TableName");

            //VERIFY
            sqlData.TableInfos.Count.ShouldEqual(2);
            sqlData.TableInfos[0].TableName.ShouldEqual("NewDataName");
            sqlData.TableInfos[1].TableName.ShouldEqual("DataChild");
        }

        [Test]
        public void Test41DecodeJsonToSqlDataWithSingleAlterationOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", false, "TableInfos", 0, "ColumnInfos", 0, "IsPrimaryKey");

            //VERIFY
            sqlData.TableInfos.Count.ShouldEqual(2);
            sqlData.TableInfos[0].TableName.ShouldEqual("DataTop");
            sqlData.TableInfos[1].TableName.ShouldEqual("DataChild");
        } 
    }
}