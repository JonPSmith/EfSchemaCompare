#region licence
// =====================================================
// EfSchemeCompare Project - project-to-compare EF schema-to-SQL schema
// Filename: Test30EfTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompareCore.EFInfo;
using Ef6Compare.Internal;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test32EfRelationships
    {
        private ICollection<EfTableInfo> _efInfos;
            
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            using (var db = new EfSchemaCompareDb())
            {
                var decoder = new Ef6MetadataDecoder(Assembly.GetAssembly(typeof(EfSchemaCompareDb)));
                _efInfos = decoder.GetAllEfTablesWithColInfo(db);
            }
        }


        private Type GetClassFromCollection(EfRelationshipInfo relEfCol)
        {
            if (!relEfCol.ClrColumnType.IsGenericType)
                throw new InvalidOperationException("I expected a generic list etc. here");
            var genArgs = relEfCol.ClrColumnType.GetGenericArguments();
            if (genArgs.Length != 1)
                throw new InvalidOperationException("I expect only one class");

            return genArgs[0];
        }

        [Test]
        public void Test01GetEfTableColumnInfo()
        {
            //SETUP

            //EXECUTE

            //VERIFY
            _efInfos.Count.ShouldEqual(7);
 
        }

        [Test]
        public void Test10DataTopSingletonNullableOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "SingletonNullable");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("One-to-ZeroOrOne");
            refEfCol.ClrColumnType.ShouldEqual(typeof(DataSingleton));
        }

        [Test]
        public void Test11DataTopChildrenOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "Children");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("One-to-Many");
            GetClassFromCollection(refEfCol).ShouldEqual(typeof(DataChild));
        }


        [Test]
        public void Test12DataTopManyChildrenOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyChildren");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("Many-to-Many");
            GetClassFromCollection(refEfCol).ShouldEqual(typeof(DataManyChildren));
        }

        [Test]
        public void Test13DataTopManyCompKeysOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataTop));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyCompKeys");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("Many-to-Many");
            GetClassFromCollection(refEfCol).ShouldEqual(typeof(DataManyCompKey));
        }



        [Test]
        public void Test20DataChildNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataChild));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "Parent");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("Many-to-One");
            refEfCol.ClrColumnType.ShouldEqual(typeof(DataTop));   
        }

        [Test]
        public void Test30DataManyChildrenNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataManyChildren));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyParents");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("Many-to-Many");
            GetClassFromCollection(refEfCol).ShouldEqual(typeof(DataTop));
        }

        [Test]
        public void Test40DataSingletonParentOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataSingleton));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "Parent");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("ZeroOrOne-to-One");
            refEfCol.ClrColumnType.ShouldEqual(typeof(DataTop));  
        }

        [Test]
        public void Test50DataCompKeyNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataCompKey));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyParents");

            //VERIFY
            refEfCol.ShouldEqual(null);
        }

        [Test]
        public void Test60DataManyCompKeyNormalColsOk()
        {
            //SETUP
            var efInfo = _efInfos.SingleOrDefault(x => x.ClrClassType == typeof(DataManyCompKey));

            //EXECUTE
            var refEfCol = efInfo.RelationshipCols.SingleOrDefault(x => x.ClrColumnName == "ManyParents");

            //VERIFY
            refEfCol.ShouldNotEqualNull();
            refEfCol.FromToRelationships.ToString().ShouldEqual("Many-to-Many");
            GetClassFromCollection(refEfCol).ShouldEqual(typeof(DataTop));
        }

    }
}