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
using CompareCore.SqlInfo;
using Ef6Compare.Internal;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test35EfRelationshipChecker
    {

        private ICollection<EfTableInfo> _efInfos;
        private EfRelationshipChecker _checker;
            
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            using (var db = new EfSchemaCompareDb())
            {
                _efInfos = EfTableInfo.GetAllEfTablesWithColInfo(db);
                var sqlInfos = SqlTableInfo.GetAllSqlTablesWithColInfo(db.Database.Connection.ConnectionString);
                _checker = new EfRelationshipChecker(_efInfos, sqlInfos);
            }
        }

        [Test]
        public void Test01GetEfTableColumnInfo()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _efInfos.Count.ShouldEqual(5);
 
        }

        [Test]
        public void Test10DataTopSingletonNullableOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "SingletonNullable");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Test11DataTopChildrenOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "Children");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual(null);
        }


        [Test]
        public void Test12DataTopManyChildrenOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyChildren");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[DataManyChildrenDataTop]");
        }

        [Test]
        public void Test13DataTopManyCompKeysOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyCompKeys");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[DataCompKeyDataTop]");
        }



        [Test]
        public void Test20DataChildNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataChild));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "Parent");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Test30DataManyChildrenNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataManyChildren));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyParents");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[DataManyChildrenDataTop]");
        }

        [Test]
        public void Test40DataSingletonParentOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataSingleton));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "Parent");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Test50DataCompKeyNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataCompKey));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyParents");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[DataCompKeyDataTop]");
        }

    }
}