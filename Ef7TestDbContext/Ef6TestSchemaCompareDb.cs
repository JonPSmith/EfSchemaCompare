#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: TestEf7SchemaCompareDb.cs
// Date Created: 2015/12/17
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using CompareCore.Utils;
using EfPocoClasses.ClassTypes;
using EfPocoClasses.DataTypes;
using EfPocoClasses.Relationships;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;


namespace Ef7TestDbContext
{
    public class TestEf7SchemaCompareDb : DbContext
    {
        public const string EfDatabaseConfigName = "TestEf7SchemaCompareDb";

        private static readonly DbContextOptions DefaultOptions;

        /// <summary>
        /// This static ctor is used to setup the defaultOptions
        /// </summary>
        static TestEf7SchemaCompareDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(EfDatabaseConfigName.GetConnectionStringAndCheckValid());
            DefaultOptions = optionsBuilder.Options;
        }

        public TestEf7SchemaCompareDb() : base(DefaultOptions)
        {

        }

        public DbSet<DataTop> DataTops { get; set; }
        public DbSet<DataChild> DataChilds { get; set; }
        public DbSet<DataManyChildren> DataManyChildrens { get; set; }
        public DbSet<DataSingleton> DataSingletons { get; set; }
        public DbSet<DataCompKey> DataCompKeys { get; set; }
        public DbSet<DataComplex> DataComplexs { get; set; }
        public DbSet<DataIntDouble> DataIntDoubles { get; set; }
        public DbSet<DataStringByte> DataStringBytes { get; set; }
        public DbSet<DataDate> DataDates { get; set; }
        public DbSet<DataGuidEnum> DataGuidEnums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<DataTop>()
            //    .Property(x => x.SingletonNullable)
            //    .IsRequired(false);
            //    //.WillCascadeOnDelete(true);

            //modelBuilder.Entity<DataTop>()
            //    .Property(x => x.ZeroOrOneData)
            //    .IsRequired(false);
            //    //.WillCascadeOnDelete(true);

            modelBuilder.Entity<DataChild>()
                .HasIndex(t => t.MyInt);

            //modelBuilder.Entity<DataSingleton>()
            //    .HasRequired(x => x.Parent)
            //    .WithOptional(x => x.SingletonNullable)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DataTop>()
            //    .HasMany(r => r.ManyChildren)
            //    .
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("DataTopId");
            //        m.MapRightKey("DataManyChildrenId");
            //        m.ToTable("NonStandardManyToManyTableName");
            //    });

            //modelBuilder.Entity<DataIntDouble>().Property(x => x.DataDecimalSmallPrecision).HasPrecision(5, 3);

            base.OnModelCreating(modelBuilder);
        }
    }
}