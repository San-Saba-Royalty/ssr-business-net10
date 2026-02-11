using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class AcquisitionLien
{
    [Key]
    public int AcquisitionLienID { get; set; }
    public int AcquisitionID { get; set; }
    public int LienTypeID { get; set; }
    public string? LienHolder { get; set; }
    [MaxLength(10)] public string? LienPosition { get; set; }
    [Column(TypeName = "money")] public decimal? OriginalAmount { get; set; }
    [Column(TypeName = "money")] public decimal? PayoffAmount { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual LienType LienType { get; set; } = null!;
}

public class AcquisitionLienCounty
{
    [Key]
    public int AcquisitionLienCountyID { get; set; }
    public int AcquisitionLienID { get; set; }
    public int AcquisitionID { get; set; }
    public int CountyID { get; set; }
    public DateTime? RecordedDate { get; set; }
    
    public virtual AcquisitionLien AcquisitionLien { get; set; } = null!;
}

public class AcquisitionLienUnit
{
    [Key]
    public int AcquisitionLienUnitID { get; set; }
    public int AcquisitionLienID { get; set; }
    public int AcquisitionUnitID { get; set; }
    
    public virtual AcquisitionLien AcquisitionLien { get; set; } = null!;
    public virtual AcquisitionUnit AcquisitionUnit { get; set; } = null!;
}

public class AcqCurativeRequirement
{
    [Key]
    public int AcqCurativeRequirementID { get; set; }
    public int AcquisitionID { get; set; }
    public int CurativeTypeID { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual CurativeType CurativeType { get; set; } = null!;
}
