#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: Ef6MetadataDecoder.cs
// Date Created: 2015/11/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using CompareCore.EFInfo;

namespace Ef6Compare.Internal
{
    public static class Ef6MetadataDecoder
    {
        /// <summary>
        /// This returns information on all the Ef classes that are mapped to the database
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ICollection<EfTableInfo> GetAllEfTablesWithColInfo(DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));
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
                                         let relationship = ConvertMetadataToInternalTypes
                                                (navProperty.FromEndMember.RelationshipMultiplicity, navProperty.ToEndMember.RelationshipMultiplicity)
                                         let columnArr = clrProperty.GetCustomAttribute<ColumnAttribute>()
                                         select new EfRelationshipInfo(relationship, clrProperty, columnArr == null ? clrProperty.Name : columnArr.Name)).ToList();

                result.Add(new EfTableInfo(tableName, tableSchema, clrClassType, columnInfos, relationshipInfos));
            }

            return result;
        }

        //--------------------------------------------------------------------------
        //private method

        /// <summary>
        /// We convert the EF6 metadata RelationshipMultiplicity to an local copy to insulate against changes in the metadata format
        /// </summary>
        /// <param name="metaDataFrom"></param>
        /// <param name="metaDataTo"></param>
        /// <returns></returns>
        private static FromToMultiplicity ConvertMetadataToInternalTypes(RelationshipMultiplicity metaDataFrom, RelationshipMultiplicity metaDataTo)
        {
            var efFrom = (EfRelationshipTypes)Enum.Parse(typeof(EfRelationshipTypes), metaDataFrom.ToString());
            var efTo = (EfRelationshipTypes)Enum.Parse(typeof(EfRelationshipTypes), metaDataTo.ToString());
            return new FromToMultiplicity(efFrom, efTo);
        } 
    }
}