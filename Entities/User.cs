using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

[Table("Users")]
public class User
{
    [Key]
    [Column("UserID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Column("UserName")]
    [StringLength(35)]
    public string? UserName { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Title { get; set; }

    [StringLength(24)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Password { get; set; }

    [StringLength(500)]
    public string? Salt { get; set; }

    [Column("Active")]
    public bool IsActive { get; set; }

    public bool Administrator { get; set; }

    public bool Locked { get; set; }

    public DateTime? PasswordExpirationDate { get; set; }

    public int NumberFailedAttempts { get; set; }

    public DateTime? LockoutExpirationDate { get; set; }

    public DateTime? LoginExpirationDate { get; set; }

    public int? LastFilterID { get; set; }

    public int? LastViewID { get; set; }

    public int? LastAcquisitionID { get; set; }

    public int? LastLetterAgreementID { get; set; }

    public bool DisplayToolbar { get; set; }

    [StringLength(20)]
    public string? Theme { get; set; }

    [StringLength(20)]
    public string? DefaultReportOutput { get; set; }

    // Navigation properties
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<UserPasswordHistory> PasswordHistories { get; set; } = new List<UserPasswordHistory>();
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<AcquisitionChange> AcquisitionChanges { get; set; } = new List<AcquisitionChange>();
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<AcquisitionDocument> AcquisitionDocuments { get; set; } = new List<AcquisitionDocument>();
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual ICollection<AcquisitionNote> AcquisitionNotes { get; set; } = new List<AcquisitionNote>();
}
