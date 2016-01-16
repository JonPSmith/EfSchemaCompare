// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataPublicPrivate.cs
// Date Created: 2016/01/16
// © Copyright Selective Analytics 2016. All rights reserved
// =====================================================

using System.Data.Entity.ModelConfiguration;

namespace EfPocoClasses.PublicPrivate
{
    public class DataPublicPrivateEf6
    {
        public int DataPublicPrivateEf6Id { get; set; }

        public int PublicInt { get; set; }
        public int PublicProtectedSetInt { get; protected set; }
        public int PublicPrivateSetInt { get; private set; }

        internal int InternalInt { get; set; }
        internal int InternalPrivateSetInt { get; private set; }

        protected int ProtectedInt { get; set; }
        protected int ProtectedPrivateSetInt { get; private set; }

        protected internal int ProtectedInternalInt { get; set; }
        protected internal int ProtectedInternalPrivateSetInt { get; private set; }

        private int PrivateInt { get; set; }

        //see http://blog.oneunicorn.com/2012/03/26/code-first-data-annotations-on-non-public-properties/
        public class DataPublicPrivateConfiguration : EntityTypeConfiguration<DataPublicPrivateEf6>
        {
            public DataPublicPrivateConfiguration()
            {
                Property(p => p.InternalInt);
                Property(p => p.InternalPrivateSetInt);
                Property(p => p.ProtectedInt);
                Property(p => p.ProtectedPrivateSetInt);
                Property(p => p.ProtectedInternalInt);
                Property(p => p.ProtectedInternalPrivateSetInt);
                Property(p => p.PrivateInt);
            }
        }
    }
}