namespace SSRBusiness.Support;

/// <summary>
/// Simple lookup list item for dropdowns and lists.
/// Replaces the anonymous type used in VB LINQ queries.
/// </summary>
public class LookupListItem
{
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public override string ToString() => Description;
}
