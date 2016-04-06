#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestEf6DataTypesDb.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EfPocoClasses.DataTypes;

namespace Ef6TestDbContext
{
    public class TestEf6DataTypesDb : DbContext
    {
        public TestEf6DataTypesDb() : base("TestEf6SchemaCompareDb")
        {
        }

        public TestEf6DataTypesDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<DataIntDouble> DataIntDoubles { get; set; }
        public DbSet<DataStringByte> DataStringBytes { get; set; }
        public DbSet<DataDate> DataDates { get; set; }
        public DbSet<DataGuidEnum> DataGuidEnums { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<DataIntDouble>().Property(x => x.DataDecimalSmallPrecision).HasPrecision(5, 3);

            base.OnModelCreating(modelBuilder);
        }
    }
}