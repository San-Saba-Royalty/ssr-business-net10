using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastReport.Export.PdfSimple;
using SSRBusiness.BusinessClasses; // For ReportDraftsDueRepository and ReportDraftsDue
using SSRBusiness.Reports;

namespace SSRBusiness.BusinessClasses
{
    public class ReportService
    {
        private readonly ReportDraftsDueRepository _draftsDueRepository;
        private readonly RptBuyerInvoicesDueRepository _buyerInvoicesDueRepository;
        private readonly RptCurativeRequirementsRepository _curativeRequirementsRepository;
        private readonly RptLetterAgreementDealsRepository _letterAgreementDealsRepository;
        private readonly RptPurchasesRepository _purchasesRepository;
        private readonly RptReferrer1099SummaryRepository _referrer1099SummaryRepository;

        public ReportService(
            ReportDraftsDueRepository draftsDueRepository,
            RptBuyerInvoicesDueRepository buyerInvoicesDueRepository,
            RptCurativeRequirementsRepository curativeRequirementsRepository,
            RptLetterAgreementDealsRepository letterAgreementDealsRepository,
            RptPurchasesRepository purchasesRepository,
            RptReferrer1099SummaryRepository referrer1099SummaryRepository)
        {
            _draftsDueRepository = draftsDueRepository;
            _buyerInvoicesDueRepository = buyerInvoicesDueRepository;
            _curativeRequirementsRepository = curativeRequirementsRepository;
            _letterAgreementDealsRepository = letterAgreementDealsRepository;
            _purchasesRepository = purchasesRepository;
            _referrer1099SummaryRepository = referrer1099SummaryRepository;
        }

        public async Task<byte[]> RunDraftsDueReportAsync(List<string> acqIdList, string sortOrder)
        {
            var data = await _draftsDueRepository.GetRptDraftsDueAsync(acqIdList, sortOrder);
            decimal totalBonus = data.Sum(x => x.TotalBonus);
            var report = DraftsDueReport.Create(data, totalBonus, true, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            return ExportToPdf(report);
        }

        public async Task<byte[]> RunBuyerInvoicesDueReportAsync(List<string> acqIdList)
        {
            var data = await _buyerInvoicesDueRepository.GetRptInvoicesDueAsync(acqIdList);
            var report = BuyerInvoicesDueReport.Create(data, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            return ExportToPdf(report);
        }

        public async Task<byte[]> RunCurativeRequirementsReportAsync(List<string> acqIdList)
        {
            var data = await _curativeRequirementsRepository.GetRptCurativeRequirementsAsync(acqIdList);
            var report = CurativeRequirementsReport.Create(data, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            return ExportToPdf(report);
        }

        public async Task<byte[]> RunLetterAgreementDealsReportAsync(List<string> acqIdList)
        {
            var data = await _letterAgreementDealsRepository.GetRptLetterAgreementDealsAsync(acqIdList);
            var report = LetterAgreementDealsReport.Create(data, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            return ExportToPdf(report);
        }

        public async Task<byte[]> RunPurchasesReportAsync(List<string> acqIdList)
        {
            var data = await _purchasesRepository.GetRptPurchasesAsync(acqIdList);
            var report = PurchasesReport.Create(data, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            return ExportToPdf(report);
        }

        public async Task<byte[]> RunReferrer1099SummaryReportAsync(string currentUserID, List<string> acqIdList)
        {
            var data = await _referrer1099SummaryRepository.GetRptReferrer1099SummaryAsync(currentUserID, acqIdList);
            var report = Referrer1099SummaryReport.Create(data, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
            return ExportToPdf(report);
        }

        private byte[] ExportToPdf(FastReport.Report report)
        {
            report.Prepare();
            using (MemoryStream ms = new MemoryStream())
            {
                var pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                return ms.ToArray();
            }
        }
    }
}
