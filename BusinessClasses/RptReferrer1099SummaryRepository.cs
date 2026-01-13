using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class RptReferrer1099SummaryRepository
    {
        private readonly SsrDbContext _context;

        // Legacy hardcoded list of users who can see SSNs
        private static readonly List<string> SsnUsers = new() { "2", "3", "4", "7", "8", "9" };

        public RptReferrer1099SummaryRepository(SsrDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportReferrer1099Summary>> GetRptReferrer1099SummaryAsync(string currentUserID, List<string> acqIdList)
        {
            if (acqIdList == null || !acqIdList.Any())
                return new List<ReportReferrer1099Summary>();

            var acqIds = acqIdList.Select(id => int.TryParse(id, out int i) ? i : 0).Where(i => i > 0).ToList();

            var query = from acq in _context.Acquisitions
                        join acqS in _context.AcquisitionSellers on acq.AcquisitionID equals acqS.AcquisitionID
                        join acqAR in _context.AcquisitionReferrers on acq.AcquisitionID equals acqAR.AcquisitionID
                        join acqR in _context.Referrers on acqAR.ReferrerID equals acqR.ReferrerID
                        join acqSt in _context.AcquisitionStatuses on acq.AcquisitionID equals acqSt.AcquisitionID into stGroup
                        from acqSt in stGroup.DefaultIfEmpty()
                        join ds in _context.DealStatuses on (acqSt != null ? acqSt.DealStatusID : 0) equals ds.DealStatusID into dsGroup
                        from ds in dsGroup.DefaultIfEmpty()
                        where acqIds.Contains(acq.AcquisitionID)
                           && (
                               acqSt == null ||
                               (
                                   acqSt.StatusDate == (
                                       from acqSt2 in _context.AcquisitionStatuses
                                       where acqSt2.AcquisitionID == acqSt.AcquisitionID
                                       select acqSt2.StatusDate
                                   ).Max()
                               )
                           )
                        orderby (acqR.ReferrerName ?? "").ToUpper(), acq.AcquisitionID
                        select new ReportReferrer1099Summary
                        {
                            ReferrerID = acqR.ReferrerID,
                            ReferrerName = acqR.ReferrerName ?? "",
                            ReferrerTypeCode = acqR.ReferrerTypeCode ?? "",
                            ReferrerTaxID = acqR.ReferrerTaxID ?? "",
                            ReferrerContactName = acqR.ContactName ?? "",
                            ReferrerContactPhone = acqR.ContactPhone ?? "",
                            ReferrerContactFax = "",
                            ReferrerAttention = "",
                            ReferrerAddressLine1 = acqR.AddressLine1 ?? "",
                            ReferrerAddressLine2 = "",
                            ReferrerAddressLine3 = "",
                            ReferrerCity = acqR.City ?? "",
                            ReferrerStateCode = acqR.StateCode ?? "",
                            ReferrerZipCode = acqR.ZipCode ?? "",
                            AcquisitionID = acq.AcquisitionID,
                            SellerName = acqS.SellerName ?? "",
                            DueDate = acq.InvoiceDueDate,
                            PaidDate = acq.PaidDate,
                            TotalBonus = acq.TotalBonus,
                            TotalBonusAndFee = acq.TotalBonusAndFee,
                            ReferralFee = acq.ReferralFee,
                            LandMan = acq.LandMan != null ? (acq.LandMan!.FirstName + " " + acq.LandMan!.LastName) : "",
                            DealStatus = ds != null ? ds.StatusName ?? "" : ""
                        };

            var result = await query.ToListAsync();

            bool canSeeSsn = SsnUsers.Contains(currentUserID);

            foreach (var item in result)
            {
                // Logic for masking TaxID
                if (item.ReferrerTypeCode == "SSN" && !canSeeSsn)
                {
                    item.ReferrerTaxID = "xxx-xx-xxxx";
                }

                item.Counties = await GetRptReferrer1099SummaryCountiesAsync(item.AcquisitionID);
                item.Operators = await GetRptReferrer1099SummaryOperatorsAsync(item.AcquisitionID);
                item.Units = await GetRptReferrer1099SummaryUnitsAsync(item.AcquisitionID);
                item.Notes = await GetRptReferrer1099SummaryNotesAsync(item.AcquisitionID);
            }

            return result;
        }

        private async Task<List<ReportReferrer1099SummaryCounty>> GetRptReferrer1099SummaryCountiesAsync(int acquisitionID)
        {
            return await (from ac in _context.AcquisitionCounties
                          join c in _context.Counties on ac.CountyID equals c.CountyID
                          where ac.AcquisitionID == acquisitionID
                          orderby (c.CountyName ?? "").ToUpper()
                          select new ReportReferrer1099SummaryCounty
                          {
                              CountyName = c.CountyName + ", " + c.StateCode
                          }).ToListAsync();
        }

        private async Task<List<ReportReferrer1099SummaryOperator>> GetRptReferrer1099SummaryOperatorsAsync(int acquisitionID)
        {
            return await (from ao in _context.AcquisitionOperators
                          join o in _context.Operators on ao.OperatorID equals o.OperatorID
                          where ao.AcquisitionID == acquisitionID
                          orderby (o.OperatorName ?? "").ToUpper()
                          select new ReportReferrer1099SummaryOperator
                          {
                              OperatorName = o.OperatorName ?? ""
                          }).ToListAsync();
        }

        private async Task<List<ReportReferrer1099SummaryUnit>> GetRptReferrer1099SummaryUnitsAsync(int acquisitionID)
        {
            return await (from au in _context.AcquisitionUnits
                          where au.AcquisitionID == acquisitionID
                          orderby (au.UnitName ?? "").ToUpper()
                          select new ReportReferrer1099SummaryUnit
                          {
                              UnitName = au.UnitName ?? "",
                              UnitInterest = au.UnitInterest,
                              InPay = au.SsrInPay == "Y" ? "Yes" : (au.SsrInPay == "N" ? "No" : "")
                          }).ToListAsync();
        }

        private async Task<List<ReportReferrer1099SummaryNote>> GetRptReferrer1099SummaryNotesAsync(int acquisitionID)
        {
            return await (from lan in _context.AcquisitionNotes
                          join nt in _context.NoteTypes on lan.NoteTypeCode equals nt.NoteTypeCode
                          where lan.AcquisitionID == acquisitionID
                          orderby lan.CreatedDateTime
                          select new ReportReferrer1099SummaryNote
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

    public class ReportReferrer1099Summary
    {
        public int ReferrerID { get; set; }
        public string ReferrerName { get; set; } = string.Empty;
        public string ReferrerTypeCode { get; set; } = string.Empty;
        public string ReferrerTaxID { get; set; } = string.Empty;
        public string ReferrerContactName { get; set; } = string.Empty;
        public string ReferrerContactPhone { get; set; } = string.Empty;
        public string ReferrerContactFax { get; set; } = string.Empty;
        public string ReferrerContactEmail { get; set; } = string.Empty;
        public string ReferrerAttention { get; set; } = string.Empty;
        public string ReferrerAddressLine1 { get; set; } = string.Empty;
        public string ReferrerAddressLine2 { get; set; } = string.Empty;
        public string ReferrerAddressLine3 { get; set; } = string.Empty;
        public string ReferrerCity { get; set; } = string.Empty;
        public string ReferrerStateCode { get; set; } = string.Empty;
        public string ReferrerZipCode { get; set; } = string.Empty;

        public int AcquisitionID { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? TotalBonus { get; set; }
        public decimal? ReferralFee { get; set; }
        public decimal? TotalBonusAndFee { get; set; }
        public string LandMan { get; set; } = string.Empty;
        public string DealStatus { get; set; } = string.Empty;

        public List<ReportReferrer1099SummaryCounty> Counties { get; set; } = new();
        public List<ReportReferrer1099SummaryOperator> Operators { get; set; } = new();
        public List<ReportReferrer1099SummaryUnit> Units { get; set; } = new();
        public List<ReportReferrer1099SummaryNote> Notes { get; set; } = new();
    }

    public class ReportReferrer1099SummaryCounty
    {
        public string CountyName { get; set; } = string.Empty;
    }

    public class ReportReferrer1099SummaryOperator
    {
        public string OperatorName { get; set; } = string.Empty;
    }

    public class ReportReferrer1099SummaryUnit
    {
        public string UnitName { get; set; } = string.Empty;
        public decimal? UnitInterest { get; set; }
        public string InPay { get; set; } = string.Empty;
    }

    public class ReportReferrer1099SummaryNote
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
