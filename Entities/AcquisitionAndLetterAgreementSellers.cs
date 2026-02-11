using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class AcquisitionSeller
{
    [Key]
    public int AcquisitionSellerID { get; set; }
    public int AcquisitionID { get; set; }
    
    [MaxLength(30)]
    public string? SellerLastName { get; set; }
    
    [MaxLength(100)]
    public string? SellerName { get; set; }
    
    [MaxLength(100)]
    public string? AddressLine1 { get; set; }
    
    [MaxLength(100)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? AddressLine3 { get; set; }
    
    [MaxLength(50)]
    public string? ContactEmail { get; set; }
    
    [MaxLength(24)]
    public string? ContactPhone { get; set; }

    [MaxLength(24)]
    public string? ContactFax { get; set; }
    
    [MaxLength(50)]
    public string? City { get; set; }
    
    [MaxLength(2)]
    public string? StateCode { get; set; }
    
    [MaxLength(10)]
    public string? ZipCode { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdatedOn { get; set; }

    // Additional properties needed by business logic (not in database)
    [NotMapped, MaxLength(500)]
    public string? ForeignAddress { get; set; }

    [NotMapped, MaxLength(100)]
    public string? DeceasedName { get; set; }

    [NotMapped, MaxLength(100)]
    public string? SpouseName { get; set; }

    [NotMapped, MaxLength(15)]
    public string? SSN { get; set; }

    [NotMapped, MaxLength(20)]
    public string? MaritalStatus { get; set; }

    [NotMapped, MaxLength(50)]
    public string? OwnershipType { get; set; }

    [NotMapped, MaxLength(100)]
    public string? VestingName { get; set; }

    [ForeignKey("AcquisitionID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
}

public class LetterAgreementSeller
{
    [Key]
    public int LetterAgreementSellerID { get; set; }
    public int LetterAgreementID { get; set; }
    public bool CompanyIndicator { get; set; }
    
    [MaxLength(30)]
    public string? SellerLastName { get; set; }
    
    [MaxLength(100)]
    public string? SellerName { get; set; }
    
    [MaxLength(100)]
    public string? AddressLine1 { get; set; }
    
    [MaxLength(100)]
    public string? AddressLine2 { get; set; }
    
    [MaxLength(50)]
    public string? City { get; set; }
    
    [MaxLength(2)]
    public string? StateCode { get; set; }
    
    [MaxLength(10)]
    public string? ZipCode { get; set; }
    
    [MaxLength(24)]
    public string? ContactPhone { get; set; }
    
    [MaxLength(24)]
    public string? ContactFax { get; set; }
    
    [MaxLength(50)]
    public string? ContactEmail { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdatedOn { get; set; }

    [ForeignKey("LetterAgreementID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
}
