// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataPublicPrivate.cs
// Date Created: 2016/01/16
// © Copyright Selective Analytics 2016. All rights reserved
// =====================================================

using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.PublicPrivate
{
    public class DataPublicPrivate
    {
        public int DataPublicPrivateId { get; set; }

        
        public int PublicInt { get; set; }
        public int PublicProtectedSetInt { get; protected set; }
        public int PublicPrivateSetInt { get; private set; }

        //NOTE: we add the Column attribute on non-public properties that we want in the database.
        //This is particular to the EF6 solution which uses the NonPublicColumnAttributeConvention class

        [Column]
        internal int InternalInt { get; set; }
        [Column]
        internal int InternalPrivateSetInt { get; private set; }

        [Column]
        protected int ProtectedInt { get; set; }
        [Column]
        protected int ProtectedPrivateSetInt { get; private set; }

        [Column]
        protected internal int ProtectedInternalInt { get; set; }
        [Column]
        protected internal int ProtectedInternalPrivateSetInt { get; private set; }

        [Column]
        private int PrivateInt { get; set; }

        /// <summary>
        ///This private property is not included in EF database
        /// </summary>
        private int PrivateIntNotInEf { get; set; }

    }
}