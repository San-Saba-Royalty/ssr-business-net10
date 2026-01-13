using System;
using System.Collections.Generic;

namespace SSRBusiness.ReportQueries;

public class ExistCheck
{
    public bool CheckExists { get; set; }
    public bool CheckNotExists { get; set; }

    public ExistCheck()
    {
        CheckExists = false;
        CheckNotExists = false;
    }
}

public class EmptyCheck
{
    public bool CheckIsEmpty { get; set; }
    public bool CheckIsNotEmpty { get; set; }

    public EmptyCheck()
    {
        CheckIsEmpty = false;
        CheckIsNotEmpty = false;
    }
}

public class ReportDate
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool CheckIsEmpty { get; set; }
    public bool CheckIsNotEmpty { get; set; }

    public ReportDate()
    {
        FromDate = null;
        ToDate = null;
        CheckIsEmpty = false;
        CheckIsNotEmpty = false;
    }
}

public class ReportSelectionCriteria
{
    public List<string> LandmanList { get; set; } = new();
    public List<string> FieldLandmanList { get; set; } = new();
    public List<string> CountyList { get; set; } = new();
    public List<string> OperatorList { get; set; } = new();
    public List<string> BuyerList { get; set; } = new();
    public string DealQueryType { get; set; } = "1";
    public List<string> DealStatusList { get; set; } = new();

    public ReportDate EffectiveDate { get; set; } = new();
    public ReportDate BuyerEffectiveDate { get; set; } = new();
    public ReportDate DueDate { get; set; } = new();
    public ReportDate PaidDate { get; set; } = new();
    public ReportDate TitleOpinionReceivedDate { get; set; } = new();
    public ReportDate ClosingLetterDate { get; set; } = new();
    public ReportDate DeedDate { get; set; } = new();
    public ReportDate InvoiceDate { get; set; } = new();
    public ReportDate InvoiceDueDate { get; set; } = new();
    public ReportDate InvoicePaidDate { get; set; } = new();

    public EmptyCheck InvoiceNumber { get; set; } = new();

    public ExistCheck ReferralCheck { get; set; } = new();
    public ExistCheck LienCheck { get; set; } = new();
    public ExistCheck CurativeCheck { get; set; } = new();
    public ExistCheck LetterAgreementCheck { get; set; } = new();

    public string SortOrder { get; set; } = "";
    public string SubTotalBy { get; set; } = "";
    public string IncludeNewNoInvoiceNumber { get; set; } = "";
    public string IncludeAmountPaidSellerSubtotal { get; set; } = "";
    public string SummaryLevelOnly { get; set; } = "";
    public string IncludeNotes { get; set; } = "";
}
