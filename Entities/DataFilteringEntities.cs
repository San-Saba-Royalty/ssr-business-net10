using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class AcqUnitCounty
{
    [Key]
    public int AcqUnitCountyID { get; set; }
    public int AcquisitionID { get; set; }
    public int AcquisitionUnitID { get; set; }
    public int CountyID { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual AcquisitionUnit AcquisitionUnit { get; set; } = null!;
    public virtual County County { get; set; } = null!;
}

public class AcqUnitWell
{
    [Key]
    public int AcqUnitWellID { get; set; }
    public int AcquisitionUnitID { get; set; }
    public int AcquisitionID { get; set; }
    
    [Required, MaxLength(200)]
    public string WellName { get; set; } = null!;

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual AcquisitionUnit AcquisitionUnit { get; set; } = null!;
}

public class Filter
{
    [Key]
    public int FilterID { get; set; }
    
    [Required, MaxLength(50)]
    public string FilterName { get; set; } = null!;
    
    public virtual ICollection<FilterField> FilterFields { get; set; } = new List<FilterField>();
}

public class FilterField
{
    [Key]
    public int FilterFieldID { get; set; }
    public int FilterID { get; set; }
    
    [Required, MaxLength(3)]
    public string ConditionalCode { get; set; } = null!;
    public int FieldID { get; set; }
    public int ComparisonTypeID { get; set; }
    
    [MaxLength(500)]
    public string? ComparisonValue { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Filter Filter { get; set; } = null!;

    [ForeignKey(nameof(FieldID))]
    public virtual LookupField? LookupField { get; set; }

    [ForeignKey(nameof(ComparisonTypeID))]
    public virtual ComparisonType? ComparisonType { get; set; }
}

public class View
{
    [Key]
    public int ViewID { get; set; }
    
    [Required, MaxLength(50)]
    public string ViewName { get; set; } = null!;
    
    public virtual ICollection<ViewField> ViewFields { get; set; } = new List<ViewField>();
    
    [MaxLength(50)]
    public string Module { get; set; } = "Acquisition";
}

public class UserPagePreference
{
    [Key]
    public int PreferenceID { get; set; }
    
    public int UserID { get; set; }
    
    [Required, MaxLength(50)]
    public string PageName { get; set; } = null!;
    
    public int ViewID { get; set; }
    
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual User User { get; set; } = null!;
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual View View { get; set; } = null!;
}

public class ViewField
{
    [Key]
    public int ViewFieldID { get; set; }
    public int ViewID { get; set; }
    public int FieldID { get; set; }
    public int DisplayOrder { get; set; }
    
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual View View { get; set; } = null!;
}
