using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class AcqUnitCountyOperator
{
    [Key]
    public int AcqUnitCountyOperID { get; set; }
    public int AcquisitionID { get; set; }
    public int AcqUnitCountyID { get; set; }
    public int OperatorID { get; set; }

    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual AcqUnitCounty AcqUnitCounty { get; set; } = null!;
    public virtual Operator Operator { get; set; } = null!;
}

public class LienType
{
    [Key]
    public int LienTypeID { get; set; }
    [Required, MaxLength(50)] public string LienTypeName { get; set; } = null!;
}



public class DisplayField
{
    [Key]
    public int FieldID { get; set; }
    [Required, MaxLength(50)] public string FieldName { get; set; } = null!;
    [Required, MaxLength(100)] public string ColumnName { get; set; } = null!;
    public int DisplayOrder { get; set; }
}

[Table("Permission")]
public class Permission
{
    [Key, MaxLength(50)]
    public string PermissionCode { get; set; } = null!;
    [Required, MaxLength(50)] public string PermissionDesc { get; set; } = null!;
    public int DisplayOrder { get; set; }
}
