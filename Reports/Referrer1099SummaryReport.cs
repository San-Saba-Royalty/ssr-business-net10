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
    public class Referrer1099SummaryReport
    {
        public static Report Create(List<ReportReferrer1099Summary> data, string runDate)
        {
            var report = new Report();

            // Register Data
            report.RegisterData(data, "Referrer1099Summary");

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
            CreateLabel(pageHeader, "Referrer 1099 Summary", 0.13f, 0, 8.0f, 0.35f, 18, FontStyle.Regular, Color.FromArgb(28, 58, 112));
            CreateLabel(pageHeader, runDate, 11.0f, 0, 1.8f, 0.2f, 8, FontStyle.Regular, Color.Black).HorzAlign = HorzAlign.Right;

            // Column Headers
            float headerTop = 0.46f;
            Color headerBg = Color.FromArgb(28, 58, 112);
            Color headerFg = Color.White;

            CreateLabel(pageHeader, "Referrer", 0.10f, headerTop, 2.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Tax ID", 2.15f, headerTop, 1.20f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Seller", 3.40f, headerTop, 2.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Due Date", 5.45f, headerTop, 0.90f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Paid Date", 6.40f, headerTop, 0.90f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Bonus", 7.35f, headerTop, 1.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg).HorzAlign = HorzAlign.Right;
            CreateLabel(pageHeader, "Ref Fee", 8.40f, headerTop, 1.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg).HorzAlign = HorzAlign.Right;
            CreateLabel(pageHeader, "Total", 9.45f, headerTop, 1.00f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg).HorzAlign = HorzAlign.Right;
            CreateLabel(pageHeader, "LandMan", 10.50f, headerTop, 1.20f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);
            CreateLabel(pageHeader, "Status", 11.75f, headerTop, 1.25f, 0.33f, 9, FontStyle.Bold, headerFg, headerBg);

            // 2. Group Header (Referrer)
            var groupHeader = new GroupHeaderBand();
            groupHeader.Name = "GroupHeader1";
            groupHeader.Height = Units.Inches * 0.4f;
            groupHeader.Condition = "[Referrer1099Summary.ReferrerID]";
            page.Bands.Add(groupHeader);

            var groupText = CreateText(groupHeader, "[Referrer1099Summary.ReferrerName]", 0.10f, 0.1f, 4.0f, 0.25f);
            groupText.Font = new Font("Tahoma", 10, FontStyle.Bold);
            groupText.TextColor = Color.FromArgb(28, 58, 112);

            // 3. Data Band
            var dataBand = new DataBand();
            dataBand.Name = "Data1";
            dataBand.Height = Units.Inches * 0.3f;
            dataBand.DataSource = report.GetDataSource("Referrer1099Summary");
            groupHeader.Data = dataBand;

            // Data Fields
            float dataTop = 0.05f;
            CreateText(dataBand, "[Referrer1099Summary.ReferrerName]", 0.10f, dataTop, 2.00f, 0.2f);
            CreateText(dataBand, "[Referrer1099Summary.ReferrerTaxID]", 2.15f, dataTop, 1.20f, 0.2f);
            CreateText(dataBand, "[Referrer1099Summary.SellerName]", 3.40f, dataTop, 2.00f, 0.2f);
            CreateText(dataBand, "[Referrer1099Summary.DueDate]", 5.45f, dataTop, 0.90f, 0.2f).Format = new DateFormat();
            CreateText(dataBand, "[Referrer1099Summary.PaidDate]", 6.40f, dataTop, 0.90f, 0.2f).Format = new DateFormat();
            CreateText(dataBand, "[Referrer1099Summary.TotalBonus]", 7.35f, dataTop, 1.00f, 0.2f).HorzAlign = HorzAlign.Right;
            CreateText(dataBand, "[Referrer1099Summary.ReferralFee]", 8.40f, dataTop, 1.00f, 0.2f).HorzAlign = HorzAlign.Right;
            CreateText(dataBand, "[Referrer1099Summary.TotalBonusAndFee]", 9.45f, dataTop, 1.00f, 0.2f).HorzAlign = HorzAlign.Right;
            CreateText(dataBand, "[Referrer1099Summary.LandMan]", 10.50f, dataTop, 1.20f, 0.2f);
            CreateText(dataBand, "[Referrer1099Summary.DealStatus]", 11.75f, dataTop, 1.25f, 0.2f);

            // 4. Page Footer
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
