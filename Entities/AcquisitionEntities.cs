using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class AcquisitionBuyer
{
    [Key]
    public int AcquisitionBuyerID { get; set; }
    public int AcquisitionID { get; set; }
    public int BuyerID { get; set; }

    // ── Per-buyer invoicing fields ──────────────────────────────────────
    [Column(TypeName = "decimal(8,5)")]
    public decimal? PurchasePercent { get; set; }

    [Column(TypeName = "money")]
    public decimal? PurchaseAmount { get; set; }

    [MaxLength(50)]
    public string? InvoiceNumber { get; set; }

    public DateTime? InvoiceDate { get; set; }
    public DateTime? InvoiceDueDate { get; set; }
    public DateTime? InvoicePaidDate { get; set; }

    public bool AutoCalculate { get; set; } = true;

    [Column(TypeName = "money")]
    public decimal? InvoiceTotal { get; set; }

    [Column(TypeName = "money")]
    public decimal? Markup { get; set; }

    [Column(TypeName = "money")]
    public decimal? RecordingFee { get; set; }

    // ── Navigation properties ───────────────────────────────────────────
    [ForeignKey("AcquisitionID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    [ForeignKey("BuyerID")]
    public virtual Buyer Buyer { get; set; } = null!;
}

public class AcquisitionCounty
{
    [Key]
    public int AcquisitionCountyID { get; set; }
    public int AcquisitionID { get; set; }
    public int CountyID { get; set; }
    public DateTime? DeedSentDate { get; set; }
    public DateTime? DeedReturnedDate { get; set; }
    [MaxLength(50)] public string? RecordingBook { get; set; }
    [MaxLength(20)] public string? RecordingPage { get; set; }

    [ForeignKey("AcquisitionID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    [ForeignKey("CountyID")]
    public virtual County County { get; set; } = null!;
}

public class AcquisitionOperator
{
    [Key]
    public int AcquisitionOperatorID { get; set; }
    public int AcquisitionID { get; set; }
    public int OperatorID { get; set; }
    public DateTime? NotifiedDateNoRec { get; set; }
    public DateTime? NotifiedDateRec { get; set; }
    public DateTime? DOReceivedDate { get; set; }

    [ForeignKey("AcquisitionID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    [ForeignKey("OperatorID")]
    public virtual Operator Operator { get; set; } = null!;
}

public class AcquisitionReferrer
{
    [Key]
    public int AcquisitionReferrerID { get; set; }
    public int AcquisitionID { get; set; }
    public int ReferrerID { get; set; }
    [Column(TypeName = "money")] public decimal? ReferralAmount { get; set; }
    [Column(TypeName = "decimal(8,5)")] public decimal? ReferralPercent { get; set; }

    [ForeignKey("AcquisitionID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    [ForeignKey("ReferrerID")]
    public virtual Referrer Referrer { get; set; } = null!;
}
