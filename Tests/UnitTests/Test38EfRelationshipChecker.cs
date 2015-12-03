#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test34EfTableInfoRelationships.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompareCore.EFInfo;
using CompareCore.SqlInfo;
using Ef6Compare.InternalEf6;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.EfClasses.Relationships;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test38EfRelationshipChecker
    {

        private IList<EfTableInfo> _efInfos;
        private EfRelationshipChecker _checker;
            
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            using (var db = new EfSchemaCompareDb())
            {
                var decoder = new Ef6MetadataDecoder(Assembly.GetAssembly(typeof(EfSchemaCompareDb)));
                _efInfos = decoder.GetAllEfTablesWithColInfo(db);
                var allSqlInfo = SqlAllInfo.SqlAllInfoFactory(db.Database.Connection.ConnectionString);
                _checker = new EfRelationshipChecker(_efInfos, allSqlInfo, allSqlInfo.TableInfos);     //NOTE: we aren't able to filter potentialManyToManyTables
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
        public void Test11DataCompKeysOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "CompositeKeyData");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual(null);
        }

        [Test]
        public void Test12DataTopChildrenOk()
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
        public void Test13DataTopManyChildrenOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyChildren");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[NonStandardManyToManyTableName]");
        }

        [Test]
        public void Test14DataTopManyCompKeysOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyCompKeys");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[DataManyCompKeyDataTop]");
        }

        [Test]
        public void Test15DataTopZeroOrOneDataOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ZeroOrOneData");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual(null);
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
            status.Result.ShouldEqual("[dbo].[NonStandardManyToManyTableName]");
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
        public void Test50DataManyCompKeyManyToManyOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataManyCompKey));
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyParents");

            //EXECUTE
            var status = _checker.CheckEfRelationshipToSql(efInfo, refEfCol);

            //VERIFY
            status.ShouldBeValid();
            status.Result.ShouldEqual("[dbo].[DataManyCompKeyDataTop]");
        }



    }
}