#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestEf6ComplexDb.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EfPocoClasses.ComplexTypes;

namespace Ef6TestDbContext
{
    public class TestEf6ComplexDb : DbContext
    {
        public TestEf6ComplexDb() : base("TestEf6SchemaCompareDb")
        {
        }

        public TestEf6ComplexDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<DataComplex> DataComplexs { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}