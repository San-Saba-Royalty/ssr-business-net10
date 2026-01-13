using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

/// <summary>
/// Represents a report definition
/// </summary>
[Table("Report")]
public class Report
{
    [Key]
    public int ReportID { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string? ReportName { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Report file name or identifier (e.g., "AcquisitionSummary.rdlc")
    /// </summary>
    [MaxLength(255)]
    public string? ReportFile { get; set; }
    
    /// <summary>
    /// Category for grouping reports
    /// </summary>
    [MaxLength(50)]
    public string? Category { get; set; }
    
    /// <summary>
    /// Display order within category
    /// </summary>
    public int DisplayOrder { get; set; }
    
    #region Filter Visibility Flags
    
    public bool ShowCountyFilter { get; set; } = true;
    public bool ShowOperatorFilter { get; set; } = true;
    public bool ShowBuyerFilter { get; set; } = true;
    public bool ShowDealStatusFilter { get; set; } = true;
    public bool ShowLandmanFilter { get; set; } = true;
    public bool ShowFieldLandmanFilter { get; set; } = true;
    
    public bool ShowEffectiveDateFilter { get; set; } = true;
    public bool ShowBuyerEffectiveDateFilter { get; set; }
    public bool ShowDueDateFilter { get; set; }
    public bool ShowPaidDateFilter { get; set; }
    public bool ShowTitleOpinionDateFilter { get; set; }
    public bool ShowClosingLetterDateFilter { get; set; }
    public bool ShowDeedDateFilter { get; set; }
    public bool ShowInvoiceDateFilter { get; set; }
    public bool ShowInvoiceDueDateFilter { get; set; }
    public bool ShowInvoicePaidDateFilter { get; set; }
    
    public bool ShowInvoiceNumberFilter { get; set; }
    public bool ShowReferralFilter { get; set; }
    public bool ShowCurativeFilter { get; set; }
    public bool ShowLienFilter { get; set; }
    public bool ShowLetterAgreementFilter { get; set; }
    
    #endregion
    
    #region Report-Specific Options
    
    public bool ShowSortByOption { get; set; }
    public bool ShowSubtotalByOption { get; set; }
    public bool ShowSummaryLevelOption { get; set; }
    public bool ShowIncludeNewAcquisitionsOption { get; set; }
    public bool ShowIncludeAmountPaidOption { get; set; }
    public bool ShowIncludeNotesOption { get; set; }
    
    #endregion
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? CreatedOn { get; set; }
    
    public DateTime? ModifiedOn { get; set; }
}