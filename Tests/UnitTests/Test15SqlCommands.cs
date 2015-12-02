#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test15SqlCommands.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Configuration;
using CompareCore.SqlInfo;
using NUnit.Framework;
using Tests.EfClasses;
using Tests.EfClasses.Relationships;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test15SqlCommands
    {

        [Test]
        public void Test01SqlTableAndColumnDataOk()
        {
            //SETUP
            var connection = ConfigurationManager.ConnectionStrings[MiscConstants.EfDatabaseConfigName].ConnectionString;

            //EXECUTE
            var result = SqlTableAndColumnData.GetSqlTablesAndColumns(connection);

            //VERIFY
            result.Count.ShouldBeGreaterThan(3);
        }

        [Test]
        public void Test10GetForeignKeysOk()
        {
            //SETUP
            var connection = ConfigurationManager.ConnectionStrings[MiscConstants.EfDatabaseConfigName].ConnectionString;

            //EXECUTE
            var result = SqlForeignKeys.GetForeignKeys(connection);

            //VERIFY
            result.Count.ShouldBeGreaterThan(3);
        }

        [Test]
        public void Test20GetPrimaryKeysNamesOk()
        {
            //SETUP
            var connection = ConfigurationManager.ConnectionStrings[MiscConstants.EfDatabaseConfigName].ConnectionString;

            //EXECUTE
            var result = SqlPrimaryKey.GetPrimaryKeysNames(connection, typeof(DataManyCompKey).Name);

            //VERIFY
            result.Count.ShouldEqual(2);
        }

        [Test]
        public void Test30GetNonPrimaryKeyIndexesOk()
        {
            //SETUP
            var connection = ConfigurationManager.ConnectionStrings[MiscConstants.EfDatabaseConfigName].ConnectionString;

            //EXECUTE
            var result = SqlIndexes.GetNonPrimaryKeyIndexes(connection);

            //VERIFY
            result.Count.ShouldBeGreaterThan(8);
        }  
    
    }
}