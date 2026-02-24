using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class LetterAgreementUnit
{
    [Key]
    public int LetterAgreementUnitID { get; set; }
    public int LetterAgreementID { get; set; }
    
    [MaxLength(200)]
    public string? UnitName { get; set; }
    
    [Column(TypeName = "decimal(20,10)")]
    public decimal? UnitInterest { get; set; }
    
    [MaxLength(10)]
    public string? UnitTypeCode { get; set; }
    
    [Column(TypeName = "decimal(13,8)")]
    public decimal? GrossAcres { get; set; }
    
    [Column(TypeName = "decimal(13,8)")]
    public decimal? NetAcres { get; set; }
    
    [MaxLength(500)]
    public string? Surveys { get; set; }
    public string? LegalDescription { get; set; }
    public int? TownshipNum { get; set; }
    public char? TownshipDir { get; set; }
    public int? RangeNum { get; set; }
    public char? RangeDir { get; set; }
    public int? SectionNum { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    [ForeignKey("UnitTypeCode")]
    public virtual UnitType? UnitType { get; set; }
    public virtual ICollection<LetAgUnitCounty> LetAgUnitCounties { get; set; } = new List<LetAgUnitCounty>();
    public virtual ICollection<LetAgUnitWell> LetAgUnitWells { get; set; } = new List<LetAgUnitWell>();
}

public class LetAgUnitCounty
{
    [Key]
    public int LetAgUnitCountyID { get; set; }
    public int LetterAgreementID { get; set; }
    public int LetterAgreementUnitID { get; set; }
    public int CountyID { get; set; }
    
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreementUnit LetterAgreementUnit { get; set; } = null!;
    public virtual County County { get; set; } = null!;
    public virtual ICollection<LetAgUnitCountyOperator> LetAgUnitCountyOperators { get; set; } = new List<LetAgUnitCountyOperator>();
}

public class LetAgUnitCountyOperator
{
    [Key]
    public int LetAgUnitCountyOperID { get; set; }
    public int LetterAgreementID { get; set; }
    public int LetAgUnitCountyID { get; set; }
    public int OperatorID { get; set; }
    
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetAgUnitCounty LetAgUnitCounty { get; set; } = null!;
    public virtual Operator Operator { get; set; } = null!;
}
