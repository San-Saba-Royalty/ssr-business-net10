using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

[Table("LoginStatuses")]
public class LoginStatus
{
    [Key]
    public int LoginStatusId { get; set; }

    [StringLength(50)]
    public string? StatusName { get; set; }
}

[Table("UserRoles")]
public class UserRole
{
    [Key]
    public int UserRoleId { get; set; }

    public int UserId { get; set; }
    public int RoleId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}

[Table("Roles")]
public class Role
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(50)]
    [Column("Role")]
    public string RoleName { get; set; } = string.Empty;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

[Table("UserHistory")]
public class UserHistory
{
    [Key]
    public int UserHistoryId { get; set; }

    public int UserId { get; set; }
    public DateTime ActionDate { get; set; }

    [StringLength(500)]
    public string? Action { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

[Table("UserPasswordHistory")]
public class UserPasswordHistory
{
    [Key]
    public int PasswordHistoryId { get; set; }

    public int UserId { get; set; }
    public DateTime ChangedDate { get; set; }

    [StringLength(500)]
    public string? OldPassword { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

// Stub classes - to be fully implemented
[Table("ChangeTypes")]
public class ChangeType
{
    [Key]
    [MaxLength(5)]
    public string ChangeTypeCode { get; set; } = null!;

    [MaxLength(50)]
    public string? ChangeTypeDesc { get; set; }

    public virtual ICollection<AcquisitionChange> AcquisitionChanges { get; set; } = new List<AcquisitionChange>();
    public virtual ICollection<LetterAgreementChange> LetterAgreementChanges { get; set; } = new List<LetterAgreementChange>();
}

[Table("AcquisitionChanges")]
public class AcquisitionChange
{
    [Key]
    public int AcquisitionChangeId { get; set; }
    
    public int AcquisitionId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(5)]
    public string ChangeTypeCode { get; set; } = null!;
    
    public DateTime ChangeDate { get; set; }
    
    [MaxLength(100)]
    public string? FieldName { get; set; }
    
    [MaxLength(500)]
    public string? NewValue { get; set; }
    
    [MaxLength(500)]
    public string? OldValue { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey("AcquisitionId")]
    public virtual Acquisition? Acquisition { get; set; }
    
    [ForeignKey("ChangeTypeCode")]
    public virtual ChangeType? ChangeType { get; set; }
}

[Table("LetterAgreementChanges")]
public class LetterAgreementChange
{
    [Key]
    public int LetterAgreementChangeId { get; set; }
    
    public int LetterAgreementId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(5)]
    public string ChangeTypeCode { get; set; } = null!;
    
    public DateTime ChangeDate { get; set; }
    
    [MaxLength(100)]
    public string? FieldName { get; set; }
    
    [MaxLength(500)]
    public string? NewValue { get; set; }
    
    [MaxLength(500)]
    public string? OldValue { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey("LetterAgreementId")]
    public virtual LetterAgreement? LetterAgreement { get; set; }
    
    [ForeignKey("ChangeTypeCode")]
    public virtual ChangeType? ChangeType { get; set; }
}

[Table("AcquisitionDocuments")]
public class AcquisitionDocument
{
    [Key]
    public int AcquisitionDocumentID { get; set; }

    public int AcquisitionID { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("AcquisitionID")]
    public virtual Acquisition? Acquisition { get; set; }

    [StringLength(500)]
    public string? DocumentLocation { get; set; }
}

