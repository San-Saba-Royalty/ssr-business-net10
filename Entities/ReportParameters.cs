namespace SSRBlazor.Models;

/// <summary>
/// Model for report filter parameters
/// </summary>
public class ReportParameters
{
    public int ReportID { get; set; }
    
    #region Multi-Select Lists
    
    /// <summary>
    /// Selected county IDs (empty = all)
    /// </summary>
    public List<int> SelectedCountyIds { get; set; } = new();
    
    /// <summary>
    /// Selected operator IDs (empty = all)
    /// </summary>
    public List<int> SelectedOperatorIds { get; set; } = new();
    
    /// <summary>
    /// Selected buyer IDs (empty = all)
    /// </summary>
    public List<int> SelectedBuyerIds { get; set; } = new();
    
    /// <summary>
    /// Selected deal status IDs (empty = all)
    /// </summary>
    public List<int> SelectedDealStatusIds { get; set; } = new();
    
    /// <summary>
    /// Deal status query type: 1=Is currently, 2=Is not currently, 3=Was ever, 4=Was never
    /// </summary>
    public int DealStatusQueryType { get; set; } = 1;
    
    /// <summary>
    /// Selected landman IDs (empty = all)
    /// </summary>
    public List<int> SelectedLandmanIds { get; set; } = new();
    
    /// <summary>
    /// Selected field landman IDs (empty = all)
    /// </summary>
    public List<int> SelectedFieldLandmanIds { get; set; } = new();
    
    #endregion
    
    #region Date Range Filters
    
    public DateRangeFilter EffectiveDate { get; set; } = new();
    public DateRangeFilter BuyerEffectiveDate { get; set; } = new();
    public DateRangeFilter DueDate { get; set; } = new();
    public DateRangeFilter PaidDate { get; set; } = new();
    public DateRangeFilter TitleOpinionReceivedDate { get; set; } = new();
    public DateRangeFilter ClosingLetterDate { get; set; } = new();
    public DateRangeFilter DeedDate { get; set; } = new();
    public DateRangeFilter InvoiceDate { get; set; } = new();
    public DateRangeFilter InvoiceDueDate { get; set; } = new();
    public DateRangeFilter InvoicePaidDate { get; set; } = new();
    
    #endregion
    
    #region Boolean Filters
    
    public BooleanFilter InvoiceNumber { get; set; } = new();
    public BooleanFilter IsReferral { get; set; } = new();
    public BooleanFilter HasCuratives { get; set; } = new();
    public BooleanFilter HasLiens { get; set; } = new();
    public BooleanFilter HasLetterAgreement { get; set; } = new();
    
    #endregion
    
    #region Report Options
    
    /// <summary>
    /// Sort by option: "DueDate" or "Landman"
    /// </summary>
    public string SortBy { get; set; } = "DueDate";
    
    /// <summary>
    /// Subtotal grouping: "LandmanMonth", "CountyMonth", "MonthLandman", "MonthCounty"
    /// </summary>
    public string SubtotalBy { get; set; } = "MonthCounty";
    
    public bool SummaryLevelOnly { get; set; } = true;
    public bool IncludeNewAcquisitions { get; set; } = true;
    public bool IncludeAmountPaidSellerSubtotal { get; set; } = true;
    public bool IncludeNotes { get; set; } = true;
    
    #endregion
    
    #region Output
    
    /// <summary>
    /// Output format: "excel", "viewer", "pdf", "word", "raw"
    /// </summary>
    public string OutputFormat { get; set; } = "excel";
    
    #endregion
    
    /// <summary>
    /// Reset all filters to defaults
    /// </summary>
    public void Reset()
    {
        SelectedCountyIds.Clear();
        SelectedOperatorIds.Clear();
        SelectedBuyerIds.Clear();
        SelectedDealStatusIds.Clear();
        DealStatusQueryType = 1;
        SelectedLandmanIds.Clear();
        SelectedFieldLandmanIds.Clear();
        
        EffectiveDate = new DateRangeFilter();
        BuyerEffectiveDate = new DateRangeFilter();
        DueDate = new DateRangeFilter();
        PaidDate = new DateRangeFilter();
        TitleOpinionReceivedDate = new DateRangeFilter();
        ClosingLetterDate = new DateRangeFilter();
        DeedDate = new DateRangeFilter();
        InvoiceDate = new DateRangeFilter();
        InvoiceDueDate = new DateRangeFilter();
        InvoicePaidDate = new DateRangeFilter();
        
        InvoiceNumber = new BooleanFilter();
        IsReferral = new BooleanFilter();
        HasCuratives = new BooleanFilter();
        HasLiens = new BooleanFilter();
        HasLetterAgreement = new BooleanFilter();
        
        SortBy = "DueDate";
        SubtotalBy = "MonthCounty";
        SummaryLevelOnly = true;
        IncludeNewAcquisitions = true;
        IncludeAmountPaidSellerSubtotal = true;
        IncludeNotes = true;
        OutputFormat = "excel";
    }
}

/// <summary>
/// Model for date range filter with From/To and Is Empty/Not Empty options
/// </summary>
public class DateRangeFilter
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsNotEmpty { get; set; }
    
    /// <summary>
    /// Validates that ToDate >= FromDate
    /// </summary>
    public bool IsValid => !FromDate.HasValue || !ToDate.HasValue || ToDate >= FromDate;
    
    /// <summary>
    /// Returns validation error message if invalid
    /// </summary>
    public string? ValidationError => IsValid ? null : "End date must be on or after start date";
    
    /// <summary>
    /// Returns true if any filter is set
    /// </summary>
    public bool HasValue => FromDate.HasValue || ToDate.HasValue || IsEmpty || IsNotEmpty;
}

/// <summary>
/// Model for boolean/tristate filter
/// </summary>
public class BooleanFilter
{
    public bool IsTrue { get; set; }
    public bool IsFalse { get; set; }
    
    /// <summary>
    /// Returns true if any filter is set
    /// </summary>
    public bool HasValue => IsTrue || IsFalse;
    
    /// <summary>
    /// Gets the effective filter value: true, false, or null (no filter)
    /// </summary>
    public bool? GetValue()
    {
        if (IsTrue && !IsFalse) return true;
        if (IsFalse && !IsTrue) return false;
        return null; // Both or neither selected = no filter
    }
}

/// <summary>
/// Lookup item for multi-select lists
/// </summary>
public class ReportLookupItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Available output formats
/// </summary>
public static class ReportOutputFormats
{
    public const string Excel = "excel";
    public const string Viewer = "viewer";
    public const string Pdf = "pdf";
    public const string Word = "word";
    public const string Raw = "raw";
    public const string Html = "html";
    
    public static readonly List<(string Value, string Text)> All = new()
    {
        (Excel, "Excel"),
        (Viewer, "Viewer"),
        (Pdf, "PDF"),
        (Word, "Word"),
        (Raw, "Raw")
    };
}

/// <summary>
/// Deal status query types
/// </summary>
public static class DealStatusQueryTypes
{
    public const int IsCurrently = 1;
    public const int IsNotCurrently = 2;
    public const int WasEver = 3;
    public const int WasNever = 4;
    
    public static readonly List<(int Value, string Text)> All = new()
    {
        (IsCurrently, "Is currently"),
        (IsNotCurrently, "Is not currently"),
        (WasEver, "Was ever"),
        (WasNever, "Was never")
    };
}

/// <summary>
/// Subtotal grouping options
/// </summary>
public static class SubtotalOptions
{
    public const string LandmanMonth = "LandmanMonth";
    public const string CountyMonth = "CountyMonth";
    public const string MonthLandman = "MonthLandman";
    public const string MonthCounty = "MonthCounty";
    
    public static readonly List<(string Value, string Text)> All = new()
    {
        (LandmanMonth, "Landman, Month"),
        (CountyMonth, "County, Month"),
        (MonthLandman, "Month, Landman"),
        (MonthCounty, "Month, County")
    };
}