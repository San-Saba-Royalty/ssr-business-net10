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

    public virtual ICollection<User> Users { get; set; } = new List<User>();
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
[Table("AcquisitionChanges")]
public class AcquisitionChange
{
    [Key]
    public int AcquisitionChangeId { get; set; }
    public int AcquisitionId { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
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

