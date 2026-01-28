using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.Interfaces;

namespace SSRBusiness.BusinessClasses;

public class CoverSheetService
{
    private readonly IFileService _fileService;
    private readonly IGeneratedDocumentService _generatedDocumentService;
    private readonly WordTemplateEngine _wordTemplateEngine;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CoverSheetService> _logger;
    private readonly IDbContextFactory<SsrDbContext> _dbContextFactory;
    private readonly string _templateShareName;

    public CoverSheetService(
        IFileService fileService,
        IGeneratedDocumentService generatedDocumentService,
        WordTemplateEngine wordTemplateEngine,
        IConfiguration configuration,
        ILogger<CoverSheetService> logger,
        IDbContextFactory<SsrDbContext> dbContextFactory)
    {
        _fileService = fileService;
        _generatedDocumentService = generatedDocumentService;
        _wordTemplateEngine = wordTemplateEngine;
        _configuration = configuration;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _templateShareName = configuration["AzureFileShare:FileShareName"] ?? "document-templates";
    }

    /// <summary>
    /// Attempts to download a template file, trying both .docx and .doc extensions.
    /// Returns the stream and the actual filename found.
    /// </summary>
    private async Task<(Stream stream, string actualFileName)> DownloadTemplateAsync(string baseFileName)
    {
        // Remove any existing extension to get the base name
        var baseName = Path.GetFileNameWithoutExtension(baseFileName);

        // Try .docx first
        var docxFileName = $"{baseName}.docx";
        try
        {
            var stream = await _fileService.DownloadFileAsync(_templateShareName, docxFileName);
            return (stream, docxFileName);
        }
        catch (FileNotFoundException)
        {
            // .docx not found, try .doc
            var docFileName = $"{baseName}.doc";
            try
            {
                var stream = await _fileService.DownloadFileAsync(_templateShareName, docFileName);
                return (stream, docFileName);
            }
            catch (FileNotFoundException)
            {
                // Neither found
                throw new InvalidOperationException(
                    $"Template file not found: tried both '{docxFileName}' and '{docFileName}' in file share '{_templateShareName}'.");
            }
        }
    }

    /// <summary>
    /// Generates an acquisition document from a template and stores it in the generated-documents share.
    /// </summary>
    /// <param name="acquisition">The acquisition entity with pre-loaded relationships</param>
    /// <param name="templatePath">Path to the template in the templates file share</param>
    /// <param name="documentTypeName">Type of document being generated (e.g., "County", "Operator")</param>
    /// <param name="context">Optional context with entity-specific IDs based on document type</param>
    /// <param name="user">Optional user for merge data</param>
    /// <returns>Result containing the document path in Azure File Share</returns>
    public async Task<GeneratedDocumentResult> GenerateAcquisitionDocumentAsync(
        Acquisition acquisition,
        string templatePath,
        string documentTypeName,
        DocumentGenerationContext? context = null,
        User? user = null)
    {
        if (acquisition == null)
            throw new ArgumentNullException(nameof(acquisition));
        if (string.IsNullOrWhiteSpace(templatePath))
            throw new ArgumentException("Template path is required", nameof(templatePath));

        try
        {
            _logger.LogInformation("Generating {DocumentType} document for Acquisition {AcquisitionID} from template {TemplatePath}",
                documentTypeName, acquisition.AcquisitionID, templatePath);

            // Download template from File Share
            Stream templateStream;
            try
            {
                templateStream = await _fileService.DownloadFileAsync(_templateShareName, templatePath);
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Template file not found: {TemplatePath} in share {ShareName}",
                    templatePath, _templateShareName);
                throw new InvalidOperationException(
                    $"Template '{templatePath}' not found in file share '{_templateShareName}'.");
            }

            // Write template to temp file for WordTemplateEngine
            var tempTemplatePath = Path.Combine(Path.GetTempPath(), $"template_{Guid.NewGuid()}{Path.GetExtension(templatePath)}");
            await using (var fileStream = File.Create(tempTemplatePath))
            {
                await templateStream.CopyToAsync(fileStream);
            }
            await templateStream.DisposeAsync();

            // Create temp output file
            var outputFileName = $"Acquisition_{acquisition.AcquisitionID}_{documentTypeName}_{DateTime.Now:yyyyMMddHHmmss}.docx";
            var tempOutputPath = Path.Combine(Path.GetTempPath(), outputFileName);

            // Generate the merged document with context
            _wordTemplateEngine.CreateMergeDocument(tempTemplatePath, tempOutputPath, acquisition, user, context);

            // Store to generated-documents file share
            await using var outputStream = File.OpenRead(tempOutputPath);
            var result = await _generatedDocumentService.StoreGeneratedDocumentAsync(
                "Acquisition",
                acquisition.AcquisitionID,
                documentTypeName,
                outputStream,
                outputFileName);

            _logger.LogInformation("Document generated and stored at {DocumentPath}", result.DocumentPath);

            // Cleanup temp files
            CleanupTempFiles(tempTemplatePath, tempOutputPath);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating document for Acquisition {AcquisitionID}", acquisition.AcquisitionID);
            throw;
        }
    }

    /// <summary>
    /// Generates a cover sheet with user context
    /// </summary>
    /// <param name="acquisition">Acquisition entity</param>
    /// <param name="documentType">Document type for template</param>
    /// <param name="description">Document description</param>
    /// <param name="username">Username for file naming</param>
    /// <param name="currentUser">Current logged-in user for template merging (optional)</param>
    /// <returns>Document path</returns>
    public async Task<string> GenerateAcquisitionCoverSheetAsync(
        Acquisition acquisition,
        string documentType,
        string description,
        string username,
        User? currentUser = null)
    {
        // Download template from File Share (try both .docx and .doc)
        var (templateStream, actualTemplateFileName) = await DownloadTemplateAsync("Acquisition Cover Sheet Template");

        // Write template to temp file
        var templateExtension = Path.GetExtension(actualTemplateFileName);
        var tempTemplatePath = Path.Combine(Path.GetTempPath(), $"acquisition_template_{Guid.NewGuid()}{templateExtension}");
        await using (var fileStream = File.Create(tempTemplatePath))
        {
            await templateStream.CopyToAsync(fileStream);
        }
        await templateStream.DisposeAsync();

        // Convert .doc to .docx if necessary
        var (docxTemplatePath, needsTemplateCleanup) = _wordTemplateEngine.ConvertToDocxIfNeeded(tempTemplatePath);

        var result = await GenerateAcquisitionDocumentAsync(acquisition, docxTemplatePath, documentType, context: null, user: currentUser);

        // Clean up temp template files
        try
        {
            if (docxTemplatePath != tempTemplatePath)
            {
                File.Delete(tempTemplatePath); // Delete original .doc temp file
            }
            if (needsTemplateCleanup)
            {
                File.Delete(docxTemplatePath); // Delete converted .docx temp file
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete temporary template files");
        }
        return result.DocumentPath;
    }

    /// <summary>
    /// Generates a cover sheet (backward compatibility)
    /// </summary>
    /// <param name="acquisition">Acquisition entity</param>
    /// <param name="documentType">Document type for template</param>
    /// <param name="description">Document description</param>
    /// <param name="username">Username for file naming</param>
    /// <returns>Document path</returns>
    public async Task<string> GenerateAcquisitionCoverSheetAsync(
        Acquisition acquisition,
        string documentType,
        string description,
        string username)
    {
        return await GenerateAcquisitionCoverSheetAsync(acquisition, documentType, description, username, currentUser: null);
    }

    /// <summary>
    /// Generates barcode cover sheets for multiple acquisitions.
    /// Creates a single merged document with all barcode cover sheets.
    /// </summary>
    /// <param name="barcodeRequests">List of barcode generation requests</param>
    /// <param name="username">Username for file naming</param>
    /// <param name="currentUserId">Current logged-in user ID for template merging (optional)</param>
    /// <returns>Result containing the merged document path</returns>
    public async Task<GeneratedDocumentResult> GenerateAcquisitionBarcodeCoverSheetsAsync(
        List<BarcodeDocumentRequest> barcodeRequests,
        string username,
        int? currentUserId = null)
    {
        User? currentUser = null;
        if (currentUserId.HasValue)
        {
            using var userContext = _dbContextFactory.CreateDbContext();
            var userRepository = new UserRepository(userContext);
            currentUser = await userRepository.LoadUserByUserIdAsync(currentUserId.Value);
            if (currentUser == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", currentUserId.Value);
            }
        }

        return await GenerateAcquisitionBarcodeCoverSheetsAsync(barcodeRequests, username, currentUser);
    }

    /// <summary>
    /// Generates barcode cover sheets for multiple acquisitions.
    /// Creates a single merged document with all barcode cover sheets.
    /// </summary>
    /// <param name="barcodeRequests">List of barcode generation requests</param>
    /// <param name="username">Username for file naming</param>
    /// <param name="currentUser">Current logged-in user for template merging (optional)</param>
    /// <returns>Result containing the merged document path</returns>
    public async Task<GeneratedDocumentResult> GenerateAcquisitionBarcodeCoverSheetsAsync(
        List<BarcodeDocumentRequest> barcodeRequests,
        string username,
        User? currentUser)
    {
        if (barcodeRequests == null || !barcodeRequests.Any())
            throw new ArgumentException("At least one barcode request is required", nameof(barcodeRequests));

        try
        {
            _logger.LogInformation("Generating {Count} barcode cover sheets for user {Username}",
                barcodeRequests.Count, username);

            // Use the provided current user for template processing
            var user = currentUser;
            if (user != null)
            {
                _logger.LogInformation("Using current user for template processing: {UserName} ({UserId})",
                    user.UserName ?? user.Email, user.UserId);
            }
            else
            {
                _logger.LogInformation("No current user provided - user placeholders will remain empty");
            }

            // Download template from File Share (try both .docx and .doc)
            var (templateStream, actualTemplateFileName) = await DownloadTemplateAsync("Acquisition Cover Sheet Template");

            _logger.LogInformation("Using template file: {TemplateFileName}", actualTemplateFileName);

            // Write template to temp file (preserve original extension)
            var templateExtension = Path.GetExtension(actualTemplateFileName);
            var tempTemplatePath = Path.Combine(Path.GetTempPath(), $"barcode_template_{Guid.NewGuid()}{templateExtension}");
            await using (var fileStream = File.Create(tempTemplatePath))
            {
                await templateStream.CopyToAsync(fileStream);
            }
            await templateStream.DisposeAsync();

            // Convert .doc to .docx if necessary
            var (docxTemplatePath, needsTemplateCleanup) = _wordTemplateEngine.ConvertToDocxIfNeeded(tempTemplatePath);

            // If conversion occurred, clean up the original temp file
            if (needsTemplateCleanup && docxTemplatePath != tempTemplatePath)
            {
                try
                {
                    File.Delete(tempTemplatePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete temporary template file: {TempPath}", tempTemplatePath);
                }
            }

            // Create temp output file for merged document
            var outputFileName = $"AcquisitionBarcode-{username}-{DateTime.Now:yyyyMMddHHmmss}.docx";
            var tempOutputPath = Path.Combine(Path.GetTempPath(), outputFileName);

            // Generate merged barcode document
            await GenerateMergedBarcodeDocumentAsync(docxTemplatePath, tempOutputPath, barcodeRequests, user);

            // Store to generated-documents file share
            await using var outputStream = File.OpenRead(tempOutputPath);
            var result = await _generatedDocumentService.StoreGeneratedDocumentAsync(
                "Acquisition",
                barcodeRequests.First().AcquisitionID, // Use first acquisition ID for organization
                "BarcodeSheet",
                outputStream,
                outputFileName);

            _logger.LogInformation("Barcode document generated and stored at {DocumentPath}", result.DocumentPath);

            // Cleanup temp files
            CleanupTempFiles(docxTemplatePath, tempOutputPath);

            // Clean up converted template file if it was created during conversion
            if (needsTemplateCleanup)
            {
                try
                {
                    File.Delete(docxTemplatePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete converted template file: {TempPath}", docxTemplatePath);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating barcode cover sheets");
            throw;
        }
    }

    /// <summary>
    /// Generates a merged barcode document from multiple requests.
    /// Each request can specify multiple copies.
    /// </summary>
    private async Task GenerateMergedBarcodeDocumentAsync(
        string templatePath,
        string outputPath,
        List<BarcodeDocumentRequest> requests,
        User? user)
    {
        using var outputDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Create(
            outputPath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);

        var mainPart = outputDoc.AddMainDocumentPart();
        mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
            new DocumentFormat.OpenXml.Wordprocessing.Body());

        bool isFirstPage = true;

        foreach (var request in requests)
        {
            // Load acquisition data with all related entities (similar to AcquisitionRepository pattern)
            using var context = _dbContextFactory.CreateDbContext();
            var acquisition = await context.Acquisitions
                .Include(a => a.AcquisitionStatus)
                .Include(a => a.AcquisitionBuyers).ThenInclude(ab => ab.Buyer).ThenInclude(b => b.BuyerContacts)
                .Include(a => a.AcquisitionSellers)
                .Include(a => a.AcquisitionCounties).ThenInclude(ac => ac.County).ThenInclude(c => c.CountyContacts)
                .Include(a => a.AcquisitionOperators).ThenInclude(ao => ao.Operator).ThenInclude(o => o.OperatorContacts)
                .Include(a => a.AcquisitionUnits).ThenInclude(au => au.UnitType)
                .Include(a => a.AcquisitionUnits).ThenInclude(au => au.AcqUnitCounties)
                .Include(a => a.AcquisitionUnits).ThenInclude(au => au.AcqUnitWells)
                .Include(a => a.AcquisitionNotes)
                .Include(a => a.AcquisitionLiens)
                .Include(a => a.AcqCurativeRequirements)
                .Include(a => a.AcquisitionReferrers)
                .FirstOrDefaultAsync(a => a.AcquisitionID == request.AcquisitionID);

            if (acquisition == null)
            {
                _logger?.LogWarning("Acquisition with ID {AcquisitionId} not found", request.AcquisitionID);
                continue;
            }

            for (int copy = 0; copy < request.NumberCopies; copy++)
            {
                // Add page break before each page except the first
                if (!isFirstPage)
                {
                    mainPart.Document.Body!.AppendChild(
                        new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Break { Type = DocumentFormat.OpenXml.Wordprocessing.BreakValues.Page })));
                }
                isFirstPage = false;

                // Copy template content and merge placeholders
                await AppendBarcodePageAsync(mainPart, templatePath, acquisition, request, user);
            }
        }

        mainPart.Document.Save();
    }

    /// <summary>
    /// Generates barcode cover sheets for multiple acquisitions (backward compatibility).
    /// Creates a single merged document with all barcode cover sheets.
    /// </summary>
    /// <param name="barcodeRequests">List of barcode generation requests</param>
    /// <param name="username">Username for file naming</param>
    /// <returns>Result containing the merged document path</returns>
    public async Task<GeneratedDocumentResult> GenerateAcquisitionBarcodeCoverSheetsAsync(
        List<BarcodeDocumentRequest> barcodeRequests,
        string username)
    {
        return await GenerateAcquisitionBarcodeCoverSheetsAsync(barcodeRequests, username, currentUser: null);
    }

    /// <summary>
    /// Appends a single barcode page to the document.
    /// </summary>
    private async Task AppendBarcodePageAsync(
        DocumentFormat.OpenXml.Packaging.MainDocumentPart targetDoc,
        string templatePath,
        Acquisition acquisition,
        BarcodeDocumentRequest request,
        User? user)
    {
        // Create a temporary merged document using proper WordTemplateEngine
        var tempMergedPath = Path.Combine(Path.GetTempPath(), $"barcode_page_{Guid.NewGuid()}.docx");

        try
        {
            // Create document generation context for barcode-specific placeholders
            var context = new DocumentGenerationContext
            {
                DocumentTypeCode = request.DocumentTypeCode,
                DocumentDescription = request.DocumentDescription
            };

            // Use WordTemplateEngine to properly merge all acquisition data and context
            _wordTemplateEngine.CreateMergeDocument(templatePath, tempMergedPath, acquisition, user, context);

            // Copy merged content to target document
            using (var sourceDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(tempMergedPath, false))
            {
                var sourceBody = sourceDoc.MainDocumentPart?.Document.Body;
                if (sourceBody != null)
                {
                    foreach (var element in sourceBody.Elements())
                    {
                        targetDoc.Document.Body!.AppendChild(element.CloneNode(true));
                    }
                }
            }
        }
        finally
        {
            CleanupTempFiles(tempMergedPath);
        }
    }


    private void CleanupTempFiles(params string[] paths)
    {
        foreach (var path in paths)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cleanup temporary file: {Path}", path);
            }
        }
    }
}
