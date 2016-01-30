#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestEf6ComplexDb.cs
// Date Created: 2016/01/30
// © Copyright Selective Analytics 2015. All rights reserved
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