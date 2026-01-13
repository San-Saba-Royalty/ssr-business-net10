using System;
using System.Collections.Generic;
using FastReport;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Format;
using SSRBusiness.Entities;
using SSRBusiness.BusinessClasses;
using System.Drawing;
using Report = FastReport.Report;

namespace SSRBusiness.Reports
{
    public class DraftsDueReport
    {
        public static Report Create(List<ReportDraftsDue> data, decimal totalBonus, bool includeNotes, string runDate)
        {
            var report = new Report();

            // Register Data
            report.RegisterData(data, "DraftsDue");

            var page = new ReportPage();
            page.Name = "Page1";
            page.PaperWidth = 279.4f; // 11 inches (Landscape)
            page.PaperHeight = 215.9f; // 8.5 inches
            page.Landscape = true;
            page.LeftMargin = 10;
            page.RightMargin = 10;
            page.TopMargin = 10;
            page.BottomMargin = 10;
            report.Pages.Add(page);

            // 1. Report Header

            // 2. Page Header
            var pageHeader = new PageHeaderBand();
            pageHeader.Name = "PageHeader1";
            pageHeader.Height = Units.Inches * 0.9f;
            page.PageHeader = pageHeader;

            // Header Labels
            CreateLabel(pageHeader, "Drafts Due", 0.13f, 0, 8.0f, 0.35f, 18, FontStyle.Regular, Color.FromArgb(28, 58, 112));
            CreateLabel(pageHeader, runDate, 11.0f, 0, 1.8f, 0.2f, 8, FontStyle.Regular, Color.Black).HorzAlign = HorzAlign.Right;

            // Column Headers
            float headerTop = 0.46f;
            Color headerBg = Color.FromArgb(28, 58, 112);
            Color headerFg = Color.White;

            CreateLabel(pageHeader, "Seller", 0.13f, headerTop, 1.64f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Due Date", 1.82f, headerTop, 0.71f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Amount", 2.59f, headerTop, 1.05f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg).HorzAlign = HorzAlign.Right;
            CreateLabel(pageHeader, "Draft Check #", 3.70f, headerTop, 1.19f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Deal Status", 4.94f, headerTop, 1.25f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Check Stub", 6.24f, headerTop, 0.43f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Buyer", 6.74f, headerTop, 1.75f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Assignee", 8.54f, headerTop, 1.16f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Inv #", 9.76f, headerTop, 0.46f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "County", 10.27f, headerTop, 1.31f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "LandMan", 11.64f, headerTop, 1.29f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);

            // 3. Data Band
            var dataBand = new DataBand();
            dataBand.Name = "Data1";
            dataBand.Height = Units.Inches * 1.1f;
            dataBand.DataSource = report.GetDataSource("DraftsDue");
            page.Bands.Add(dataBand);

            // Data Fields
            float dataTop = 0.07f;
            CreateText(dataBand, "[DraftsDue.SellerName]", 0.13f, dataTop, 1.64f, 0.2f);
            // Removed explicit Format for Date to avoid constructor error. FastReport handles default date format or we can use expression in string.
            CreateText(dataBand, "[DraftsDue.DueDate]", 1.82f, dataTop, 0.71f, 0.2f).Format = new DateFormat();
            CreateText(dataBand, "[DraftsDue.TotalBonus]", 2.59f, dataTop, 1.05f, 0.2f).HorzAlign = HorzAlign.Right;
            CreateText(dataBand, "[DraftsDue.DraftCheckNumber]", 3.70f, dataTop, 1.19f, 0.2f);
            CreateText(dataBand, "[DraftsDue.DealStatus]", 4.94f, dataTop, 1.25f, 0.2f);
            CreateText(dataBand, "[DraftsDue.HaveCheckStub]", 6.24f, dataTop, 0.43f, 0.2f);
            CreateText(dataBand, "[DraftsDue.BuyerName]", 6.74f, dataTop, 1.75f, 0.2f);
            CreateText(dataBand, "[DraftsDue.Assignee]", 8.54f, dataTop, 1.16f, 0.2f);
            CreateText(dataBand, "[DraftsDue.InvoiceNumber]", 9.76f, dataTop, 0.46f, 0.2f);
            CreateText(dataBand, "[DraftsDue.LandMan]", 11.64f, dataTop, 1.29f, 0.2f);

            if (includeNotes)
            {
                // Placeholder
            }

            // 4. Report Summary (Footer)
            // Use ReportSummaryBand if available.
            var reportSummary = new ReportSummaryBand();
            reportSummary.Name = "ReportSummary1";
            reportSummary.Height = Units.Inches * 0.3f;
            page.ReportSummary = reportSummary;

            CreateLabel(reportSummary, "Total Amount", 0.13f, 0, 1.31f, 0.2f, 9, FontStyle.Bold, Color.White, Color.FromArgb(96, 117, 155));

            // Total Sum Display (Passed as parameter)
            var totalContext = CreateText(reportSummary, totalBonus.ToString("C2"), 1.82f, 0, 1.83f, 0.2f);
            totalContext.HorzAlign = HorzAlign.Right;
            totalContext.Fill = new FastReport.SolidFill(Color.FromArgb(96, 117, 155));
            totalContext.TextColor = Color.White;
            totalContext.Font = new Font("Tahoma", 9, FontStyle.Bold);

            // 5. Page Footer
            var pageFooter = new PageFooterBand();
            pageFooter.Name = "PageFooter1";
            pageFooter.Height = Units.Inches * 0.3f;
            page.PageFooter = pageFooter;

            CreateText(pageFooter, "[PageN]", 12.46f, 0.07f, 0.5f, 0.2f).HorzAlign = HorzAlign.Right;

            return report;
        }

        private static TextObject CreateLabel(BandBase band, string text, float left, float top, float width, float height, int fontSize, FontStyle fontStyle, Color color, Color? backColor = null)
        {
            var textObj = new TextObject();
            textObj.Text = text;
            textObj.Left = Units.Inches * left;
            textObj.Top = Units.Inches * top;
            textObj.Width = Units.Inches * width;
            textObj.Height = Units.Inches * height;
            textObj.Font = new Font("Tahoma", fontSize, fontStyle);
            textObj.TextColor = color;
            if (backColor.HasValue)
            {
                textObj.Fill = new FastReport.SolidFill(backColor.Value);
            }
            band.Objects.Add(textObj);
            return textObj;
        }

        private static TextObject CreateText(BandBase band, string expression, float left, float top, float width, float height)
        {
            var textObj = new TextObject();
            textObj.Text = expression;
            textObj.Bounds = new RectangleF(Units.Inches * left, Units.Inches * top, Units.Inches * width, Units.Inches * height);
            textObj.Font = new Font("Tahoma", 9, FontStyle.Regular);
            band.Objects.Add(textObj);
            return textObj;
        }
    }
}
