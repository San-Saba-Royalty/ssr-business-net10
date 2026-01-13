using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class AcquisitionStatus
{
    [Key]
    public int AcquisitionStatusID { get; set; }
    public int AcquisitionID { get; set; }
    public int DealStatusID { get; set; }
    public int UserID { get; set; }
    public DateTime StatusDate { get; set; }
    public string? Notes { get; set; }

    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual DealStatus DealStatus { get; set; } = null!;
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}

public class AcquisitionNote
{
    [Key]
    public int AcquisitionNoteID { get; set; }
    public int AcquisitionID { get; set; }
    public int UserID { get; set; }
    public DateTime CreatedDateTime { get; set; }
    
    [Required, MaxLength(2)]
    public string NoteTypeCode { get; set; } = null!;
    
    [MaxLength(3000)]
    public string? NoteText { get; set; }

    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual NoteType NoteType { get; set; } = null!;
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}

public class LetterAgreementStatus
{
    [Key]
    public int LetterAgreementStatusID { get; set; }
    public int LetterAgreementID { get; set; }
    
    [Required, MaxLength(5)]
    public string DealStatusCode { get; set; } = null!;
    public int UserID { get; set; }
    public DateTime StatusDate { get; set; }
    public string? Notes { get; set; }

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    public virtual LetterAgreementDealStatus LetterAgreementDealStatus { get; set; } = null!;
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}

public class LetterAgreementNote
{
    [Key]
    public int LetterAgreementNoteID { get; set; }
    public int LetterAgreementID { get; set; }
    public int UserID { get; set; }
    public DateTime CreatedDateTime { get; set; }
    
    [Required, MaxLength(2)]
    public string NoteTypeCode { get; set; } = null!;
    
    [MaxLength(3000)]
    public string? NoteText { get; set; }

    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    public virtual NoteType NoteType { get; set; } = null!;
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}
