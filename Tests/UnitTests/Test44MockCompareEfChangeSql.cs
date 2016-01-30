#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test44MockCompareEfChangeSql.cs
// Date Created: 2015/12/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.Linq;
using CompareCore;
using CompareCore.EFInfo;
using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test44MockCompareEfChangeSql
    {
        [Test]
        public void Test01CompareSameMockDataOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid();
        }

        //-------------------------------------------------
        //Table errors

        [Test]
        public void Test05CompareMockDataChangeTableNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "NewDataName", "TableInfos", 0, "TableName");
            var sqlInfoDict = sqlData.TableInfos.ToDictionary(x => x.CombinedName);
            var comparer = new EfCompare("SqlRefString", sqlInfoDict);

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Table: The SQL SqlRefString does not contain a table called [dbo].[DataTop]. Needed by EF class DataTop.\n" +
            "Missing SQL Table: Could not find the SQL table called [dbo].[DataTop].", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false);
            sqlInfoDict.Keys.Count.ShouldEqual(1);
            sqlInfoDict.ContainsKey("[dbo].[NewDataName]").ShouldEqual(true);
        }

        //-----------------------------------------------------
        //column errors

        [Test]
        public void Test10CompareMockDataChangeColumnNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadColName", "TableInfos", 0, "ColumnInfos", 0, "ColumnName");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Column: The SQL SqlRefString table [dbo].[DataTop] does not contain a column called DataTopId. Needed by EF class DataTop.\n"+
                "Missing Foreign Key: EF has a Many-to-One relationship between DataChild.Parent and DataTop but we don't find that in SQL.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: SQL SqlRefString table [dbo].[DataTop] has a column called BadColName (.NET type System.Int32) that EF does not access.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test11CompareMockDataChangeColumnSqlTypeOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "bit", "TableInfos", 0, "ColumnInfos", 0, "SqlTypeName");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column Type: The SQL SqlRefString column [dbo].[DataTop].DataTopId type does not match EF. SQL type = bit, EF type = int.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test12CompareMockDataChangePrimaryKeyOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", false, "TableInfos", 0, "ColumnInfos", 0, "IsPrimaryKey");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Primary Key: The SQL SqlRefString  column [dbo].[DataTop].DataTopId primary key settings don't match. SQL says it is NOT a key, EF says it is a key.\n"+
                "Missing Foreign Key: EF has a Many-to-One relationship between DataChild.Parent and DataTop but we don't find that in SQL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test13CompareMockDataChangePrimaryKeyOrderOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", 2, "TableInfos", 0, "ColumnInfos", 0, "PrimaryKeyOrder");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Primary Key Order: The SQL SqlRefString  column [dbo].[DataTop].DataTopId primary key order does not match. SQL order = 2, EF order = 1.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test14CompareMockDataChangeIsNullableOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "TableInfos", 0, "ColumnInfos", 0, "IsNullable");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column Nullable: SQL SqlRefString column [dbo].[DataTop].DataTopId nullablity does not match. SQL is NULL, EF is NOT NULL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test15CompareMockDataChangeMaxLengthOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", 2, "TableInfos", 0, "ColumnInfos", 0, "MaxLength");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("MaxLength: The  SQL SqlRefString  column [dbo].[DataTop].DataTopId, type System.Int32, length does not match EF. SQL length = 2, EF length = 4.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test16CompareMockDataRemoveColumnInToBeCheckedOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "TableInfos", 0, "ColumnInfos", 0);
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Column: The SQL SqlRefString table [dbo].[DataTop] does not contain a column called DataTopId. Needed by EF class DataTop.\n"+
                "Missing Foreign Key: EF has a Many-to-One relationship between DataChild.Parent and DataTop but we don't find that in SQL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        //--------------------------------------------------
        //foreign key errors

        [Test]
        public void Test20CompareMockDataChangeForeignKeyParentTableNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ParentTableName");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign Key: EF has a Many-to-One relationship between DataChild.Parent and DataTop but we don't find that in SQL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Ignore("CompareEfWithSql will NOT catch this error")]
        [Test]
        public void Test21CompareMockDataChangeForeignKeyParentColNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ParentColName");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign key: The 'RefUnitTest' SQL database has a foreign key Parent: DataChild.DataTopId, Referenced: DataTop.DataTopId, which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has a foreign key Parent: DataChild.BadName, Referenced: DataTop.DataTopId, which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test22CompareMockDataChangeForeignKeyReferencedTableNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ReferencedTableName");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign Key: EF has a Many-to-One relationship between DataChild.Parent and DataTop but we don't find that in SQL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test23CompareMockDataChangeForeignKeyReferencedColNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "ReferencedColName");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Foreign Key: EF has a Many-to-One relationship between DataChild.Parent and DataTop but we don't find that in SQL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test24CompareMockDataChangeForeignKeyReferencedColNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "ForeignKeys", 0, "DeleteAction");
                        var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Cascade Delete: The Many-to-One relationship between DataChild.Parent and DataTop has different cascase delete value. SQL foreign key say BadName, EF setting is CASCADE.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        //-------------------------------------------
        //Indexes

        //!!NOT IMPLEMENTED YET

        //[Test]
        //public void Test30CompareMockDataChangeIndexTableNameOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "Indexes", 0, "TableName");

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
        //    string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has an index [dbo].[BadName].DataTopId: (not primary key, not clustered, not unique), which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        //}

        //[Test]
        //public void Test31CompareMockDataChangeIndexColumnNameOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", "BadName", "Indexes", 0, "ColumnName");

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
        //    string.Join(",", status.Warnings).ShouldEqual("Warning: The 'ToBeCheckUnitTest' database has an index [dbo].[DataChild].BadName: (not primary key, not clustered, not unique), which the 'RefUnitTest' database did not have.", string.Join(",", status.Warnings));
        //}

        //[Test]
        //public void Test32CompareMockDataChangeIndexIsPrimaryIndexOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "Indexes", 0, "IsPrimaryIndex");

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Index Mismatch: The 'RefUnitTest' SQL database, the index on [dbo].[DataChild].DataTopId is NOT a primary key index, while the index on the same table.column in SQL database ToBeCheckUnitTest is a primary key index.", status.GetAllErrors());
        //    status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        //}

        //[Test]
        //public void Test33CompareMockDataChangeIndexClusteredOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "Indexes", 0, "Clustered");

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Index Mismatch: The 'RefUnitTest' SQL database, the index on [dbo].[DataChild].DataTopId is NOT clustered, while the index on the same table.column in SQL database ToBeCheckUnitTest is clustered.", status.GetAllErrors());
        //    status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        //}

        //[Test]
        //public void Test34CompareMockDataChangeIndexIsUniqueOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleAlteration<SqlAllInfo>("SqlAllInfo01*.json", true, "Indexes", 0, "IsUnique");

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Index Mismatch: The 'RefUnitTest' SQL database, index on [dbo].[DataChild].DataTopId is NOT unique, while the index on the same table.column in SQL database ToBeCheckUnitTest is unique.", status.GetAllErrors());
        //    status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        //}

        //[Test]
        //public void Test35CompareMockDataChangeIndexRemoveNonPrimaryKeyInSetOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "Indexes", 0);

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataTopId: (not primary key, not clustered, not unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
        //    status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        //}

        //[Test]
        //public void Test36CompareMockDataChangeIndexRemovePrimaryKeyInSetOk()
        //{
        //    //SETUP
        //    var comparer = new EfCompare("SqlRefString","");
        //    var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
        //    var sqlData = LoadJsonHelpers.DeserializeObjectWithSingleRemoval<SqlAllInfo>("SqlAllInfo01*.json", "Indexes", 1);

        //    //EXECUTE
        //    var status = comparer.CompareEfWithSql(efData, sqlData);

        //    //VERIFY
        //    status.ShouldBeValid(false);
        //    status.GetAllErrors().ShouldEqual("Missing Index: The 'RefUnitTest' SQL database has an index [dbo].[DataChild].DataChildId: (primary key, clustered, unique), which is missing in the 'ToBeCheckUnitTest' database.", status.GetAllErrors());
        //    status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        //}
    }
}