using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class RptBuyerInvoicesDueRepository
    {
        private readonly SsrDbContext _context;

        public RptBuyerInvoicesDueRepository(SsrDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportInvoicesDue>> GetRptInvoicesDueAsync(List<string> acqIdList)
        {
            if (acqIdList == null || !acqIdList.Any())
                return new List<ReportInvoicesDue>();

            var acqIds = acqIdList.Select(id => int.TryParse(id, out int i) ? i : 0).Where(i => i > 0).ToList();

            var query = from acq in _context.Acquisitions
                        join s in _context.AcquisitionSellers on acq.AcquisitionID equals s.AcquisitionID
                        join b in _context.AcquisitionBuyers on acq.AcquisitionID equals b.AcquisitionID
                        join buyer in _context.Buyers on b.BuyerID equals buyer.BuyerID
                        join acqSt in _context.AcquisitionStatuses on acq.AcquisitionID equals acqSt.AcquisitionID into acqStGroup
                        from acqSt in acqStGroup.DefaultIfEmpty()
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
                        orderby (buyer.BuyerName ?? "").ToUpper(), acq.InvoiceNumber, acq.AcquisitionID
                        select new ReportInvoicesDue
                        {
                            AcquisitionID = acq.AcquisitionID,
                            BuyerName = buyer.BuyerName ?? "",
                            BuyerAcquisitionNumber = acq.AcquisitionNumber ?? "",
                            InvoiceNumber = acq.InvoiceNumber ?? "",
                            SellerName = s.SellerName ?? "",
                            DueDate = acq.DueDate,
                            InvoiceDueDate = acq.InvoiceDueDate,
                            PaidDate = acq.PaidDate,
                            TotalBonus = acq.PaidDate == null ? null : acq.TotalBonus,
                            InvoiceTotal = acq.InvoiceTotal,
                            LandMan = acq.LandMan != null ? (acq.LandMan!.FirstName + " " + acq.LandMan!.LastName) : "",
                            DealStatus = ds != null ? ds.StatusName ?? "" : ""
                        };

            var result = await query.ToListAsync();

            foreach (var item in result)
            {
                item.Counties = await GetRptInvoicesDueCountiesAsync(item.AcquisitionID);
                item.Operators = await GetRptInvoicesDueOperatorsAsync(item.AcquisitionID);
                item.Units = await GetRptInvoicesDueUnitsAsync(item.AcquisitionID);
                item.Notes = await GetRptInvoicesDueNotesAsync(item.AcquisitionID);
            }

            return result;
        }

        public async Task<List<ReportInvoicesDueCounty>> GetRptInvoicesDueCountiesAsync(int acquisitionID)
        {
            return await (from ac in _context.AcquisitionCounties
                          join c in _context.Counties on ac.CountyID equals c.CountyID
                          where ac.AcquisitionID == acquisitionID
                          orderby (c.CountyName ?? "").ToUpper()
                          select new ReportInvoicesDueCounty
                          {
                              CountyName = c.CountyName + ", " + c.StateCode
                          }).ToListAsync();
        }

        public async Task<List<ReportInvoicesDueOperator>> GetRptInvoicesDueOperatorsAsync(int acquisitionID)
        {
            return await (from ao in _context.AcquisitionOperators
                          join o in _context.Operators on ao.OperatorID equals o.OperatorID
                          where ao.AcquisitionID == acquisitionID
                          orderby (o.OperatorName ?? "").ToUpper()
                          select new ReportInvoicesDueOperator
                          {
                              OperatorName = o.OperatorName ?? "",
                              DOReceivedDate = ao.DOReceivedDate
                          }).ToListAsync();
        }

        public async Task<List<ReportInvoicesDueUnit>> GetRptInvoicesDueUnitsAsync(int acquisitionID)
        {
            return await (from au in _context.AcquisitionUnits
                          where au.AcquisitionID == acquisitionID
                          orderby (au.UnitName ?? "").ToUpper()
                          select new ReportInvoicesDueUnit
                          {
                              UnitName = au.UnitName ?? "",
                              UnitInterest = au.UnitInterest,
                              InPay = au.SsrInPay == "Y" ? "Yes" : (au.SsrInPay == "N" ? "No" : "")
                          }).ToListAsync();
        }

        public async Task<List<ReportInvoicesDueNote>> GetRptInvoicesDueNotesAsync(int acquisitionID)
        {
            return await (from an in _context.AcquisitionNotes
                          join nt in _context.NoteTypes on an.NoteTypeCode equals nt.NoteTypeCode
                          where an.AcquisitionID == acquisitionID
                          orderby an.CreatedDateTime
                          select new ReportInvoicesDueNote
                          {
                              NoteCreatedOn = an.CreatedDateTime,
                              UserFirstName = an.User != null ? an.User.FirstName ?? "" : "",
                              UserLastName = an.User != null ? an.User.LastName ?? "" : "",
                              UserName = an.User != null ? an.User.UserId.ToString() : "",
                              NoteTypeDesc = nt.NoteTypeDesc ?? "",
                              Note = an.NoteText ?? ""
                          }).ToListAsync();
        }
    }

    public class ReportInvoicesDue
    {
        public int AcquisitionID { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerAcquisitionNumber { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? TotalBonus { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public string LandMan { get; set; } = string.Empty;
        public string DealStatus { get; set; } = string.Empty;

        public List<ReportInvoicesDueCounty> Counties { get; set; } = new();
        public List<ReportInvoicesDueOperator> Operators { get; set; } = new();
        public List<ReportInvoicesDueUnit> Units { get; set; } = new();
        public List<ReportInvoicesDueNote> Notes { get; set; } = new();
    }

    public class ReportInvoicesDueCounty
    {
        public string CountyName { get; set; } = string.Empty;
    }

    public class ReportInvoicesDueOperator
    {
        public string OperatorName { get; set; } = string.Empty;
        public DateTime? DOReceivedDate { get; set; }
    }

    public class ReportInvoicesDueUnit
    {
        public string UnitName { get; set; } = string.Empty;
        public decimal? UnitInterest { get; set; }
        public string InPay { get; set; } = string.Empty;
    }

    public class ReportInvoicesDueNote
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
