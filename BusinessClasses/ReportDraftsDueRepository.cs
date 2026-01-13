using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class ReportDraftsDueRepository
    {
        private readonly SsrDbContext _context;

        public ReportDraftsDueRepository(SsrDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportDraftsDue>> GetRptDraftsDueAsync(List<string> acqIdList, string sortOrder)
        {
            if (acqIdList == null || !acqIdList.Any())
                return new List<ReportDraftsDue>();

            var acqIds = new List<int>();
            foreach (var id in acqIdList)
            {
                if (int.TryParse(id, out int i)) acqIds.Add(i);
            }

            // Using explicit joins for Clarity and Performance, or relying on Navigations in Projection.
            // Using navigations in projection is often cleaner in EF Core.

            var query = from acq in _context.Acquisitions
                        join s in _context.AcquisitionSellers on acq.AcquisitionID equals s.AcquisitionID
                        join b in _context.AcquisitionBuyers on acq.AcquisitionID equals b.AcquisitionID
                        join acqSt in _context.AcquisitionStatuses on acq.AcquisitionID equals acqSt.AcquisitionID into acqStGroup
                        from acqSt in acqStGroup.DefaultIfEmpty()
                        join ds in _context.DealStatuses on (acqSt != null ? acqSt.DealStatusID : 0) equals ds.DealStatusID into dsGroup
                        from ds in dsGroup.DefaultIfEmpty()
                        where acqIds.Contains(acq.AcquisitionID)
                           && acq.PaidDate == null
                           && (
                               acqSt == null ||
                               (
                                   acqSt.StatusDate == (
                                       from acqSt2 in _context.AcquisitionStatuses
                                       where acqSt2.AcquisitionID == acqSt.AcquisitionID
                                       select acqSt2.StatusDate
                                   ).Max()
                                   && (ds == null || ds.ExcludeFromReports == false)
                               )
                           )
                        select new ReportDraftsDue
                        {
                            SellerName = s.SellerName ?? "",
                            DueDate = acq.DueDate,
                            TotalBonus = acq.TotalBonus ?? 0,
                            DraftCheckNumber = acq.DraftCheckNumber ?? "",
                            DealStatus = ds != null ? ds.StatusName ?? "" : "",
                            HaveCheckStub = acq.HaveCheckStub == "Yes",
                            CheckStubDesc = acq.CheckStubDesc ?? "",
                            BuyerName = b.Buyer != null ? b.Buyer.BuyerName ?? "" : "",
                            Assignee = acq.Assignee ?? "",
                            InvoiceNumber = acq.InvoiceNumber ?? "",
                            LandMan = acq.LandMan != null ? (acq.LandMan!.FirstName + " " + acq.LandMan!.LastName) : "",
                            DealStatusID = acqSt != null ? acqSt.DealStatusID : 0,
                            AcquisitionID = acq.AcquisitionID,
                            Counties = new List<ReportDraftsDueCounty>(),
                            Notes = new List<ReportDraftsDueNote>()
                        };

            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "DueDate";

            switch (sortOrder)
            {
                case "SellerName":
                    query = query.OrderBy(q => q.SellerName);
                    break;
                case "DueDate":
                    query = query.OrderBy(q => q.DueDate).ThenBy(q => q.SellerName).ThenBy(q => q.AcquisitionID);
                    break;
                case "LandMan":
                    query = query.OrderBy(q => q.LandMan).ThenBy(q => q.DueDate);
                    break;
            }

            var result = await query.ToListAsync();

            foreach (var item in result)
            {
                item.Counties = await GetRptDraftsDueCountiesAsync(item.AcquisitionID.ToString());
                item.Notes = await GetRptDraftsDueNotesAsync(item.AcquisitionID.ToString());
            }

            return result;
        }


        public async Task<List<ReportDraftsDueCounty>> GetRptDraftsDueCountiesAsync(string acquisitionID)
        {
            if (!int.TryParse(acquisitionID, out int acqId)) return new List<ReportDraftsDueCounty>();

            var query = from ac in _context.AcquisitionCounties
                        join c in _context.Counties on ac.CountyID equals c.CountyID
                        where ac.AcquisitionID == acqId
                        select new ReportDraftsDueCounty
                        {
                            CountyID = c.CountyID,
                            CountyName = c.CountyName ?? "",
                            StateName = c.State != null ? c.State.StateName ?? "" : ""
                        };

            return await query.ToListAsync();
        }

        public async Task<List<ReportDraftsDueNote>> GetRptDraftsDueNotesAsync(string acquisitionID)
        {
            if (!int.TryParse(acquisitionID, out int acqId)) return new List<ReportDraftsDueNote>();

            var query = from an in _context.AcquisitionNotes
                        join nt in _context.NoteTypes on an.NoteTypeCode equals nt.NoteTypeCode
                        // Join User safely? Or rely on Navigation?
                        // an.User is defined.
                        where an.AcquisitionID == acqId
                        orderby an.CreatedDateTime descending
                        select new ReportDraftsDueNote
                        {
                            NoteID = an.AcquisitionNoteID,
                            InputDate = an.CreatedDateTime,
                            // Assuming an.User navigation works. If not, fallback to empty.
                            UserFirstName = an.User != null ? an.User.FirstName ?? "" : "",
                            UserLastName = an.User != null ? an.User.LastName ?? "" : "",
                            UserName = an.User != null ? an.User.UserId.ToString() : "",
                            NoteTypeDesc = nt.NoteTypeDesc ?? "",
                            Note = an.NoteText ?? ""
                        };

            return await query.ToListAsync();
        }
    }

    public class ReportDraftsDue
    {
        public string SellerName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public decimal TotalBonus { get; set; }
        public string DraftCheckNumber { get; set; } = string.Empty;
        public string DealStatus { get; set; } = string.Empty;
        public bool HaveCheckStub { get; set; }
        public string CheckStubDesc { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string Assignee { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string LandMan { get; set; } = string.Empty;
        public int DealStatusID { get; set; }
        public int AcquisitionID { get; set; }

        // Sub-data
        public List<ReportDraftsDueCounty> Counties { get; set; } = new();
        public List<ReportDraftsDueNote> Notes { get; set; } = new();
    }

    public class ReportDraftsDueCounty
    {
        public int CountyID { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public string County => $"{CountyName}, {StateName}";
    }

    public class ReportDraftsDueNote
    {
        public int NoteID { get; set; }
        public DateTime? InputDate { get; set; }
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string NoteTypeDesc { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
