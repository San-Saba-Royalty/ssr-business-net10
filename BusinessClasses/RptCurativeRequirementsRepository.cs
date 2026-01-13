using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses
{
    public class RptCurativeRequirementsRepository
    {
        private readonly SsrDbContext _context;

        public RptCurativeRequirementsRepository(SsrDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportCurativeRequirement>> GetRptCurativeRequirementsAsync(List<string> acqIdList)
        {
            if (acqIdList == null || !acqIdList.Any())
                return new List<ReportCurativeRequirement>();

            var acqIds = acqIdList.Select(id => int.TryParse(id, out int i) ? i : 0).Where(i => i > 0).ToList();
            if (!acqIds.Any()) return new List<ReportCurativeRequirement>();

            // 1. Fetch Main Data
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
                        orderby buyer.BuyerName.ToUpper(), acq.InvoiceNumber, acq.AcquisitionID
                        select new ReportCurativeRequirement
                        {
                            AcquisitionID = acq.AcquisitionID,
                            BuyerName = buyer.BuyerName ?? "",
                            BuyerAcquisitionNumber = acq.AcquisitionNumber ?? "",
                            InvoiceNumber = acq.InvoiceNumber ?? "",
                            SellerName = s.SellerName ?? "",
                            InvoiceDueDate = acq.InvoiceDueDate,
                            TitleOpinionReceivedDate = acq.TitleOpinionReceivedDate,
                            LandMan = acq.LandMan != null ? (acq.LandMan!.FirstName + " " + acq.LandMan!.LastName) : "",
                            InvoiceTotal = acq.InvoiceTotal,
                            DealStatus = ds != null ? ds.StatusName ?? "" : ""
                        };

            var result = await query.ToListAsync();
            var distinctIds = result.Select(r => r.AcquisitionID).Distinct().ToList();

            if (distinctIds.Any()) 
            {
                // 2. Batch Fetch Sub-Entities
                var countiesCheck = await GetBatchRptCurativeRequirementsCountiesAsync(distinctIds);
                var operatorsCheck = await GetBatchRptCurativeRequirementsOperatorsAsync(distinctIds);
                var unitsCheck = await GetBatchRptCurativeRequirementsUnitsAsync(distinctIds);
                var curativesCheck = await GetBatchRptCurativeRequirementsCurativesAsync(distinctIds);

                var countiesLookup = countiesCheck.ToLookup(x => x.AcquisitionID, x => x.Data);
                var operatorsLookup = operatorsCheck.ToLookup(x => x.AcquisitionID, x => x.Data);
                var unitsLookup = unitsCheck.ToLookup(x => x.AcquisitionID, x => x.Data);
                var curativesLookup = curativesCheck.ToLookup(x => x.AcquisitionID, x => x.Data);

                // 3. Assemble
                foreach (var item in result)
                {
                    item.Counties = countiesLookup[item.AcquisitionID].ToList();
                    item.Operators = operatorsLookup[item.AcquisitionID].ToList();
                    item.Units = unitsLookup[item.AcquisitionID].ToList();
                    item.Curatives = curativesLookup[item.AcquisitionID].ToList();
                }
            }

            return result;
        }

        private async Task<List<(int AcquisitionID, ReportCurativeRequirementCounty Data)>> GetBatchRptCurativeRequirementsCountiesAsync(List<int> acquisitionIds)
        {
            return await (from ac in _context.AcquisitionCounties
                          join c in _context.Counties on ac.CountyID equals c.CountyID
                          where acquisitionIds.Contains(ac.AcquisitionID)
                          orderby (c.CountyName ?? "").ToUpper()
                          select new 
                          { 
                              ac.AcquisitionID, 
                              Data = new ReportCurativeRequirementCounty { CountyName = c.CountyName + ", " + c.StateCode } 
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.Data)).ToList());
        }

        private async Task<List<(int AcquisitionID, ReportCurativeRequirementOperator Data)>> GetBatchRptCurativeRequirementsOperatorsAsync(List<int> acquisitionIds)
        {
            return await (from ao in _context.AcquisitionOperators
                          join o in _context.Operators on ao.OperatorID equals o.OperatorID
                          where acquisitionIds.Contains(ao.AcquisitionID)
                          orderby (o.OperatorName ?? "").ToUpper()
                          select new 
                          {
                              ao.AcquisitionID,
                              Data = new ReportCurativeRequirementOperator
                              {
                                  OperatorName = o.OperatorName ?? "",
                                  DOReceivedDate = ao.DOReceivedDate
                              }
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.Data)).ToList());
        }

        private async Task<List<(int AcquisitionID, ReportCurativeRequirementUnit Data)>> GetBatchRptCurativeRequirementsUnitsAsync(List<int> acquisitionIds)
        {
            return await (from au in _context.AcquisitionUnits
                          where acquisitionIds.Contains(au.AcquisitionID)
                          orderby (au.UnitName ?? "").ToUpper()
                          select new 
                          {
                              au.AcquisitionID,
                              Data = new ReportCurativeRequirementUnit
                              {
                                  UnitName = au.UnitName ?? "",
                                  UnitInterest = au.UnitInterest,
                                  InPay = au.SsrInPay == "Y" ? "Yes" : (au.SsrInPay == "N" ? "No" : "")
                              }
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.Data)).ToList());
        }

        private async Task<List<(int AcquisitionID, ReportCurativeRequirementCurative Data)>> GetBatchRptCurativeRequirementsCurativesAsync(List<int> acquisitionIds)
        {
            return await (from cur in _context.AcqCurativeRequirements
                          join ct in _context.CurativeTypes on cur.CurativeTypeID equals ct.CurativeTypeID
                          where acquisitionIds.Contains(cur.AcquisitionID)
                          orderby (ct.CurativeTypeName ?? "").ToUpper(), cur.CompletedDate
                          select new 
                          {
                              cur.AcquisitionID,
                              Data = new ReportCurativeRequirementCurative
                              {
                                  CurativeTypeName = ct.CurativeTypeName ?? "",
                                  CompletedDate = cur.CompletedDate,
                                  Notes = cur.Notes ?? ""
                              }
                          })
                          .ToListAsync()
                          .ContinueWith(t => t.Result.Select(x => (x.AcquisitionID, x.Data)).ToList());
        }
    }

    public class ReportCurativeRequirement
    {
        public int AcquisitionID { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerAcquisitionNumber { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public DateTime? InvoiceDueDate { get; set; }
        public DateTime? TitleOpinionReceivedDate { get; set; }
        public string LandMan { get; set; } = string.Empty;
        public decimal? InvoiceTotal { get; set; }
        public string DealStatus { get; set; } = string.Empty;

        public List<ReportCurativeRequirementCounty> Counties { get; set; } = new();
        public List<ReportCurativeRequirementOperator> Operators { get; set; } = new();
        public List<ReportCurativeRequirementUnit> Units { get; set; } = new();
        public List<ReportCurativeRequirementCurative> Curatives { get; set; } = new();
    }

    public class ReportCurativeRequirementCounty
    {
        public string CountyName { get; set; } = string.Empty;
    }

    public class ReportCurativeRequirementOperator
    {
        public string OperatorName { get; set; } = string.Empty;
        public DateTime? DOReceivedDate { get; set; }
    }

    public class ReportCurativeRequirementUnit
    {
        public string UnitName { get; set; } = string.Empty;
        public decimal? UnitInterest { get; set; }
        public string InPay { get; set; } = string.Empty;
    }

    public class ReportCurativeRequirementCurative
    {
        public string CurativeTypeName { get; set; } = string.Empty;
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
