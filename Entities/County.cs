using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

public class County
{
    [Key]
    public int CountyID { get; set; }

    [Required, MaxLength(2)]
    public string StateCode { get; set; } = null!;

    [Required, MaxLength(50)]
    public string CountyName { get; set; } = null!;

    [MaxLength(50)] public string? ContactName { get; set; }
    [MaxLength(50)] public string? ContactEmail { get; set; }
    [MaxLength(24)] public string? ContactPhone { get; set; }
    [MaxLength(24)] public string? ContactFax { get; set; }
    
    [MaxLength(50)] public string? Attention { get; set; }

    [MaxLength(100)] public string? AddressLine1 { get; set; }
    [MaxLength(100)] public string? AddressLine2 { get; set; }
    [MaxLength(100)] public string? AddressLine3 { get; set; }
    [MaxLength(50)] public string? City { get; set; }
    [MaxLength(10)] public string? ZipCode { get; set; }

    [Column(TypeName = "money")]
    public decimal? RecordingFeeFirstPage { get; set; }
    
    [Column(TypeName = "money")]
    public decimal? RecordingFeeAdditionalPage { get; set; }

    [Column(TypeName = "money")]
    public decimal? CourtFee { get; set; }

    [ForeignKey(nameof(StateCode))]
    public virtual State? State { get; set; }
    
    public virtual ICollection<CountyContact> CountyContacts { get; set; } = new List<CountyContact>();
}
