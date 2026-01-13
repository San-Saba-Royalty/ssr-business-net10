using System.ComponentModel.DataAnnotations;

namespace SSRBusiness.Entities;

public class DocumentTemplate
{
    [Key]
    public int DocumentTemplateID { get; set; }

    [Required, MaxLength(20)]
    public string DocumentTypeCode { get; set; } = null!;

    [Required, MaxLength(500)]
    public string DocumentTemplateDesc { get; set; } = null!;

    [MaxLength(500)]
    public string? DocumentTemplateLocation { get; set; }

    [MaxLength(20)]
    public string? DSFileID { get; set; }

    public virtual DocumentType DocumentType { get; set; } = null!;
    
    public virtual ICollection<DocTemplateCustomField> CustomFields { get; set; } = new List<DocTemplateCustomField>();
}
