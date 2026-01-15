using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

[Table("DealStatus")]
public class DealStatus
{
    [Key]
    public int DealStatusID { get; set; }

    [Required, MaxLength(50)]
    public string StatusName { get; set; } = null!;
    public bool ExcludeFromReports { get; set; }
    public bool DefaultStatus { get; set; }
}

public class NoteType
{
    [Key, MaxLength(2)]
    public string NoteTypeCode { get; set; } = null!;

    [Required, MaxLength(30)]
    public string NoteTypeDesc { get; set; } = null!;
}

public class UnitType
{
    [Key, MaxLength(10)]
    public string UnitTypeCode { get; set; } = null!;

    [Required, MaxLength(50)]
    public string UnitTypeDesc { get; set; } = null!;
}

[Table("RolePermission")]
public class RolePermission
{
    [Key]
    public int RolePermissionID { get; set; }
    public int RoleID { get; set; }

    [Required, MaxLength(50)]
    public string PermissionCode { get; set; } = null!;

    [ForeignKey("RoleID")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("PermissionCode")]
    public virtual Permission Permission { get; set; } = null!;
}
