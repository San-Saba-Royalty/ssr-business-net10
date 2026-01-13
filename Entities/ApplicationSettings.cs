using System.ComponentModel.DataAnnotations;

namespace SSRBusiness.Entities;

public class ApplicationSettings
{
    [Key]
    public int? PasswordExpirationDays { get; set; }
    public bool PasswordRequiresAlphaNumeric { get; set; }
    public bool PasswordRequiresSpecial { get; set; }
    public bool PasswordRequiresUpperCase { get; set; }
    public int NumberPasswordsStored { get; set; }
    public bool EnableLockoutDueToFailedPassword { get; set; }
    public int? NumberAttemptsBeforeLockout { get; set; }
    public int? NumberMinutesToLockout { get; set; }
}
