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
        public DbSet<DataComplex> DataComplexs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<DataTop>()
                .HasOptional(x => x.SingletonNullable)
                .WithRequired(x => x.Parent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataTop>()
                .HasOptional(x => x.ZeroOrOneData)
                .WithRequired()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataSingleton>()
                .HasRequired(x => x.Parent)
                .WithOptional(x => x.SingletonNullable)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataTop>()
                .HasMany(r => r.ManyChildren)
                .WithMany(r => r.ManyParents)
                .Map(m =>
                {
                    m.MapLeftKey("DataTopId");
                    m.MapRightKey("DataManyChildrenId");
                    m.ToTable("NonStandardManyToManyTableName");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}