#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfSchemaCompareDb.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Tests.EfClasses
{
    public class EfSchemaCompareDb : DbContext
    {
        public EfSchemaCompareDb()
        {
        }

        public EfSchemaCompareDb(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }


        public DbSet<DataTop> DataTops { get; set; }
        public DbSet<DataChild> DataChilds { get; set; }
        public DbSet<DataManyChildren> DataManyChildrens { get; set; }
        public DbSet<DataSingleton> DataSingletons { get; set; }
        public DbSet<DataCompKey> DataCompKeys { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<DataTop>()
                .HasOptional(x => x.SingletonNullable)
                .WithOptionalPrincipal()
                .WillCascadeOnDelete(false);


            //modelBuilder.Entity<DataSingleton>()
            //    .HasRequired(x => x.OnlyParent)
            //    .WithOptional(x => x.SingletonNullable)
            //    .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}