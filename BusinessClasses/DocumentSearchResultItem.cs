using System;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Result item for document search operations
/// </summary>
public class DocumentSearchResultItem
{
    public int AcquisitionDocumentID { get; set; }
    public int AcquisitionID { get; set; }
    public string? DocumentLocation { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
    
    // Acquisition Details
    public string? SellerName { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string? Counties { get; set; }
    public string? Operators { get; set; }
    public string? Buyers { get; set; }
}
