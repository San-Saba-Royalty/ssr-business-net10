using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSRBusiness.Entities;

[Table("UserViewPreferences")]
public class UserViewPreference
{
    [Key]
    public int UserViewPreferenceID { get; set; }

    public int UserID { get; set; }

    [StringLength(100)]
    public string ViewName { get; set; } = string.Empty;

    [StringLength(50)]
    public string TableName { get; set; } = string.Empty; // e.g. 'Acquisitions', 'LetterAgreements'

    public string ColumnOrder { get; set; } = "[]"; // JSON array of column names/IDs

    public bool IsDefault { get; set; }
}
