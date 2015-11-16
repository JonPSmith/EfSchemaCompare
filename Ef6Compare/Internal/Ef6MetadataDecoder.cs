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
        /// <param name="dataClassesAssembly">If non-null then holds the assembly of the data classes, 
        /// otherwise we assum they are in the same assembly as DbContext</param>
        /// <returns></returns>
        public static ICollection<EfTableInfo> GetAllEfTablesWithColInfo(DbContext context, Assembly dataClassesAssembly)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));
            var efClassesAssembly = dataClassesAssembly ?? Assembly.GetAssembly(context.GetType());

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
                var clrClassType = efClassesAssembly.GetType(oSpaceEntity.ToString(), false);
                if (clrClassType == null)
                    throw new InvalidOperationException("Could not find the EF data class {0} in the assembly {1}."+
                        " If data classes are in a separate assembly to the DbContext then use the method with <T>");

                int i = 1;
                var primaryKeys = metadata.GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == clrClassType).KeyProperties
                    .Select(x => new EfKeyOrder(x.Name, i++)).ToList();

                var columnInfos = new List<EfColumnInfo>();
                foreach (var edmProperty in entitySet.ElementType.DeclaredProperties)
                {
                    var columnName = mapping.EntityTypeMappings.Single()
                        .Fragments.Single()
                        .PropertyMappings.OfType<ScalarPropertyMapping>()
                        .Single(m => m.Property == edmProperty)
                        .Column.Name;
                    var sqlTypeName = tableEntitySet.ElementType.DeclaredMembers
                        .Single(x => x.Name == columnName).TypeUsage.EdmType.Name;
                    var clrProperty =
                        clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Single(x => x.Name == edmProperty.Name);
                    var primaryKey = primaryKeys.SingleOrDefault(x => x.Name == edmProperty.Name);
                    columnInfos.Add(new EfColumnInfo(columnName, sqlTypeName, edmProperty.Nullable,
                        edmProperty.MaxLength, primaryKey, clrProperty));
                }

                //var columnInfos = (from edmProperty in entitySet.ElementType.DeclaredProperties
                //                   let columnName = mapping.EntityTypeMappings.Single()
                //                       .Fragments.Single()
                //                       .PropertyMappings.OfType<ScalarPropertyMapping>()
                //                       .Single(m => m.Property == edmProperty)
                //                       .Column.Name
                //                   let sqlTypeName = tableEntitySet.ElementType.DeclaredMembers
                //                       .Single(x => x.Name == columnName).TypeUsage.EdmType.Name
                //                   let clrProperty = clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == edmProperty.Name)
                //                   let primaryKey = primaryKeys.SingleOrDefault(x => x.Name == edmProperty.Name)
                //                   select new EfColumnInfo(columnName, sqlTypeName, edmProperty.Nullable, edmProperty.MaxLength, primaryKey, clrProperty)).ToList();


                var relationshipInfos = (from navProperty in entitySet.ElementType.NavigationProperties
                                         let clrProperty = clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == navProperty.Name)
                                         let relationship = ConvertMetadataToFromToMultpicity(navProperty.FromEndMember, navProperty.ToEndMember)
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
        /// <param name="fromRelationship"></param>
        /// <param name="toRelationship"></param>
        /// <returns></returns>
        private static FromToRelationship ConvertMetadataToFromToMultpicity(RelationshipEndMember fromRelationship, RelationshipEndMember toRelationship)
        {
            var efFromType = (EfRelationshipTypes)Enum.Parse(typeof(EfRelationshipTypes), fromRelationship.RelationshipMultiplicity.ToString());
            var fromCascade = fromRelationship.DeleteBehavior == OperationAction.Cascade;
            var efToType = (EfRelationshipTypes)Enum.Parse(typeof(EfRelationshipTypes), toRelationship.RelationshipMultiplicity.ToString());
            var toCascade = toRelationship.DeleteBehavior == OperationAction.Cascade;
            return new FromToRelationship(efFromType, fromCascade, efToType, toCascade);
        } 
    }
}