using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Grc.Assessments;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;

namespace Grc.Reporting.Application;

/// <summary>
/// Report generator for PDF and Excel reports
/// </summary>
public class ReportGenerator
{
    private readonly ILogger<ReportGenerator> _logger;

    public ReportGenerator(ILogger<ReportGenerator> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Generate assessment report in PDF format
    /// </summary>
    public async Task<byte[]> GenerateAssessmentReportPdfAsync(
        AssessmentDto assessment,
        List<ControlAssessmentDto> controlAssessments,
        string language = "en")
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text($"Assessment Report: {assessment.Name}")
                        .SemiBold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().Text($"Assessment: {assessment.Name}");
                            column.Item().Text($"Type: {assessment.Type}");
                            column.Item().Text($"Status: {assessment.Status}");
                            column.Item().Text($"Completion: {assessment.CompletionPercentage}%");
                            column.Item().Text($"Overall Score: {assessment.OverallScore}");

                            column.Item().PaddingTop(10).Text("Control Assessments:").SemiBold();
                            
                            foreach (var control in controlAssessments)
                            {
                                column.Item().Text($"- {control.ControlNumber}: {control.Status} (Score: {control.VerifiedScore ?? control.SelfScore})");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        });
    }

    /// <summary>
    /// Generate assessment report in Excel format
    /// </summary>
    public async Task<byte[]> GenerateAssessmentReportExcelAsync(
        AssessmentDto assessment,
        List<ControlAssessmentDto> controlAssessments)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Assessment Report");

            // Header
            worksheet.Cell(1, 1).Value = "Assessment Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;

            // Assessment details
            worksheet.Cell(3, 1).Value = "Assessment Name:";
            worksheet.Cell(3, 2).Value = assessment.Name;
            worksheet.Cell(4, 1).Value = "Type:";
            worksheet.Cell(4, 2).Value = assessment.Type.ToString();
            worksheet.Cell(5, 1).Value = "Status:";
            worksheet.Cell(5, 2).Value = assessment.Status.ToString();
            worksheet.Cell(6, 1).Value = "Completion:";
            worksheet.Cell(6, 2).Value = $"{assessment.CompletionPercentage}%";
            worksheet.Cell(7, 1).Value = "Overall Score:";
            worksheet.Cell(7, 2).Value = assessment.OverallScore;

            // Control assessments table
            worksheet.Cell(9, 1).Value = "Control Number";
            worksheet.Cell(9, 2).Value = "Status";
            worksheet.Cell(9, 3).Value = "Self Score";
            worksheet.Cell(9, 4).Value = "Verified Score";
            worksheet.Cell(9, 5).Value = "Priority";

            var headerRange = worksheet.Range(9, 1, 9, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 10;
            foreach (var control in controlAssessments)
            {
                worksheet.Cell(row, 1).Value = control.ControlNumber ?? "N/A";
                worksheet.Cell(row, 2).Value = control.Status.ToString();
                worksheet.Cell(row, 3).Value = control.SelfScore;
                worksheet.Cell(row, 4).Value = control.VerifiedScore;
                worksheet.Cell(row, 5).Value = control.Priority.ToString();
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        });
    }

    /// <summary>
    /// Generate gap analysis report
    /// </summary>
    public async Task<byte[]> GenerateGapAnalysisReportAsync(
        AssessmentDto assessment,
        Dictionary<string, decimal> gapAnalysis,
        string format = "pdf")
    {
        if (format.ToLower() == "excel")
        {
            return await GenerateGapAnalysisExcelAsync(assessment, gapAnalysis);
        }
        else
        {
            return await GenerateGapAnalysisPdfAsync(assessment, gapAnalysis);
        }
    }

    private async Task<byte[]> GenerateGapAnalysisPdfAsync(AssessmentDto assessment, Dictionary<string, decimal> gapAnalysis)
    {
        // Similar to assessment report but focused on gaps
        return await Task.FromResult(new byte[0]); // Placeholder
    }

    private async Task<byte[]> GenerateGapAnalysisExcelAsync(AssessmentDto assessment, Dictionary<string, decimal> gapAnalysis)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Gap Analysis");

        worksheet.Cell(1, 1).Value = "Gap Analysis Report";
        worksheet.Cell(1, 1).Style.Font.Bold = true;

        worksheet.Cell(3, 1).Value = "Framework/Control";
        worksheet.Cell(3, 2).Value = "Gap Score";
        worksheet.Cell(3, 1).Style.Font.Bold = true;
        worksheet.Cell(3, 2).Style.Font.Bold = true;

        int row = 4;
        foreach (var gap in gapAnalysis)
        {
            worksheet.Cell(row, 1).Value = gap.Key;
            worksheet.Cell(row, 2).Value = gap.Value;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Generate executive summary report
    /// </summary>
    public async Task<byte[]> GenerateExecutiveSummaryAsync(
        AssessmentDto assessment,
        Dictionary<string, object> summaryData,
        string language = "en")
    {
        // High-level summary for executives
        return await GenerateAssessmentReportPdfAsync(assessment, new List<ControlAssessmentDto>(), language);
    }
}

