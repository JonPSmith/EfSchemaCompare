#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataDate.cs
// Date Created: 2015/12/01
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.EfClasses.DataTypes
{
    public class DataDate
    {
        public int DataDateId { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataDateTimeDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DataDateTime { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DataDateTime2 { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime DataSmallDateTime { get; set; }

        public DateTimeOffset DataDateTimeOffset { get; set; }

        public TimeSpan DataTimeSpan { get; set; }

    }
}