using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class Buyer
{
    [Key]
    public int BuyerID { get; set; }

    [Required, MaxLength(100)]
    public string BuyerName { get; set; } = null!;

    public bool DefaultBuyer { get; set; }

    [MaxLength(100)]
    public string? ContactName { get; set; }

    [MaxLength(100)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    [MaxLength(50)]
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

    public decimal? DefaultCommission { get; set; }

    // Navigation property - many-to-many relationship with Acquisitions through AcquisitionBuyer
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<AcquisitionBuyer> AcquisitionBuyers { get; set; } = new List<AcquisitionBuyer>();
    
    // Navigation property for contacts
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<BuyerContact> BuyerContacts { get; set; } = new List<BuyerContact>();
}
