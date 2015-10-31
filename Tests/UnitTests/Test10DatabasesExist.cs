#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Test10DatabasesExist.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using NUnit.Framework;
using Tests.Helpers;

namespace Tests.UnitTests
{
    public class Test10DatabasesExist
    {
        [Test]
        public void Test01EfCreatedDatabaseExists()
        {            
            //SETUP

            //EXECUTE
            DatabaseHelpers.EfWipeCreateDatabase();

            //VERIFY
        }

        [Test]
        public void Test10SqlCreatedDatabaseExists()
        {
            //SETUP

            //EXECUTE
            DatabaseHelpers.DbUpWipeCreateDatabase();

            //VERIFY
        } 
    }
}