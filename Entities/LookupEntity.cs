using System.ComponentModel.DataAnnotations;

namespace SSRBusiness.Entities;

public class Lookup
{
    [Key]
    public int LookupID { get; set; }
    
    [Required, MaxLength(50)]
    public string LookupType { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string LookupCode { get; set; } = null!;
    
    [Required, MaxLength(100)]
    public string LookupDescription { get; set; } = null!;
    
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
