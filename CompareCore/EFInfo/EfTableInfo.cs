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
using System.Runtime.CompilerServices;
using CompareCore.Utils;

namespace CompareCore.EFInfo
{
    public class EfTableInfo
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



    }
}