using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class RptLetterAgreementDealsRepository
    {
        private readonly SsrDbContext _context;

        public RptLetterAgreementDealsRepository(SsrDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportLetterAgreementDeals>> GetRptLetterAgreementDealsAsync(List<string> letAgIdList)
        {
            if (letAgIdList == null || !letAgIdList.Any())
                return new List<ReportLetterAgreementDeals>();

            var letAgIds = letAgIdList.Select(id => int.TryParse(id, out int i) ? i : 0).Where(i => i > 0).ToList();

            var query = from letAg in _context.LetterAgreements
                        join letAgSt in _context.LetterAgreementStatuses on letAg.LetterAgreementID equals letAgSt.LetterAgreementID into stGroup
                        from letAgSt in stGroup.DefaultIfEmpty()
                        join letAgS in _context.LetterAgreementSellers on letAg.LetterAgreementID equals letAgS.LetterAgreementID into sGroup
                        from letAgS in sGroup.DefaultIfEmpty()
                        join letAgR in _context.LetterAgreementReferrers on letAg.LetterAgreementID equals letAgR.LetterAgreementID into rGroup
                        from letAgR in rGroup.DefaultIfEmpty()
                        join referrer in _context.Referrers on (letAgR != null ? letAgR.ReferrerID : 0) equals referrer.ReferrerID into refGroup
                        from referrer in refGroup.DefaultIfEmpty()
                        join ds in _context.LetterAgreementDealStatuses on (letAgSt != null ? letAgSt.DealStatusCode : "") equals ds.DealStatusCode into dsGroup
                        from ds in dsGroup.DefaultIfEmpty()
                        where letAgIds.Contains(letAg.LetterAgreementID)
                           && (
                               letAgSt == null ||
                               (
                                   letAgSt.StatusDate == (
                                       from letAgSt2 in _context.LetterAgreementStatuses
                                       where letAgSt2.LetterAgreementID == letAgSt.LetterAgreementID
                                       select letAgSt2.StatusDate
                                   ).Max()
                               )
                           )
                        orderby letAg.LetterAgreementID
                        select new ReportLetterAgreementDeals
                        {
                            LetterAgreementID = letAg.LetterAgreementID,
                            ReferrerName = referrer != null ? referrer.ReferrerName ?? "" : "",
                            SellerName = letAgS != null ? letAgS.SellerName ?? "" : "",
                            SellerPhone = letAgS != null ? letAgS.ContactPhone ?? "" : "",
                            CreatedOnDate = letAg.CreatedOn,
                            PurchasePrice = letAg.TotalBonus,
                            ConsiderationFee = letAg.ConsiderationFee,
                            ReferralFee = letAg.ReferralFee,
                            Total = letAg.TotalBonusAndFee,
                            LandMan = letAg.LandMan != null ? (letAg.LandMan!.FirstName + " " + letAg.LandMan!.LastName) : "",
                            DealStatus = ds != null ? ds.StatusName ?? "" : ""
                        };

            var result = await query.ToListAsync();

            foreach (var item in result)
            {
                item.Counties = await GetRptLetterAgreementDealsCountiesAsync(item.LetterAgreementID);
                item.Units = await GetRptLetterAgreementDealsUnitsAsync(item.LetterAgreementID);
                item.Notes = await GetRptLetterAgreementDealsNotesAsync(item.LetterAgreementID);

                // Populate summary fields used for sorting in legacy
                if (item.Counties.Any())
                {
                    item.CountyName = item.Counties.Count > 1 ? " Multiple" : item.Counties.First().CountyName;
                }
                else
                {
                    item.CountyName = " None";
                }
            }

            return result;
        }

        private async Task<List<ReportLetterAgreementDealsCounty>> GetRptLetterAgreementDealsCountiesAsync(int letterAgreementID)
        {
            return await (from lac in _context.LetterAgreementCounties
                          join c in _context.Counties on lac.CountyID equals c.CountyID
                          where lac.LetterAgreementID == letterAgreementID
                          orderby (c.CountyName ?? "").ToUpper()
                          select new ReportLetterAgreementDealsCounty
                          {
                              CountyName = c.CountyName + ", " + c.StateCode
                          }).ToListAsync();
        }

        private async Task<List<ReportLetterAgreementDealsUnit>> GetRptLetterAgreementDealsUnitsAsync(int letterAgreementID)
        {
            return await (from lau in _context.LetterAgreementUnits
                          where lau.LetterAgreementID == letterAgreementID
                          orderby (lau.UnitName ?? "").ToUpper()
                          select new ReportLetterAgreementDealsUnit
                          {
                              UnitName = lau.UnitName ?? "",
                              UnitInterest = lau.UnitInterest
                          }).ToListAsync();
        }

        private async Task<List<ReportLetterAgreementDealsNote>> GetRptLetterAgreementDealsNotesAsync(int letterAgreementID)
        {
            return await (from lan in _context.LetterAgreementNotes
                          join nt in _context.NoteTypes on lan.NoteTypeCode equals nt.NoteTypeCode
                          where lan.LetterAgreementID == letterAgreementID
                          orderby lan.CreatedDateTime
                          select new ReportLetterAgreementDealsNote
                          {
                              NoteCreatedOn = lan.CreatedDateTime,
                              UserFirstName = lan.User.FirstName ?? "",
                              UserLastName = lan.User.LastName ?? "",
                              UserName = lan.User.UserId.ToString(),
                              NoteTypeDesc = nt.NoteTypeDesc ?? "",
                              Note = lan.NoteText ?? ""
                          }).ToListAsync();
        }
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
