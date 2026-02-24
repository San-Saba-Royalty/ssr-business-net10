using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;



public class CountyAppraisalGroup
{
    [Key]
    public int CountyAppraisalGroupID { get; set; }
    public int CountyID { get; set; }
    public int AppraisalGroupID { get; set; }
    public DateTime EffectiveDate { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual County County { get; set; } = null!;
    public virtual AppraisalGroup AppraisalGroup { get; set; } = null!;
}



public class CadTable
{
    [Key]
    public int CadTableID { get; set; }

    [Required, MaxLength(50)]
    public string TableName { get; set; } = null!;

    [Required, MaxLength(50)]
    public string DisplayName { get; set; } = null!;

    [Required, MaxLength(200)]
    public string ConnectionString { get; set; } = null!;
    public bool IncludeInSearch { get; set; }
}

public class FolderLocation
{
    [Key]
    public int FolderLocationID { get; set; }

    [Required, MaxLength(100)]
    public string FolderLocationText { get; set; } = null!;
}

public class LetAgUnitWell
{
    [Key]
    public int LetAgUnitWellID { get; set; }
    public int LetterAgreementUnitID { get; set; }
    public int LetterAgreementID { get; set; }

    [Required, MaxLength(200)]
    public string WellName { get; set; } = null!;

    [ForeignKey("LetterAgreementUnitID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreementUnit LetterAgreementUnit { get; set; } = null!;

    [ForeignKey("LetterAgreementID")]
    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual LetterAgreement LetterAgreement { get; set; } = null!;
}
