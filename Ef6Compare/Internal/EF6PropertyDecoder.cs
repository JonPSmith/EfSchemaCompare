#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EF6PropertyDecoder.cs
// Date Created: 2015/11/17
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using CompareCore.EFInfo;

namespace Ef6Compare.Internal
{
    internal class Ef6PropertyDecoder
    {
        private readonly EntitySetMapping _mapping;
        private readonly EntitySet _tableEntitySet;

        public Ef6PropertyDecoder(EntitySetMapping mapping, EntitySet tableEntitySet)
        {
            _mapping = mapping;
            _tableEntitySet = tableEntitySet;
        }

        public List<EfColumnInfo> DecodeTableProperties(EntitySet entitySet, Type clrClassType, List<EfKeyOrder> primaryKeys)
        {
            var columnInfos = new List<EfColumnInfo>();
            foreach (var edmProperty in entitySet.ElementType.DeclaredProperties)
            {
                if (edmProperty.IsComplexType)
                {
                    var complexColumn = _mapping.EntityTypeMappings.Single()
                        .Fragments.Single()
                        .PropertyMappings.OfType<ComplexPropertyMapping>().Single(m => m.Property == edmProperty); 
                    columnInfos.AddRange(DecodeComplexTypes(edmProperty, complexColumn));          
                }
                else
                {
                    //normal column
                    var columnName = _mapping.EntityTypeMappings.Single()
                        .Fragments.Single()
                        .PropertyMappings.OfType<ScalarPropertyMapping>()
                        .Single(m => m.Property == edmProperty)
                        .Column.Name;
                    var sqlTypeName = _tableEntitySet.ElementType.DeclaredMembers
                        .Single(x => x.Name == columnName).TypeUsage.EdmType.Name;
                    var clrProperty =
                        clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Single(x => x.Name == edmProperty.Name);
                    var primaryKey = primaryKeys.SingleOrDefault(x => x.Name == edmProperty.Name);
                    columnInfos.Add(new EfColumnInfo(columnName, sqlTypeName, edmProperty.Nullable,
                        edmProperty.MaxLength, primaryKey, clrProperty));
                }
            }

            return columnInfos;

        }

        private static IEnumerable<EfColumnInfo> DecodeComplexTypes(EdmProperty edmProperty, ComplexPropertyMapping complexMapping, string prefix = null)
        {
            throw new NotImplementedException("We do not currently handle complex properties");
            var complexCols = new List<EfColumnInfo>();
            foreach (var property in complexMapping.TypeMappings.SelectMany(x => x.PropertyMappings))
            {
                    //var columnName = mapping.EntityTypeMappings.Single()
                    //        .Fragments.Single()
                    //        .PropertyMappings.OfType<ScalarPropertyMapping>()
                    //        .Single(m => m.property == edmProperty)
                    //        .Column.Name;
                    //var sqlTypeName = tableEntitySet.ElementType.DeclaredMembers
                    //    .Single(x => x.Name == columnName).TypeUsage.EdmType.Name;
                    //var clrProperty =
                    //    clrClassType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //        .Single(x => x.Name == edmProperty.Name);

            }
            return complexCols;
        }
    }
}