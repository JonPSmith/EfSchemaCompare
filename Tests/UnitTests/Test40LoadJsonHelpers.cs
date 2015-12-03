#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test40LoadJsonHelpers.cs
// Date Created: 2015/12/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test40LoadJsonHelpers
    {
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
            sqlData.TableInfos[0].ColumnInfo[0].ToString().ShouldEqual("ColumnName: DataTopId, ColumnSqlType: int, IsPrimaryKey: True, IsNullable: False, MaxLength: 4");
            sqlData.TableInfos[0].ColumnInfo[1].ToString().ShouldEqual("ColumnName: MyBool, ColumnSqlType: bit, IsPrimaryKey: False, IsNullable: False, MaxLength: 1");
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

        [Test]
        public void Test10DecodeJsonToSqlDataWithSingleAlterationOk()
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
        public void Test11DecodeJsonToSqlDataWithSingleAlterationOk()
        {
            //SETUP

            //EXECUTE
            var sqlData = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", false, "TableInfos", 0, "ColumnInfo", 0, "IsPrimaryKey");

            //VERIFY
            sqlData.TableInfos.Count.ShouldEqual(2);
            sqlData.TableInfos[0].TableName.ShouldEqual("DataTop");
            sqlData.TableInfos[1].TableName.ShouldEqual("DataChild");
        } 
    }
}