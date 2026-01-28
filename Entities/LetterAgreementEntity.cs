using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class LetterAgreement
{
    [Key]
    public int LetterAgreementID { get; set; }
    public int? AcquisitionID { get; set; }
    public int? BankingDays { get; set; }
    [Column(TypeName = "money")] public decimal? TotalBonus { get; set; }
    [Column(TypeName = "money")] public decimal? ConsiderationFee { get; set; }
    public bool TakeConsiderationFromTotal { get; set; }
    public int? LandManID { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdatedOn { get; set; }
    public bool Referrals { get; set; }
    [Column(TypeName = "money")] public decimal? ReferralFee { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ReceiptDate { get; set; }
    
    // Calculated totals
    [Column(TypeName = "decimal(18,8)")] public decimal? TotalGrossAcres { get; set; }
    [Column(TypeName = "decimal(18,8)")] public decimal? TotalNetAcres { get; set; }
    [Column(TypeName = "money")] public decimal? TotalBonusAndFee { get; set; }

    // Additional properties needed by business logic (not in database)
    [NotMapped, MaxLength(200)]
    public string? LetterAgreementName { get; set; }

    [NotMapped]
    public decimal? TotalOfferAmount { get; set; }

    [NotMapped]
    public decimal? TotalBonusAcceptanceLetterAmount { get; set; }

    [NotMapped, MaxLength(50)]
    public string? StateName { get; set; }

    [NotMapped, MaxLength(2)]
    public string? StateCode { get; set; }

    [NotMapped, MaxLength(20)]
    public string? AcquisitionNumber { get; set; }

    [NotMapped, MaxLength(20)]
    public string? AssignmentNumber { get; set; }

    [NotMapped, MaxLength(100)]
    public string? ReferrerName { get; set; }

    [NotMapped, MaxLength(2000)]
    public string? DealNotes { get; set; }

    [NotMapped, MaxLength(1000)]
    public string? DealTabs { get; set; }

    // Navigation properties
    public virtual Acquisition? Acquisition { get; set; }
    public virtual User? LandMan { get; set; }
    public virtual ICollection<LetterAgreementUnit> LetterAgreementUnits { get; set; } = new List<LetterAgreementUnit>();
    public virtual ICollection<LetterAgreementNote> LetterAgreementNotes { get; set; } = new List<LetterAgreementNote>();
    public virtual ICollection<LetterAgreementSeller> LetterAgreementSellers { get; set; } = new List<LetterAgreementSeller>();
    public virtual ICollection<LetterAgreementReferrer> LetterAgreementReferrers { get; set; } = new List<LetterAgreementReferrer>();
    public virtual ICollection<LetterAgreementCounty> LetterAgreementCounties { get; set; } = new List<LetterAgreementCounty>();
    public virtual ICollection<LetterAgreementOperator> LetterAgreementOperators { get; set; } = new List<LetterAgreementOperator>();
    public virtual ICollection<LetterAgreementStatus> LetterAgreementStatuses { get; set; } = new List<LetterAgreementStatus>();
    public virtual ICollection<LetterAgreementChange> LetterAgreementChanges { get; set; } = new List<LetterAgreementChange>();
}

