namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Context for document generation that carries entity-specific IDs based on DocumentTypeCode.
/// Mirrors the legacy pattern where documents are associated with specific entities.
/// </summary>
public class DocumentGenerationContext
{
    /// <summary>
    /// Selected county ID for County document type.
    /// </summary>
    public int? CountyId { get; set; }

    /// <summary>
    /// Selected operator ID for Operator document type.
    /// </summary>
    public int? OperatorId { get; set; }

    /// <summary>
    /// Selected buyer ID for Buyer document type.
    /// </summary>
    public int? BuyerId { get; set; }

    /// <summary>
    /// Number of pages to record (used for County documents).
    /// </summary>
    public int PagesToRecord { get; set; } = 1;

    /// <summary>
    /// Custom field values from DocTemplateCustomField definitions.
    /// Key = CustomTag, Value = user-provided value.
    /// </summary>
    public Dictionary<string, string> CustomFields { get; set; } = new();

    /// <summary>
    /// Signing partner ID for Letter Agreement documents.
    /// </summary>
    public int? SigningPartnerId { get; set; }

    /// <summary>
    /// State code for Letter Agreement template selection (TX, LA, etc.).
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Document type code for barcode generation (e.g., "County", "Operator").
    /// </summary>
    public string? DocumentTypeCode { get; set; }

    /// <summary>
    /// Document description for barcode generation.
    /// </summary>
    public string? DocumentDescription { get; set; }
}
