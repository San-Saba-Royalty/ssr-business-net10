using FastReport;
using FastReport.Export.PdfSimple;
using System.Collections;

namespace SSRBusiness.BusinessClasses;

public class ReportingService
{
    /// <summary>
    /// Generates a PDF report using FastReport.
    /// </summary>
    /// <param name="reportTemplatePath">Absolute path to .frx file</param>
    /// <param name="data">Data source (IEnumerable or DataSet)</param>
    /// <param name="dataName">Name of the data source in the report</param>
    /// <param name="outputPath">Output path for PDF</param>
    public void GeneratePdfReport(string reportTemplatePath, IEnumerable data, string dataName, string outputPath)
    {
        if (!File.Exists(reportTemplatePath))
            throw new FileNotFoundException("Report template not found", reportTemplatePath);

        using (Report report = new Report())
        {
            report.Load(reportTemplatePath);
            
            // Register Business Object data
            report.RegisterData(data, dataName);
            // Enable all tables if using DataSet, or the specific object
            report.GetDataSource(dataName).Enabled = true;

            if (report.Prepare())
            {
                using (PDFSimpleExport pdfExport = new PDFSimpleExport())
                {
                    report.Export(pdfExport, outputPath);
                }
            }
        }
    }
}
