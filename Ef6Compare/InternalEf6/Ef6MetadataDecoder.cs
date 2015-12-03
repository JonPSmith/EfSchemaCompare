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

namespace Ef6Compare.InternalEf6
{
    public class Ef6MetadataDecoder
    {
        private readonly Assembly _dataClassesAssembly;

        public Ef6MetadataDecoder(Assembly dataClassesAssembly)
        {
            if (dataClassesAssembly == null) throw new ArgumentNullException("dataClassesAssembly");
            _dataClassesAssembly = dataClassesAssembly;
        }

        /// <summary>
        /// This returns information on all the Ef classes that are mapped to the database
        /// </summary>
        /// <returns></returns>
        public IList<EfTableInfo> GetAllEfTablesWithColInfo(DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

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
                var clrClassType = GetClrClassType(oSpaceEntity.ToString());

                int i = 1;
                var primaryKeys = metadata.GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == clrClassType).KeyProperties
                    .Select(x => new EfKeyOrder(x.Name, i++)).ToList();

                var propDecoder = new Ef6PropertyDecoder(mapping, tableEntitySet, GetClrClassType);
                var columnInfos = propDecoder.DecodeTableProperties(entitySet, clrClassType, primaryKeys);

                var relationshipInfos = (from navProperty in entitySet.ElementType.NavigationProperties
                                         let clrProperty = clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == navProperty.Name)
                                         let relationship = ConvertMetadataToFromToMultpicity(navProperty.FromEndMember, navProperty.ToEndMember)
                                         let columnArr = clrProperty.GetCustomAttribute<ColumnAttribute>()
                                         select new EfRelationshipInfo(relationship, clrProperty.Name, clrProperty.PropertyType)).ToList();

                result.Add(new EfTableInfo(tableName, tableSchema, clrClassType, columnInfos, relationshipInfos));
            }

            return result;
        }

        private Type GetClrClassType(string classFullName)
        {

            var clrClassType = _dataClassesAssembly.GetType(classFullName, false);
            if (clrClassType == null)
                throw new InvalidOperationException("Could not find the EF data class {0} in the assembly {1}." +
                                                    " If data classes are in a separate assembly to the DbContext then use the method with <T>");
            return clrClassType;
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