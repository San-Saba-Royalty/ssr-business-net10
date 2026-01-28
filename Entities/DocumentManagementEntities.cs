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

/// <summary>
/// Request for generating acquisition barcode cover sheets
/// </summary>
public class BarcodeDocumentRequest
{
    [Required]
    public int AcquisitionID { get; set; }

    [Required, MaxLength(20)]
    public string DocumentTypeCode { get; set; } = null!;

    [Required, MaxLength(500)]
    public string DocumentDescription { get; set; } = null!;

    [Required, Range(1, 100)]
    public int NumberCopies { get; set; } = 1;
}
