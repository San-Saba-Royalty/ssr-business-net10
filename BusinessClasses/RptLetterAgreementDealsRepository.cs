using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class RptLetterAgreementDealsRepository
    {
        private readonly SsrDbContext _context;
        private readonly ILogger<RptLetterAgreementDealsRepository>? _logger;

        public RptLetterAgreementDealsRepository(SsrDbContext context, ILogger<RptLetterAgreementDealsRepository>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ReportLetterAgreementDeals>> GetRptLetterAgreementDealsAsync(List<string> acqIdList)
        {
            if (acqIdList == null || !acqIdList.Any())
                return new List<ReportLetterAgreementDeals>();

            var acqIds = acqIdList.Select(id => int.TryParse(id, out int i) ? i : 0).Where(i => i > 0).ToList();

            if (!acqIds.Any())
                return new List<ReportLetterAgreementDeals>();

            _logger?.LogInformation("Loading letter agreements for {Count} acquisitions", acqIds.Count);

            // WORKAROUND for EF Core 10.0 OPENJSON bug:
            // Instead of using Contains() which generates malformed OPENJSON syntax,
            // we batch the IDs and query in smaller chunks to avoid the bug entirely.
            const int batchSize = 100;
            var letterAgreements = new List<LetterAgreement>();

            for (int i = 0; i < acqIds.Count; i += batchSize)
            {
                var batch = acqIds.Skip(i).Take(batchSize).ToList();

                var batchResults = await _context.LetterAgreements
                    .FromSqlRaw($@"
                        SELECT [l].[LetterAgreementID], [l].[AcquisitionID], [l].[BankingDays], [l].[ConsiderationFee],
                               [l].[CreatedOn], [l].[EffectiveDate], [l].[LandManID], [l].[LastUpdatedOn],
                               [l].[ReceiptDate], [l].[ReferralFee], [l].[Referrals], [l].[TakeConsiderationFromTotal],
                               [l].[TotalBonus], [l].[TotalBonusAndFee], [l].[TotalGrossAcres], [l].[TotalNetAcres]
                        FROM [LetterAgreements] AS [l]
                        WHERE [l].[AcquisitionID] IN ({string.Join(",", batch)})")
                    .Include(la => la.LandMan)
                    .OrderBy(la => la.LetterAgreementID)
                    .ToListAsync();

                letterAgreements.AddRange(batchResults);

                _logger?.LogDebug("Loaded batch {BatchNumber} with {Count} letter agreements", (i / batchSize) + 1, batchResults.Count);
            }

            _logger?.LogInformation("Successfully loaded {Count} letter agreements for {AcqCount} acquisitions", letterAgreements.Count, acqIds.Count);

            if (!letterAgreements.Any())
                return new List<ReportLetterAgreementDeals>();

            // Extract the actual LetterAgreementIDs to use for all status/seller/county/etc. lookups
            var letAgIds = letterAgreements.Select(la => la.LetterAgreementID).ToList();

            // Batch load all statuses - using same workaround to avoid OPENJSON bug
            var allStatuses = await LoadInBatchesAsync(
                letAgIds,
                batchIds => _context.LetterAgreementStatuses
                    .FromSqlRaw($"SELECT * FROM [LetterAgreementStatus] WHERE [LetterAgreementID] IN ({string.Join(",", batchIds)})")
                    .Include(s => s.LetterAgreementDealStatus)
                    .ToListAsync());

            // Find latest status for each letter agreement
            var latestStatusMap = allStatuses
                .GroupBy(s => s.LetterAgreementID)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(s => s.StatusDate).FirstOrDefault()
                );

            // Batch load all sellers - using same workaround
            var sellers = await LoadInBatchesAsync(
                letAgIds,
                batchIds => _context.LetterAgreementSellers
                    .FromSqlRaw($"SELECT * FROM [LetterAgreementSellers] WHERE [LetterAgreementID] IN ({string.Join(",", batchIds)})")
                    .ToListAsync());
            var sellerMap = sellers.ToDictionary(s => s.LetterAgreementID);

            // Batch load all referrers - using same workaround
            var referrerIds = await LoadInBatchesAsync(
                letAgIds,
                batchIds => _context.LetterAgreementReferrers
                    .FromSqlRaw($"SELECT * FROM [LetterAgreementReferrers] WHERE [LetterAgreementID] IN ({string.Join(",", batchIds)})")
                    .ToListAsync());

            // Load the actual referrer objects for the referrer IDs we found
            var uniqueReferrerIds = referrerIds.Select(r => r.ReferrerID).Distinct().ToList();
            var referrerEntities = uniqueReferrerIds.Count > 0
                ? await _context.Referrers.Where(r => uniqueReferrerIds.Contains(r.ReferrerID)).ToListAsync()
                : new List<Referrer>();

            var referrerDict = referrerEntities.ToDictionary(r => r.ReferrerID);
            var referrerMap = referrerIds
                .Where(lar => referrerDict.ContainsKey(lar.ReferrerID))
                .ToDictionary(
                    lar => lar.LetterAgreementID,
                    lar => referrerDict[lar.ReferrerID]);

            // Batch load counties
            var allCounties = await LoadInBatchesAsync(letAgIds, batchIds =>
                (from lac in _context.LetterAgreementCounties
                 join c in _context.Counties on lac.CountyID equals c.CountyID
                 where batchIds.Contains(lac.LetterAgreementID)
                 orderby (c.CountyName ?? "").ToUpper()
                 select new { lac.LetterAgreementID, Data = new ReportLetterAgreementDealsCounty { CountyName = c.CountyName + ", " + c.StateCode } })
                .ToListAsync());
            var countyMap = allCounties.GroupBy(x => x.LetterAgreementID).ToDictionary(g => g.Key, g => g.Select(x => x.Data).ToList());

            // Batch load units
            var allUnits = await LoadInBatchesAsync(letAgIds, batchIds =>
                (from lau in _context.LetterAgreementUnits
                 where batchIds.Contains(lau.LetterAgreementID)
                 orderby (lau.UnitName ?? "").ToUpper()
                 select new { lau.LetterAgreementID, Data = new ReportLetterAgreementDealsUnit { UnitName = lau.UnitName ?? "", UnitInterest = lau.UnitInterest } })
                .ToListAsync());
            var unitMap = allUnits.GroupBy(x => x.LetterAgreementID).ToDictionary(g => g.Key, g => g.Select(x => x.Data).ToList());

            // Batch load notes
            var allNotes = await LoadInBatchesAsync(letAgIds, batchIds =>
                (from lan in _context.LetterAgreementNotes
                 join nt in _context.NoteTypes on lan.NoteTypeCode equals nt.NoteTypeCode
                 where batchIds.Contains(lan.LetterAgreementID)
                 orderby lan.CreatedDateTime
                 select new
                 {
                     lan.LetterAgreementID,
                     Data = new ReportLetterAgreementDealsNote
                     {
                         NoteCreatedOn = lan.CreatedDateTime,
                         UserFirstName = lan.User.FirstName ?? "",
                         UserLastName = lan.User.LastName ?? "",
                         UserName = lan.User.UserId.ToString(),
                         NoteTypeDesc = nt.NoteTypeDesc ?? "",
                         Note = lan.NoteText ?? ""
                     }
                 })
                .ToListAsync());
            var noteMap = allNotes.GroupBy(x => x.LetterAgreementID).ToDictionary(g => g.Key, g => g.Select(x => x.Data).ToList());

            var result = new List<ReportLetterAgreementDeals>();

            foreach (var letAg in letterAgreements)
            {
                latestStatusMap.TryGetValue(letAg.LetterAgreementID, out var latestStatus);
                sellerMap.TryGetValue(letAg.LetterAgreementID, out var seller);
                referrerMap.TryGetValue(letAg.LetterAgreementID, out var referrer);

                // Retrieve lists from generic maps
                countyMap.TryGetValue(letAg.LetterAgreementID, out var counties);
                unitMap.TryGetValue(letAg.LetterAgreementID, out var units);
                noteMap.TryGetValue(letAg.LetterAgreementID, out var notes);

                var reportItem = new ReportLetterAgreementDeals
                {
                    LetterAgreementID = letAg.LetterAgreementID,
                    ReferrerName = referrer?.ReferrerName ?? "",
                    SellerName = seller?.SellerName ?? "",
                    SellerPhone = seller?.ContactPhone ?? "",
                    CreatedOnDate = letAg.CreatedOn,
                    PurchasePrice = letAg.TotalBonus,
                    ConsiderationFee = letAg.ConsiderationFee,
                    ReferralFee = letAg.ReferralFee,
                    Total = letAg.TotalBonusAndFee,
                    LandMan = letAg.LandMan != null ? $"{letAg.LandMan.FirstName} {letAg.LandMan.LastName}".Trim() : "",
                    DealStatus = latestStatus?.LetterAgreementDealStatus?.StatusName ?? "",

                    // Assign bulk loaded lists
                    Counties = counties ?? new List<ReportLetterAgreementDealsCounty>(),
                    Units = units ?? new List<ReportLetterAgreementDealsUnit>(),
                    Notes = notes ?? new List<ReportLetterAgreementDealsNote>()
                };

                // Populate summary fields used for grouping/sorting
                if (reportItem.Counties.Any())
                {
                    reportItem.CountyName = reportItem.Counties.Count > 1 ? " Multiple" : reportItem.Counties.First().CountyName;
                }
                else
                {
                    reportItem.CountyName = " None";
                }

                result.Add(reportItem);
            }

            return result;
        }

        /// <summary>
        /// Helper method to load data in batches to avoid EF Core 10.0 OPENJSON bug.
        /// Splits a list of IDs into smaller batches and executes the query function for each batch.
        /// Note: All IDs are validated integers, so no SQL injection risk when used with FromSqlRaw.
        /// </summary>
#pragma warning disable EF1002 // Risk of SQL injection - suppressed because IDs are validated integers
        private async Task<List<T>> LoadInBatchesAsync<T>(List<int> ids, Func<List<int>, Task<List<T>>> queryFunc)
        {
            const int batchSize = 100;
            var results = new List<T>();

            for (int i = 0; i < ids.Count; i += batchSize)
            {
                var batch = ids.Skip(i).Take(batchSize).ToList();
                var batchResults = await queryFunc(batch);
                results.AddRange(batchResults);
            }

            return results;
        }
#pragma warning restore EF1002
    }

    public class ReportLetterAgreementDeals
    {
        public int LetterAgreementID { get; set; }
        public string ReferrerName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string SellerPhone { get; set; } = string.Empty;
        public DateTime CreatedOnDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ConsiderationFee { get; set; }
        public decimal? ReferralFee { get; set; }
        public decimal? Total { get; set; }
        public string LandMan { get; set; } = string.Empty;
        public string DealStatus { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;

        // Legacy calculated properties for grouping/sorting
        public string YearMonthValue => CreatedOnDate.ToString("yyyyMM");
        public string MonthNameYear => CreatedOnDate.ToString("MMMM yyyy");

        public List<ReportLetterAgreementDealsCounty> Counties { get; set; } = new();
        public List<ReportLetterAgreementDealsUnit> Units { get; set; } = new();
        public List<ReportLetterAgreementDealsNote> Notes { get; set; } = new();
    }

    public class ReportLetterAgreementDealsCounty
    {
        public string CountyName { get; set; } = string.Empty;
    }

    public class ReportLetterAgreementDealsUnit
    {
        public string UnitName { get; set; } = string.Empty;
        public decimal? UnitInterest { get; set; }
    }

    public class ReportLetterAgreementDealsNote
    {
        public DateTime NoteCreatedOn { get; set; }
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string NoteTypeDesc { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public string NoteCreatedBy => $"{UserFirstName} {UserLastName}".Trim();
    }
}
