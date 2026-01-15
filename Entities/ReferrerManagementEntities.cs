using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class UserAcquisitionSetup
{
    [Key]
    public int UserAcquisitionSetupID { get; set; }
    public int UserID { get; set; }
    public int AcquisitionID { get; set; }

    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
    public virtual Acquisition Acquisition { get; set; } = null!;
}

public class Referrer
{
    [Key]
    public int ReferrerID { get; set; }

    [MaxLength(100)]
    public string? ReferrerName { get; set; }

    [MaxLength(4)]
    public string? ReferrerTypeCode { get; set; }

    [MaxLength(11)]
    public string? ReferrerTaxID { get; set; }

    [MaxLength(50)]
    public string? ContactName { get; set; }

    [MaxLength(50)]
    public string? ContactEmail { get; set; }

    [MaxLength(24)]
    public string? ContactPhone { get; set; }

    [MaxLength(50)]
    public string? AddressLine1 { get; set; }

    [MaxLength(30)]
    public string? City { get; set; }

    [MaxLength(2)]
    public string? StateCode { get; set; }

    [MaxLength(10)]
    public string? ZipCode { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdatedOn { get; set; }

    public virtual ICollection<ReferrerForm> ReferrerForms { get; set; } = new List<ReferrerForm>();
}

public class ReferrerType
{
    [Key, MaxLength(4)]
    public string ReferrerTypeCode { get; set; } = null!;

    [Required, MaxLength(50)]
    public string ReferrerTypeDesc { get; set; } = null!;
}

public class ReferrerForm
{
    [Key]
    public int ReferrerFormID { get; set; }
    public int ReferrerID { get; set; }

    [Required, MaxLength(5)]
    public string FormTypeCode { get; set; } = null!;

    [Required, MaxLength(4)]
    public string FormYear { get; set; } = null!;

    [MaxLength(50)]
    public string? DSFileID { get; set; }

    public virtual Referrer Referrer { get; set; } = null!;
}

public class ReferrerFormType
{
    [Key, MaxLength(5)]
    public string FormTypeCode { get; set; } = null!;

    [Required, MaxLength(100)]
    public string FormTypeDesc { get; set; } = null!;
}
