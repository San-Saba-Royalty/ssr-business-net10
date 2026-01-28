using System.ComponentModel.DataAnnotations;

namespace SSRBusiness.Entities;

public class DocTemplateCustomField
{
    [Key]
    public int DocTemplateCustomFieldID { get; set; }
    public int DocumentTemplateID { get; set; }

    [Required, MaxLength(200)]
    public string CustomPhrase { get; set; } = null!;

    [Required, MaxLength(50)]
    public string CustomTag { get; set; } = null!;

    public virtual DocumentTemplate DocumentTemplate { get; set; } = null!;
}
