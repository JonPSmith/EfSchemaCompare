#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test46MockCompareEfChangeEf.cs
// Date Created: 2015/12/04
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
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
            var comparer = new EfCompare("SqlRefString", "");
            var efData = LoadJsonHelpers.DeserializeData<List<EfTableInfo>>("EfTableInfos01*.json");
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

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
            var comparer = new EfCompare("SqlRefString", "");
            var efData = LoadJsonHelpers.DeserializeDataWithSingleAlteration<List<EfTableInfo>>("EfTableInfos01*.json", );
            var sqlData = LoadJsonHelpers.DeserializeData<SqlAllInfo>("SqlAllInfo01*.json");

            //EXECUTE
            var status = comparer.CompareEfWithSql(efData, sqlData);

            //VERIFY
            status.ShouldBeValid(false);
            status.GetAllErrors().ShouldEqual("Missing Table: The SQL SqlRefString does not contain a table called [dbo].[DataTop]. Needed by EF class DataTop.\n" +
            "Missing SQL Table: Could not find the SQL table called [dbo].[DataTop].", status.GetAllErrors());
            string.Join(",", status.Warnings).ShouldEqual("Warning: SQL SqlRefString table [dbo].[NewDataName] was not used by EF.", string.Join(",", status.Warnings));
        } 
    }
}