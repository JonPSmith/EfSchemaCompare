#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestEf6PublicPrivateDb.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EfPocoClasses.PublicPrivate;

namespace Ef6TestDbContext
{
    public class TestEf6PublicPrivateDb : DbContext
    {
        public TestEf6PublicPrivateDb() : base("TestEf6SchemaCompareDb")
        {
        }

        public TestEf6PublicPrivateDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<DataPublicPrivate> DataPublicPrivates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Conventions.Add(new NonPublicColumnAttributeConvention());

            base.OnModelCreating(modelBuilder);
        }
    }
}