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
    public class BuyerInvoicesDueReport
    {
        public static Report Create(List<ReportInvoicesDue> data, string runDate)
        {
            var report = new Report();

            // Register Data
            report.RegisterData(data, "BuyerInvoicesDue");

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

            // 1. Page Header
            var pageHeader = new PageHeaderBand();
            pageHeader.Name = "PageHeader1";
            pageHeader.Height = Units.Inches * 0.9f;
            page.PageHeader = pageHeader;

            // Header Labels
            CreateLabel(pageHeader, "Buyer Invoices Due", 0.13f, 0, 8.0f, 0.35f, 18, FontStyle.Regular, Color.FromArgb(28, 58, 112));
            CreateLabel(pageHeader, runDate, 11.0f, 0, 1.8f, 0.2f, 8, FontStyle.Regular, Color.Black).HorzAlign = HorzAlign.Right;

            // Column Headers
            float headerTop = 0.46f;
            Color headerBg = Color.FromArgb(28, 58, 112);
            Color headerFg = Color.White;

            CreateLabel(pageHeader, "Buyer", 0.10f, headerTop, 2.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Seller", 2.15f, headerTop, 2.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Acq #", 4.20f, headerTop, 0.80f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Inv #", 5.05f, headerTop, 0.80f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Due Date", 5.90f, headerTop, 0.90f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Inv Due", 6.85f, headerTop, 0.90f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Paid Date", 7.80f, headerTop, 0.90f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Amount", 8.75f, headerTop, 1.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg).HorzAlign = HorzAlign.Right;
            CreateLabel(pageHeader, "Inv Total", 9.80f, headerTop, 1.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg).HorzAlign = HorzAlign.Right;
            CreateLabel(pageHeader, "LandMan", 10.85f, headerTop, 1.20f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Status", 12.10f, headerTop, 1.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);

            // 2. Data Band
            var dataBand = new DataBand();
            dataBand.Name = "Data1";
            dataBand.Height = Units.Inches * 0.3f;
            dataBand.DataSource = report.GetDataSource("BuyerInvoicesDue");
            page.Bands.Add(dataBand);

            // Data Fields
            float dataTop = 0.05f;
            CreateText(dataBand, "[BuyerInvoicesDue.BuyerName]", 0.10f, dataTop, 2.00f, 0.2f);
            CreateText(dataBand, "[BuyerInvoicesDue.SellerName]", 2.15f, dataTop, 2.00f, 0.2f);
            CreateText(dataBand, "[BuyerInvoicesDue.BuyerAcquisitionNumber]", 4.20f, dataTop, 0.80f, 0.2f);
            CreateText(dataBand, "[BuyerInvoicesDue.InvoiceNumber]", 5.05f, dataTop, 0.80f, 0.2f);
            CreateText(dataBand, "[BuyerInvoicesDue.DueDate]", 5.90f, dataTop, 0.90f, 0.2f).Format = new DateFormat();
            CreateText(dataBand, "[BuyerInvoicesDue.InvoiceDueDate]", 6.85f, dataTop, 0.90f, 0.2f).Format = new DateFormat();
            CreateText(dataBand, "[BuyerInvoicesDue.PaidDate]", 7.80f, dataTop, 0.90f, 0.2f).Format = new DateFormat();
            CreateText(dataBand, "[BuyerInvoicesDue.TotalBonus]", 8.75f, dataTop, 1.00f, 0.2f).HorzAlign = HorzAlign.Right;
            CreateText(dataBand, "[BuyerInvoicesDue.InvoiceTotal]", 9.80f, dataTop, 1.00f, 0.2f).HorzAlign = HorzAlign.Right;
            CreateText(dataBand, "[BuyerInvoicesDue.LandMan]", 10.85f, dataTop, 1.20f, 0.2f);
            CreateText(dataBand, "[BuyerInvoicesDue.DealStatus]", 12.10f, dataTop, 1.00f, 0.2f);

            // 3. Page Footer
            var pageFooter = new PageFooterBand();
            pageFooter.Name = "PageFooter1";
            pageFooter.Height = Units.Inches * 0.3f;
            page.PageFooter = pageFooter;

            CreateText(pageFooter, "Page [PageN]", 12.46f, 0.07f, 1.0f, 0.2f).HorzAlign = HorzAlign.Right;

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
