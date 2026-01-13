using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class RptPurchasesRepository
    {
        private readonly SsrDbContext _context;

        public RptPurchasesRepository(SsrDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportPurchases>> GetRptPurchasesAsync(List<string> acqIdList)
        {
            if (acqIdList == null || !acqIdList.Any())
                return new List<ReportPurchases>();

            var acqIds = acqIdList.Select(id => int.TryParse(id, out int i) ? i : 0).Where(i => i > 0).ToList();
            if (!acqIds.Any()) return new List<ReportPurchases>();

            // 1. Fetch Main Data
            var query = from acq in _context.Acquisitions
                        join acqSt in _context.AcquisitionStatuses on acq.AcquisitionID equals acqSt.AcquisitionID into stGroup
                        from acqSt in stGroup.DefaultIfEmpty()
                        join acqB in _context.AcquisitionBuyers on acq.AcquisitionID equals acqB.AcquisitionID into bGroup
                        from acqB in bGroup.DefaultIfEmpty()
                        join buyer in _context.Buyers on (acqB != null ? acqB.BuyerID : 0) equals buyer.BuyerID into buyGroup
                        from buyer in buyGroup.DefaultIfEmpty()
                        join acqS in _context.AcquisitionSellers on acq.AcquisitionID equals acqS.AcquisitionID into sGroup
                        from acqS in sGroup.DefaultIfEmpty()
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
                        orderby acq.AcquisitionID
                        select new ReportPurchases
                        {
                            AcquisitionID = acq.AcquisitionID,
                            BuyerName = buyer != null ? buyer.BuyerName ?? "" : "",
                            SellerName = acqS != null ? acqS.SellerName ?? "" : "",
                            EffectiveDate = acq.EffectiveDate,
                            DueDate = acq.InvoiceDueDate,
                            PaidDate = acq.PaidDate,
                            TotalBonus = acq.TotalBonus,
                            InvoiceTotal = acq.InvoiceTotal,
                            Commission = acq.Commission,
                            LandMan = acq.LandMan != null ? (acq.LandMan!.FirstName + " " + acq.LandMan!.LastName) : "",
                            DealStatus = ds != null ? ds.StatusName ?? "" : ""
                        };

            var result = await query.ToListAsync();
            var distinctIds = result.Select(r => r.AcquisitionID).Distinct().ToList();

            if (distinctIds.Any())
            {
                // 2. Batch Fetch Sub-Entities
                var countiesCheck = await GetBatchRptPurchasesCountiesAsync(distinctIds);
                var unitsCheck = await GetBatchRptPurchasesUnitsAsync(distinctIds);
                var notesCheck = await GetBatchRptPurchasesNotesAsync(distinctIds);

                var countiesLookup = countiesCheck.ToLookup(x => x.AcquisitionID, x => x.County);
                var unitsLookup = unitsCheck.ToLookup(x => x.AcquisitionID, x => x.Unit);
                var notesLookup = notesCheck.ToLookup(x => x.AcquisitionID, x => x.Note);

                // 3. Assemble in Memory
                foreach (var item in result)
                {
                    item.Counties = countiesLookup[item.AcquisitionID].ToList();
                    item.Units = unitsLookup[item.AcquisitionID].ToList();
                    item.Notes = notesLookup[item.AcquisitionID].ToList();

                    if (item.Counties.Any())
                    {
                        item.CountyName = item.Counties.Count > 1 ? " Multiple" : item.Counties.First().CountyName;
                    }
                    else
                    {
                        item.CountyName = " None";
                    }
                }
            }

            return result;
        }

        private async Task<List<(int AcquisitionID, ReportPurchasesCounty County)>> GetBatchRptPurchasesCountiesAsync(List<int> acquisitionIds)
        {
            return await (from ac in _context.AcquisitionCounties
                          join c in _context.Counties on ac.CountyID equals c.CountyID
                          where acquisitionIds.Contains(ac.AcquisitionID)
                          orderby (c.CountyName ?? "").ToUpper()
                          select new 
                          { 
                              ac.AcquisitionID, 
                              County = new ReportPurchasesCounty { CountyName = c.CountyName + ", " + c.StateCode } 
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.County)).ToList());
        }

        private async Task<List<(int AcquisitionID, ReportPurchasesUnit Unit)>> GetBatchRptPurchasesUnitsAsync(List<int> acquisitionIds)
        {
            return await (from au in _context.AcquisitionUnits
                          where acquisitionIds.Contains(au.AcquisitionID)
                          orderby (au.UnitName ?? "").ToUpper()
                          select new 
                          { 
                              au.AcquisitionID, 
                              Unit = new ReportPurchasesUnit 
                              { 
                                  UnitName = au.UnitName ?? "",
                                  UnitInterest = au.UnitInterest,
                                  InPay = au.SsrInPay == "Y" ? "Yes" : (au.SsrInPay == "N" ? "No" : "")
                              }
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.Unit)).ToList());
        }

        private async Task<List<(int AcquisitionID, ReportPurchasesNote Note)>> GetBatchRptPurchasesNotesAsync(List<int> acquisitionIds)
        {
            return await (from lan in _context.AcquisitionNotes
                          join nt in _context.NoteTypes on lan.NoteTypeCode equals nt.NoteTypeCode
                          where acquisitionIds.Contains(lan.AcquisitionID)
                          orderby lan.CreatedDateTime
                          select new 
                          {
                              lan.AcquisitionID,
                              Note = new ReportPurchasesNote
                              {
                                  NoteCreatedOn = lan.CreatedDateTime,
                                  UserFirstName = lan.User != null ? lan.User.FirstName ?? "" : "",
                                  UserLastName = lan.User != null ? lan.User.LastName ?? "" : "",
                                  UserName = lan.User != null ? lan.User.UserId.ToString() : "",
                                  NoteTypeDesc = nt.NoteTypeDesc ?? "",
                                  Note = lan.NoteText ?? ""
                              }
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.Note)).ToList());
        }
    }

    public class ReportPurchases
    {
        public int AcquisitionID { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public DateTime? EffectiveDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? TotalBonus { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public decimal? Commission { get; set; }
        public string LandMan { get; set; } = string.Empty;
        public string DealStatus { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;

        public string YearMonthValue => EffectiveDate?.ToString("yyyyMM") ?? "";
        public string MonthNameYear => EffectiveDate?.ToString("MMMM yyyy") ?? "";

        public List<ReportPurchasesCounty> Counties { get; set; } = new();
        public List<ReportPurchasesUnit> Units { get; set; } = new();
        public List<ReportPurchasesNote> Notes { get; set; } = new();
    }

    public class ReportPurchasesCounty
    {
        public string CountyName { get; set; } = string.Empty;
    }

    public class ReportPurchasesUnit
    {
        public string UnitName { get; set; } = string.Empty;
        public decimal? UnitInterest { get; set; }
        public string InPay { get; set; } = string.Empty;
    }

    public class ReportPurchasesNote
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
