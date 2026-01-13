using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSRBusiness.Entities;

public class DocumentType
{
    [Key, MaxLength(20)]
    public string DocumentTypeCode { get; set; } = null!;

    [Required, MaxLength(500)]
    public string DocumentTypeDesc { get; set; } = null!;

    [MaxLength(20)]
    public string? DSCollectionID { get; set; }
}


public class CurativeType
{
    [Key]
    public int CurativeTypeID { get; set; }

    [Required, MaxLength(100)]
    public string CurativeTypeName { get; set; } = null!;
}
