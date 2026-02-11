using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SSRBusiness.Entities;

public class OperatorContact
{
    [Key]
    public int OperatorContactID { get; set; }
    public int OperatorID { get; set; }
    public int DocumentTemplateID { get; set; }

    [MaxLength(50)]
    public string? ContactName { get; set; }

    [MaxLength(50)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)] public string? Attention { get; set; }
    [MaxLength(24)] public string? ContactPhone { get; set; }
    [MaxLength(24)] public string? ContactFax { get; set; }

    [MaxLength(100)] public string? AddressLine1 { get; set; }
    [MaxLength(100)] public string? AddressLine2 { get; set; }
    [MaxLength(100)] public string? AddressLine3 { get; set; }
    [MaxLength(50)] public string? City { get; set; }
    [MaxLength(2)] public string? StateCode { get; set; }
    [MaxLength(10)] public string? ZipCode { get; set; }

    [JsonIgnore] // Break circular reference for OpenAPI schema generation
    public virtual Operator Operator { get; set; } = null!;
}


