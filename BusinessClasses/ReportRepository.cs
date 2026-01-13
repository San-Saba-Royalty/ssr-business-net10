using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for Report data access
/// </summary>
public class ReportRepository
{
    private readonly SsrDbContext _context;

    public ReportRepository(SsrDbContext context)
    {
        _context = context;
    }

    #region Report Operations

    /// <summary>
    /// Get all active reports (hardcoded - Report table doesn't exist)
    /// </summary>
    public Task<List<Report>> GetAllAsync()
    {
        // Since Report table doesn't exist, return hardcoded list for now
        var reports = new List<Report>
        {
            new Report
            {
                ReportID = 1,
                ReportName = "Drafts Due",
                Description = "Shows drafts that are coming due.",
                Category = "Land",
                ReportFile = "rptDraftsDue",
                DisplayOrder = 1,
                IsActive = true,
                ShowCountyFilter = true,
                ShowOperatorFilter = true,
                ShowBuyerFilter = true,
                ShowDealStatusFilter = true,
                ShowLandmanFilter = true,
                ShowFieldLandmanFilter = true,
                ShowDueDateFilter = true,
                ShowSortByOption = true
            },
            new Report
            {
                ReportID = 2,
                ReportName = "Buyer Invoices Due",
                Description = "Shows invoices due for buyers.",
                Category = "Accounting",
                ReportFile = "rptBuyerInvoicesDue",
                DisplayOrder = 2,
                IsActive = true,
                ShowBuyerFilter = true,
                ShowInvoiceDueDateFilter = true
            },
            new Report
            {
                ReportID = 3,
                ReportName = "Curative Requirements",
                Description = "Shows curative requirements for acquisitions.",
                Category = "Legal",
                ReportFile = "rptCurativeRequirements",
                DisplayOrder = 3,
                IsActive = true,
                ShowBuyerFilter = true,
                ShowDealStatusFilter = true,
                ShowCurativeFilter = true
            },
            new Report
            {
                ReportID = 4,
                ReportName = "Letter Agreement Deals",
                Description = "Shows deals associated with letter agreements.",
                Category = "Land",
                ReportFile = "rptLetterAgreementDeals",
                DisplayOrder = 4,
                IsActive = true,
                ShowLetterAgreementFilter = true
            },
            new Report
            {
                ReportID = 5,
                ReportName = "Purchases",
                Description = "Shows purchases made within a timeframe.",
                Category = "Accounting",
                ReportFile = "rptPurchases",
                DisplayOrder = 5,
                IsActive = true,
                ShowCountyFilter = true,
                ShowEffectiveDateFilter = true
            },
            new Report
            {
                ReportID = 6,
                ReportName = "Referrer 1099 Summary",
                Description = "Shows a summary of referral fees paid.",
                Category = "Accounting",
                ReportFile = "rptReferrer1099Summary",
                DisplayOrder = 6,
                IsActive = true,
                ShowLandmanFilter = true,
                ShowPaidDateFilter = true
            }
        };
        return Task.FromResult(reports);
    }

    /// <summary>
    /// Get reports grouped by category
    /// </summary>
    public async Task<Dictionary<string, List<Report>>> GetByCategoryAsync()
    {
        var reports = await GetAllAsync();

        return reports
            .GroupBy(r => r.Category ?? "Other")
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Get report by ID
    /// </summary>
    public async Task<Report?> GetByIdAsync(int reportId)
    {
        var reports = await GetAllAsync();
        return reports.FirstOrDefault(r => r.ReportID == reportId);
    }

    #endregion

    #region Lookup Data for Filters

    /// <summary>
    /// Get all counties for filter list
    /// </summary>
    public async Task<List<County>> GetCountiesAsync()
    {
        return await _context.Counties
            .OrderBy(c => c.CountyName)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all operators for filter list
    /// </summary>
    public async Task<List<Operator>> GetOperatorsAsync()
    {
        return await _context.Operators
            .OrderBy(o => o.OperatorName)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all buyers for filter list
    /// </summary>
    public async Task<List<Buyer>> GetBuyersAsync()
    {
        return await _context.Buyers
            .OrderBy(b => b.BuyerName)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all filters (deal statuses) for filter list
    /// </summary>
    public async Task<List<Filter>> GetDealStatusesAsync()
    {
        return await _context.Filters
            .OrderBy(f => f.FilterName)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all referrers (landmen) for filter list
    /// </summary>
    public async Task<List<Referrer>> GetLandmenAsync()
    {
        return await _context.Referrers
            .OrderBy(r => r.ReferrerName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion
}