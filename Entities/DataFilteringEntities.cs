using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class AcqUnitCounty
{
    [Key]
    public int AcqUnitCountyID { get; set; }
    public int AcquisitionID { get; set; }
    public int AcquisitionUnitID { get; set; }
    public int CountyID { get; set; }
    
    public virtual Acquisition Acquisition { get; set; } = null!;
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

    public virtual Filter Filter { get; set; } = null!;
}

public class View
{
    [Key]
    public int ViewID { get; set; }
    
    [Required, MaxLength(50)]
    public string ViewName { get; set; } = null!;
    
    public virtual ICollection<ViewField> ViewFields { get; set; } = new List<ViewField>();
}

public class ViewField
{
    [Key]
    public int ViewFieldID { get; set; }
    public int ViewID { get; set; }
    public int FieldID { get; set; }
    public int DisplayOrder { get; set; }
    
    public virtual View View { get; set; } = null!;
}
