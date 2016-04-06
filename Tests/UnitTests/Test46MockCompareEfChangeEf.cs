#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test46MockCompareEfChangeEf.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
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
    public class Test46MockCompareEfChangeEf
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
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", "NewTableName", 0, "TableName");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var sqlInfoDict = sqlData.TableInfos.ToDictionary(x => x.CombinedName);
            var comparer = new EfCompare("SqlRefString", sqlInfoDict);

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Table: The SQL SqlRefString does not contain a table called [dbo].[NewTableName]. Needed by EF class DataTop.\n" +
            "Missing SQL Table: Could not find the SQL table called [dbo].[NewTableName].", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false);
            sqlInfoDict.Keys.Count.ShouldEqual(1);
            sqlInfoDict.ContainsKey("[dbo].[DataTop]").ShouldEqual(true);
        }

        //-----------------------------------------------------
        //column errors

        [Test]
        public void Test10CompareMockDataChangeColumnNameOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", "BadName", 0, "NormalCols", 0, "SqlColumnName");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Column: The SQL SqlRefString table [dbo].[DataTop] does not contain a column called BadName. Needed by EF class DataTop.", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: SQL SqlRefString table [dbo].[DataTop] has a column called DataTopId (.NET type System.Int32) that EF does not access.", string.Join(",", status.Warnings));
        }

        [Test]
        public void Test11CompareMockDataChangeColumnSqlTypeOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", "varchar", 0, "NormalCols", 0, "SqlTypeName");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column Type: The SQL SqlRefString column [dbo].[DataTop].DataTopId type does not match EF. SQL type = int, EF type = varchar.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test12CompareMockDataChangePrimaryKeyOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", false, 0, "NormalCols", 0, "IsPrimaryKey");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Primary Key: The SQL SqlRefString  column [dbo].[DataTop].DataTopId primary key settings don't match. SQL says it is a key, EF says it is NOT a key.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test13CompareMockDataChangePrimaryKeyOrderOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", 2, 0, "NormalCols", 0, "PrimaryKeyOrder");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Primary Key Order: The SQL SqlRefString  column [dbo].[DataTop].DataTopId primary key order does not match. SQL order = 1, EF order = 2.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test14CompareMockDataChangeIsNullableOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", true, 0, "NormalCols", 0, "IsNullable");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Column Nullable: SQL SqlRefString column [dbo].[DataTop].DataTopId nullablity does not match. SQL is NOT NULL, EF is NULL.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test15CompareMockDataChangeMaxLengthOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", 2, 0, "NormalCols", 0, "MaxLength");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("MaxLength: The  SQL SqlRefString  column [dbo].[DataTop].DataTopId, type System.Int32, length does not match EF. SQL length = 4, EF length = 2.", status.GetAllErrors());
            status.HasWarnings.ShouldEqual(false, string.Join(",", status.Warnings));
        }

        [Test]
        public void Test16CompareMockDataRemoveColumnInToBeCheckedOk()
        {
            //SETUP
            var efData = LoadJsonHelpers.DeserializeArrayWithSingleRemoval<List<EfTableInfo>>("EfTableInfos01*.json", 1, "NormalCols", 1);
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");
            var comparer = new EfCompare("SqlRefString", sqlData.TableInfos.ToDictionary(x => x.CombinedName));

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid();
            string.Join(",", status.Warnings).ShouldEqual("Warning: SQL SqlRefString table [dbo].[DataChild] has a column called MyString (.NET type System.String) that EF does not access.", string.Join(",", status.Warnings));
        }

        //TODO: add index checks and 
    }
}