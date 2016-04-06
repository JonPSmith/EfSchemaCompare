#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: EfTableInfo.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompareCore.Utils;

[assembly: InternalsVisibleTo("Tests")]

namespace CompareCore.EFInfo
{
    public class EfTableInfo
    {
        internal EfTableInfo()
        {
        }

        public EfTableInfo(string tableName, string schemaName, Type clrClassType, IList<EfColumnInfo> normalCols, IList<EfRelationshipInfo> relationshipCols)
        {
            TableName = tableName;
            SchemaName = schemaName;
            ClrClassType = clrClassType;
            NormalCols = normalCols;
            RelationshipCols = relationshipCols;
        }

        public string SchemaName { get; private set; }

        public string TableName { get; private set; }

        public string CombinedName { get { return FormatHelpers.FormCombinedSchemaTableName(SchemaName, TableName); } }

        public Type ClrClassType { get; private set; }

        public IList<EfColumnInfo> NormalCols { get; private set; }

        public IList<EfRelationshipInfo> RelationshipCols { get; private set; }

        public override string ToString()
        {
            return string.Format("Name: {0}.{1}, NormalCols: {2}, Relationships: {3}", SchemaName, TableName, NormalCols.Count, RelationshipCols.Count);
        }

        //----------------------------------------------------
    }
}