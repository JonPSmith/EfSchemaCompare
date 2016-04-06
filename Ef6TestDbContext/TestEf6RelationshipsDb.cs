#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestEf6RelationshipsDb.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using EfPocoClasses.Relationships;

namespace Ef6TestDbContext
{
    public class TestEf6RelationshipsDb : DbContext
    {
        public TestEf6RelationshipsDb() : base("TestEf6SchemaCompareDb")
        {
        }

        public TestEf6RelationshipsDb(string nameOrConnectionString) : base(nameOrConnectionString)
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
                .WithRequired(x => x.Parent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DataTop>()
                .HasOptional(x => x.ZeroOrOneData)
                .WithRequired()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataChild>()
                .Property(t => t.MyInt)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));

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