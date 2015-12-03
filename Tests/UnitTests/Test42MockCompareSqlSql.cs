#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test40LoadJsonHelpers.cs
// Date Created: 2015/12/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations.Schema;
using CompareCore;
using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test42MockCompareSqlSql
    {

        [Test]
        public void Test01CompareSameMockDataOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData, sqlData);

            //VERIFY
            status.ShouldBeValid();
        }

        //-------------------------------------------------
        //Table errors

        [Test]
        public void Test05CompareMockDataChangeTableNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "NewDataName", "TableInfos", 0, "TableName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Table: The 'RefUnitTest' SQL database has a table called [dbo].[DataTop], which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: SQL database 'RefUnitTest', table [dbo].[NewDataName] table contained an extra table, [dbo].[NewDataName]", string.Join(",", status.Warnings));
        }

        //-----------------------------------------------------
        //column errors

        [Test]
        public void Test10CompareMockDataChangeColumnNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadColName", "TableInfos", 0, "ColumnInfos", 0, "ColumnName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Column: The SQL table [dbo].[DataTop] in second database does not contain a column called DataTopId.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database SQL table [dbo].[DataTop] has a column called BadColName (type int), which database 'RefUnitTest' did not have.", string.Join(",", status.Warnings));       
        }

        [Test]
        public void Test11CompareMockDataChangeColumnSqlTypeOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "bit", "TableInfos", 0, "ColumnInfos", 0, "SqlTypeName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column Type: SQL column [dbo].[DataTop].DataTopId type does not match EF. 'RefUnitTest' db type = int, 'ToBeCheckUnitTest' db type = bit.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test12CompareMockDataChangePrimaryKeyOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", false, "TableInfos", 0, "ColumnInfos", 0, "IsPrimaryKey");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Primary Key: The SQL column [dbo].[DataTop].DataTopId primary key settings don't match. 'RefUnitTest' db says is a key, 'ToBeCheckUnitTest' db says it is NOT a key.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test13CompareMockDataChangePrimaryKeyOrderOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", 2, "TableInfos", 0, "ColumnInfos", 0, "PrimaryKeyOrder");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Key Order: The SQL column [dbo].[DataTop].DataTopId primary key order does not match. 'RefUnitTest' db order = 1, 'ToBeCheckUnitTest' db order = 2.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test14CompareMockDataChangeIsNullableOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "TableInfos", 0, "ColumnInfos", 0, "IsNullable");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column Nullable: SQL column [dbo].[DataTop].DataTopId nullablity does not match. 'RefUnitTest' db is NOT NULL, 'ToBeCheckUnitTest' db is NULL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test15CompareMockDataChangeMaxLengthOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", 2, "TableInfos", 0, "ColumnInfos", 0, "MaxLength");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column MaxLength: SQL column [dbo].[DataTop].DataTopId MaxLength does not match. 'RefUnitTest' db MaxLength = RefUnitTest, 'ToBeCheckUnitTest' db MaxLength = 2.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test16CompareMockDataRemoveColumnInToBeCheckedOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "TableInfos", 0, "ColumnInfos", 0);

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Column: The SQL table [dbo].[DataTop] in second database does not contain a column called DataTopId.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test17CompareMockDataRemoveColumnInRefOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeDataWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "TableInfos", 0, "ColumnInfos", 0);
            var sqlData2 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid();
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database SQL table [dbo].[DataTop] has a column called DataTopId (type int), which database 'RefUnitTest' did not have.", string.Join(",", status.Warnings));
        }

        //--------------------------------------------------
        //foreign key errors

        [Test]
        public void Test20CompareMockDataChangeForeignKeyParentTableNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ParentTableName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign key: The 'RefUnitTest' SQL database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId, which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has a foreign key Parent: BadName.DataTopId, Referenced: DataTop.DataTopId, which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test21CompareMockDataChangeForeignKeyParentColNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ParentColName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign key: The 'RefUnitTest' SQL database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId, which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has a foreign key Parent: DataChild.BadName, Referenced: DataTop.DataTopId, which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test22CompareMockDataChangeForeignKeyReferencedTableNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ReferencedTableName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign key: The 'RefUnitTest' SQL database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId, which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has a foreign key Parent: DataChild.DataTopId, Referenced: BadName.DataTopId, which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test23CompareMockDataChangeForeignKeyReferencedColNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ReferencedColName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign key: The 'RefUnitTest' SQL database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId, which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.BadName, which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test24CompareMockDataChangeForeignKeyReferencedColNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "DeleteAction");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Foreign Key Delete Action: The [RefUnitTest] database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId that has delete action of CASCADE, while database [ToBeCheckUnitTest] has delete action of BadName.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        //-------------------------------------------
        //Indexes

        [Test]
        public void Test30CompareMockDataChangeIndexTableNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "Indexes", 0, "TableName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has an index [dbo].[BadName].DataTopId: (not primary key, not clustered, not unique), which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test31CompareMockDataChangeIndexColumnNameOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "Indexes", 0, "ColumnName");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has an index [dbo].[DataChild].BadName: (not primary key, not clustered, not unique), which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test32CompareMockDataChangeIndexIsPrimaryIndexOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "Indexes", 0, "IsPrimaryIndex");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Index Mismatch: The 'RefUnitTest' SQL database, the index on [dbo].[DataChild].DataTopId is NOT a primary key index, while the index on the same table.column in SQL database ToBeCheckUnitTest is a primary key index.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test33CompareMockDataChangeIndexClusteredOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "Indexes", 0, "Clustered");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Index Mismatch: The 'RefUnitTest' SQL database, the index on [dbo].[DataChild].DataTopId is NOT clustered, while the index on the same table.column in SQL database ToBeCheckUnitTest is clustered.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test34CompareMockDataChangeIndexIsUniqueOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "Indexes", 0, "IsUnique");

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Index Mismatch: The 'RefUnitTest' SQL database, index on [dbo].[DataChild].DataTopId is NOT unique, while the index on the same table.column in SQL database ToBeCheckUnitTest is unique.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test35CompareMockDataChangeIndexRemoveNonPrimaryKeyInSetOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "Indexes", 0);

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test36CompareMockDataChangeIndexRemovePrimaryKeyInSetOk()
        {
            //SETUP
            var comparer = new SqlCompare("RefUnitTest", "ToBeCheckUnitTest", "", true);
            var sqlData1 = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlData2 = LoadJsonHelpers.DeserializeDataWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "Indexes", 1);

            //EXECUTE
            var status = comparer.CompareSqlToSql(sqlData1, sqlData2);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataChildId: (primary key, clustered, unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }
    }
}