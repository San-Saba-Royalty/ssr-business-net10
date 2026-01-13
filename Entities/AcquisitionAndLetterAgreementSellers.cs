using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
}
