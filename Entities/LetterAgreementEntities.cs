using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class LetterAgreementChange
{
    [Key]
    public int LetterAgreementChangeID { get; set; }
    public int LetterAgreementID { get; set; }
    public int UserID { get; set; }
    public DateTime ChangeDate { get; set; }

    [Required, MaxLength(5)]
    public string ChangeTypeCode { get; set; } = null!;

    [MaxLength(250)]
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}

public class LetterAgreementOperator
{
    [Key]
    public int LetterAgreementOperatorID { get; set; }
    public int LetterAgreementID { get; set; }
    public int OperatorID { get; set; }

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    public virtual Operator Operator { get; set; } = null!;
}

public class LetterAgreementCounty
{
    [Key]
    public int LetterAgreementCountyID { get; set; }
    public int LetterAgreementID { get; set; }
    public int CountyID { get; set; }

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    public virtual County County { get; set; } = null!;
}

public class LetterAgreementReferrer
{
    [Key]
    public int LetterAgreementReferrerID { get; set; }
    public int LetterAgreementID { get; set; }
    public int ReferrerID { get; set; }

    [Column(TypeName = "money")]
    public decimal? ReferralAmount { get; set; }

    [Column(TypeName = "decimal(8,5)")]
    public decimal? ReferralPercent { get; set; }
    public bool SellerPaysReferralAmount { get; set; }

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    public virtual Referrer Referrer { get; set; } = null!;
}

[Table("LetterAgreementDealStatus")]
public class LetterAgreementDealStatus
{
    [Key, MaxLength(5)]
    public string DealStatusCode { get; set; } = null!;

    [Required, MaxLength(50)]
    public string StatusName { get; set; } = null!;
    public bool ExcludeFromReports { get; set; }
}

public class LetterAgreementFilter
{
    [Key]
    public int LetterAgreementFilterID { get; set; }
    // Add additional filter properties as needed based on original VB schema
}
