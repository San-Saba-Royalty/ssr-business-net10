using System.ComponentModel.DataAnnotations;

namespace SSRBusiness.Entities;

public class AppraisalGroup
{
    [Key]
    public int AppraisalGroupID { get; set; }
    
    [Required, MaxLength(50)]
    public string AppraisalGroupName { get; set; } = null!;
    
    [MaxLength(50)]
    public string? ContactName { get; set; }

    [MaxLength(50)]
    public string? ContactEmail { get; set; }

    [MaxLength(24)]
    public string? ContactPhone { get; set; }

    [MaxLength(24)]
    public string? ContactFax { get; set; }

    [MaxLength(50)]
    public string? Attention { get; set; }

    [MaxLength(100)]
    public string? AddressLine1 { get; set; }

    [MaxLength(100)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? AddressLine3 { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }

    [MaxLength(2)]
    public string? StateCode { get; set; }

    [MaxLength(10)]
    public string? ZipCode { get; set; }
}
