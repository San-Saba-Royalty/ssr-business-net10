using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class AcqUnitCountyOperator
{
    [Key]
    public int AcqUnitCountyOperID { get; set; }
    public int AcquisitionID { get; set; }
    public int AcqUnitCountyID { get; set; }
    public int OperatorID { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
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
    
    [MaxLength(50)]
    public string Module { get; set; } = "Acquisition";
    
    [MaxLength(20)]
    public string? DataType { get; set; }
}

[Table("Permission")]
public class Permission
{
    [Key, MaxLength(50)]
    public string PermissionCode { get; set; } = null!;
    [Required, MaxLength(50)] public string PermissionDesc { get; set; } = null!;
    public int DisplayOrder { get; set; }
}
