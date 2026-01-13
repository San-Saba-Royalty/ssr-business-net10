using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class AcquisitionUnit
{
    [Key]
    public int AcquisitionUnitID { get; set; }
    public int AcquisitionID { get; set; }
    
    [MaxLength(200)]
    public string? UnitName { get; set; }
    
    [Column(TypeName = "decimal(20,10)")]
    public decimal? UnitInterest { get; set; }
    
    [MaxLength(10)]
    public string? UnitTypeCode { get; set; }
    
    [Required, MaxLength(20)]
    public string SsrInPay { get; set; } = null!;
    
    [Column(TypeName = "decimal(13,8)")]
    public decimal? GrossAcres { get; set; }
    
    [Column(TypeName = "decimal(13,8)")]
    public decimal? NetAcres { get; set; }
    
    [MaxLength(500)]
    public string? Surveys { get; set; }
    public DateTime? RecordedDate { get; set; }
    
    [MaxLength(20)]
    public string? VolumeNumber { get; set; }
    
    [MaxLength(20)]
    public string? PageNumber { get; set; }
    
    public string? LegalDescription { get; set; }
    public int? TownshipNum { get; set; }
    public char? TownshipDir { get; set; }
    public int? RangeNum { get; set; }
    public char? RangeDir { get; set; }
    public int? SectionNum { get; set; }

    public virtual Acquisition Acquisition { get; set; } = null!;
    public virtual UnitType? UnitType { get; set; }
    public virtual ICollection<AcqUnitCounty> AcqUnitCounties { get; set; } = new List<AcqUnitCounty>();
    public virtual ICollection<AcqUnitWell> AcqUnitWells { get; set; } = new List<AcqUnitWell>();
}
