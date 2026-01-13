using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class Acquisition
{
    [Key]
    public int AcquisitionID { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? BuyerEffectiveDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? TotalBonus { get; set; }

    [Column(TypeName = "money")]
    public decimal? DraftFee { get; set; }

    [Column(TypeName = "money")]
    public decimal? TotalBonusAndFee { get; set; }

    [MaxLength(50)]
    public string? DraftCheckNumber { get; set; }
    public DateTime? LiensLetterSent { get; set; }

    [Column(TypeName = "money")]
    public decimal? LienAmount { get; set; }

    [MaxLength(50)]
    public string? TitleOpinion { get; set; }

    [MaxLength(500)]
    public string? ClosingStatus { get; set; }

    [MaxLength(50)]
    public string? Buyer { get; set; }

    [MaxLength(20)]
    public string? AcquisitionNumber { get; set; }

    [MaxLength(50)]
    public string? Assignee { get; set; }
    public DateTime? ClosingLetterDate { get; set; }
    public DateTime? DeedDate { get; set; }

    [MaxLength(20)]
    public string? InvoiceNumber { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? InvoiceDueDate { get; set; }
    public DateTime? InvoicePaidDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? InvoiceTotal { get; set; }

    [Column(TypeName = "money")]
    public decimal? Commission { get; set; }
    public int? LandManID { get; set; }

    [MaxLength(20)]
    public string? SsrInPay { get; set; }

    [MaxLength(50)]
    public string? CheckStubDesc { get; set; }

    [Required, MaxLength(5)]
    public string HaveCheckStub { get; set; } = "No";

    [Column(TypeName = "decimal(13,8)")]
    public decimal? TotalGrossAcres { get; set; }

    [Column(TypeName = "decimal(13,8)")]
    public decimal? TotalNetAcres { get; set; }
    public bool FieldCheck { get; set; }
    public bool Liens { get; set; }
    public DateTime? TitleOpinionReceivedDate { get; set; }
    public bool TaxesDue { get; set; }

    [Column(TypeName = "money")]
    public decimal? TaxAmountDue { get; set; }

    [Column(TypeName = "money")]
    public decimal? TaxAmountPaid { get; set; }

    [Column(TypeName = "money")]
    public decimal? ConsiderationFee { get; set; }

    [Column(TypeName = "decimal(8,5)")]
    public decimal? CommissionPercent { get; set; }
    public bool AutoCalculateInvoice { get; set; }
    public int? FieldLandmanID { get; set; }

    [MaxLength(20)]
    public string? DSCollectionID { get; set; }

    [MaxLength(100)]
    public string? FolderLocation { get; set; }
    public bool Referrals { get; set; }

    [Column(TypeName = "money")]
    public decimal? ReferralFee { get; set; }
    public bool TakeConsiderationFromTotal { get; set; }

    [MaxLength(5)]
    public string? WhoPaysReferral { get; set; }

    public virtual User? LandMan { get; set; }
    public virtual ICollection<AcquisitionBuyer> AcquisitionBuyers { get; set; } = new List<AcquisitionBuyer>();
    public virtual ICollection<AcquisitionChange> AcquisitionChanges { get; set; } = new List<AcquisitionChange>();
    public virtual ICollection<AcquisitionCounty> AcquisitionCounties { get; set; } = new List<AcquisitionCounty>();
    public virtual ICollection<AcquisitionDocument> AcquisitionDocuments { get; set; } = new List<AcquisitionDocument>();
    public virtual ICollection<AcquisitionNote> AcquisitionNotes { get; set; } = new List<AcquisitionNote>();
    public virtual ICollection<AcquisitionSeller> AcquisitionSellers { get; set; } = new List<AcquisitionSeller>();
    public virtual ICollection<AcquisitionReferrer> AcquisitionReferrers { get; set; } = new List<AcquisitionReferrer>();
    public virtual ICollection<AcquisitionOperator> AcquisitionOperators { get; set; } = new List<AcquisitionOperator>();
    public virtual ICollection<AcquisitionStatus> AcquisitionStatus { get; set; } = new List<AcquisitionStatus>();
    public virtual ICollection<AcquisitionUnit> AcquisitionUnits { get; set; } = new List<AcquisitionUnit>();
    public virtual ICollection<AcquisitionLien> AcquisitionLiens { get; set; } = new List<AcquisitionLien>();
    public virtual ICollection<AcqCurativeRequirement> AcqCurativeRequirements { get; set; } = new List<AcqCurativeRequirement>();
}

public class Sequence
{
    [Key]
    public int SequenceID { get; set; }

    [MaxLength(50)]
    public string? SequenceName { get; set; }
    public int IncrementAmount { get; set; }
    public int StartValue { get; set; }
    public int CurrentValue { get; set; }
}

public class SigningPartner
{
    [Key]
    public int SigningPartnerID { get; set; }

    [Required, MaxLength(100)]
    public string SigningPartnerName { get; set; } = null!;
}

public class State
{
    [Key, MaxLength(2)]
    public string StateCode { get; set; } = null!;

    [Required, MaxLength(35)]
    public string StateName { get; set; } = null!;
}
