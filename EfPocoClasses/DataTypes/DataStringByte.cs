#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: DataStringByte.cs
// Date Created: 2016/04/06
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfPocoClasses.DataTypes
{
    public class DataStringByte
    {
        public int DataStringByteId { get; set; }

        [Column(TypeName = "nchar")]
        public char DataChar { get; set; }

        public string DataStringNvarchar { get; set; }

        [MaxLength(25)]
        public string DataStringNvarchar25 { get; set; }

        [Column(TypeName = "nchar")]
        [MaxLength(25)]
        public string DataStringNchar25Fixed { get; set; }

        [MaxLength(25)]
        [Required(AllowEmptyStrings = true)]
        public string DataStringNvarchar25NotNull { get; set; }

        [Column(TypeName = "varchar")]
        public string DataStringVarchar { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(25)]
        public string DataStringVarchar25 { get; set; }

        [Column(TypeName = "char")]
        [MaxLength(25)]
        public string DataStringChar25Fixed { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(25)]
        [Required(AllowEmptyStrings = true)]
        public string DataStringVarchar25NotNull { get; set; }


        [Column(TypeName = "binary")]
        [MaxLength(25)]
        public byte[] DataByteBinary25 { get; set; }

        [Column(TypeName = "varbinary")]
        public byte[] DataByteVarbinary { get; set; }

        [Column(TypeName = "varbinary")]
        [MaxLength(25)]
        public byte[] DataByteVarbinary25 { get; set; }

        [Column(TypeName = "varbinary")]
        [MaxLength(25)]
        [Required]
        public byte[] DataByteVarbinary25NotNull { get; set; }
    }
}