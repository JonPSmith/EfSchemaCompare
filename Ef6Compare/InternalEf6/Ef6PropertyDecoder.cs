﻿#region licence
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

namespace Ef6Compare.InternalEf6
{
    internal class Ef6PropertyDecoder
    {
        private readonly EntitySetMapping _mapping;
        private readonly EntitySet _tableEntitySet;
        private readonly Func<string, Type> _getClrTypeFromFullName;

        public Ef6PropertyDecoder(EntitySetMapping mapping, EntitySet tableEntitySet, Func<string,Type> getClrTypeFromFullName)
        {
            _mapping = mapping;
            _tableEntitySet = tableEntitySet;
            _getClrTypeFromFullName = getClrTypeFromFullName;
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
                    columnInfos.AddRange(DecodeComplexTypes(complexColumn, clrClassType));          
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
                    var clrProperty = Ef6MetadataDecoder.GetPublicAndPrivatePropertyByName(clrClassType, edmProperty.Name);
                    var primaryKey = primaryKeys.SingleOrDefault(x => x.Name == edmProperty.Name);
                    columnInfos.Add(new EfColumnInfo(columnName, sqlTypeName, edmProperty.Nullable,
                         edmProperty.MaxLength, edmProperty.Precision, primaryKey, clrProperty));
                }
            }

            return columnInfos;

        }

        private IEnumerable<EfColumnInfo> DecodeComplexTypes(ComplexPropertyMapping complexMapping, Type parentClass)
        {
            //throw new NotImplementedException("We do not currently handle complex properties");
            var complexCols = new List<EfColumnInfo>();
            foreach (var property in complexMapping.TypeMappings.SelectMany(x => x.PropertyMappings))
            {
                var complexClrType =
                    Ef6MetadataDecoder.GetPublicAndPrivatePropertyByName(parentClass, complexMapping.Property.Name)
                        .PropertyType;
                if (property.Property.IsComplexType)
                {
                    complexCols.AddRange(DecodeComplexTypes((ComplexPropertyMapping)property, complexClrType));
                }
                else
                {
                    var columnName = ((ScalarPropertyMapping)property).Column.Name;
                    var sqlTypeName = _tableEntitySet.ElementType.DeclaredMembers
                        .Single(x => x.Name == columnName).TypeUsage.EdmType.Name;
                    var clrProperty = Ef6MetadataDecoder.GetPublicAndPrivatePropertyByName(complexClrType, property.Property.Name);
                    complexCols.Add(new EfColumnInfo(columnName, sqlTypeName, property.Property.Nullable,
                        property.Property.MaxLength, property.Property.Precision, null, clrProperty));                 
                }
            }
            return complexCols;
        }
    }
}