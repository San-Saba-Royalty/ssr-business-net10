using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class LookupField
{
    [Key]
    public int FieldID { get; set; }

    [Required, MaxLength(50)]
    public string FieldName { get; set; } = null!;

    [Required, MaxLength(5)]
    public string DataTypeCode { get; set; } = null!;

    [ForeignKey(nameof(DataTypeCode))]
    public virtual LookupFieldDataType? LookupFieldDataType { get; set; }
}

public class LookupFieldDataType
{
    [Key, MaxLength(5)]
    public string DataTypeCode { get; set; } = null!;

    [Required, MaxLength(20)]
    public string DataTypeDesc { get; set; } = null!;
}

public class ComparisonType
{
    [Key]
    public int ComparisonTypeID { get; set; }

    [Required, MaxLength(50)]
    public string ComparisonDesc { get; set; } = null!;
}

[Table("ComparisonLookupXref")]
public class ComparisonLookupXref
{
    [Key]
    public int ComparisonLookupID { get; set; }

    [Required, MaxLength(5)]
    public string DataTypeCode { get; set; } = null!;

    public int ComparisonTypeID { get; set; }

    [ForeignKey(nameof(DataTypeCode))]
    public virtual LookupFieldDataType DataType { get; set; } = null!;

    [ForeignKey(nameof(ComparisonTypeID))]
    public virtual ComparisonType ComparisonType { get; set; } = null!;
}
