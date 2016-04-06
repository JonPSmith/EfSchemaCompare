#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataDate.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.DataTypes
{
    public class DataDate
    {
        public int DataDateId { get; set; }

        [Column(TypeName = "date")]
        public DateTime DataSqlDate { get; set; }

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