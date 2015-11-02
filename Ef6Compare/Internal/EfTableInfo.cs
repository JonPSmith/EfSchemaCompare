#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfTableInfo.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CompareCore.EFInfo;
using CompareCore.Utils;

[assembly: InternalsVisibleTo("Tests")]

namespace Ef6Compare.Internal
{
    internal class EfTableInfo
    {
        public string TableName { get; set; }

        public string SchemaName { get; set; }

        public string CombinedName { get { return FormatHelpers.FormCombinedSchemaTableName(SchemaName, TableName); } }

        public Type ClrClassType { get; set; }

        public ICollection<EfColumnInfo> NormalCols { get; set; }

        public ICollection<EfRelationshipInfo> RelationshipCols { get; set; }

        public EfTableInfo(string tableName, string schemaName, Type clrClassType, ICollection<EfColumnInfo> normalCols, ICollection<EfRelationshipInfo> relationshipCols)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ClrClassType = clrClassType;
            NormalCols = normalCols;
            RelationshipCols = relationshipCols;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}.{1}, NormalCols: {2}, Relationships: {3}", SchemaName, TableName, NormalCols.Count, RelationshipCols.Count);
        }

        //----------------------------------------------------

        /// <summary>
        /// This returns information on all the Ef classes that are mapped to the database
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ICollection<EfTableInfo> GetAllEfTablesWithColInfo(DbContext context)
        {
            var metadata = ((IObjectContextAdapter) context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection) metadata.GetItemCollection(DataSpace.OSpace));
            var efClassesAssembly = Assembly.GetAssembly(context.GetType());

            // Get all the classes included in this Ef Context
            var allEfClasses = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets;

            var result = new List<EfTableInfo>();

            foreach (var entitySet in allEfClasses)
            {
                // Find the mapping between conceptual and storage model for this entity set
                var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

                // Find the storage entity set (table) that the entity is mapped
                var tableEntitySet = mapping
                    .EntityTypeMappings.Single()
                    .Fragments.Single()
                    .StoreEntitySet;

                // Return the table name from the storage entity set
                var tableName = (string)(tableEntitySet.MetadataProperties["Table"].Value ?? tableEntitySet.Name);
                var tableSchema = tableEntitySet.MetadataProperties["Schema"].Value.ToString();
                var oSpaceEntity = objectItemCollection.Single(x => x.ToString().EndsWith("." + entitySet.ElementType.Name));
                var clrClassType = efClassesAssembly.GetType(oSpaceEntity.ToString(), true);

                int i = 1;
                var primaryKeys = metadata.GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == clrClassType).KeyProperties
                    .Select(x => new EfKeyOrder(x.Name, i++)).ToList();

                var columnInfos = (from edmProperty in entitySet.ElementType.DeclaredProperties
                                   let columnName = mapping.EntityTypeMappings.Single()
                                       .Fragments.Single()
                                       .PropertyMappings.OfType<ScalarPropertyMapping>()
                                       .Single(m => m.Property == edmProperty)
                                       .Column.Name
                                   let clrProperty = clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == edmProperty.Name)
                                   let primaryKey = primaryKeys.SingleOrDefault(x => x.Name == edmProperty.Name)
                                   select new EfColumnInfo(columnName, edmProperty.Nullable, edmProperty.MaxLength, primaryKey, clrProperty)).ToList();

                var relationshipInfos = (from navProperty in entitySet.ElementType.NavigationProperties
                                         let clrProperty = clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == navProperty.Name)
                                         select new EfRelationshipInfo(navProperty, clrProperty)).ToList();

                result.Add(new EfTableInfo(tableName, tableSchema, clrClassType, columnInfos, relationshipInfos));
            }

            return result;
        }
    }
}