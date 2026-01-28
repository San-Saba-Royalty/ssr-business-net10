using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.Interfaces;
using DocSharp.Binary.DocFileFormat;
using DocSharp.Binary.OpenXmlLib.WordprocessingML;
using DocSharp.Binary.WordprocessingMLMapping;
using DocSharp.Binary.StructuredStorage.Reader;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Engine for generating Letter Agreement documents with multi-section composition.
/// Handles Letter Agreement, Conveyance, Exhibit A, and Signature block generation.
/// </summary>
public class LetterAgreementTemplateEngine
{
    private readonly IDbContextFactory<SsrDbContext> _contextFactory;
    private readonly IFileService _fileService;
    private readonly IGeneratedDocumentService _generatedDocumentService;
    private readonly ILogger<LetterAgreementTemplateEngine> _logger;

    public LetterAgreementTemplateEngine(
        IDbContextFactory<SsrDbContext> contextFactory,
        IFileService fileService,
        IGeneratedDocumentService generatedDocumentService,
        ILogger<LetterAgreementTemplateEngine> logger)
    {
        _contextFactory = contextFactory;
        _fileService = fileService;
        _generatedDocumentService = generatedDocumentService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a complete Letter Agreement document with all sub-sections.
    /// </summary>
    public async Task<GeneratedDocumentResult> CreateLetterAgreementAsync(LetterAgreementCriteria criteria)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Load Letter Agreement with all related data
        var letterAgreement = await context.LetterAgreements
            .Include(la => la.LetterAgreementSellers)
            .Include(la => la.LetterAgreementUnits)
            .FirstOrDefaultAsync(la => la.LetterAgreementID == criteria.LetterAgreementId);

        if (letterAgreement == null)
        {
            throw new InvalidOperationException($"Letter Agreement {criteria.LetterAgreementId} not found.");
        }

        // Load signing partner details
        var signingPartner = letterAgreement.LetterAgreementSellers
            .FirstOrDefault(sp => sp.LetterAgreementSellerID == criteria.SigningPartnerId);

        if (signingPartner == null)
        {
            throw new InvalidOperationException($"Signing Partner {criteria.SigningPartnerId} not found.");
        }

        // Determine state for template selection (TX vs LA)
        string stateCode = DetermineStateCode(letterAgreement);
        criteria.StateCode = stateCode;

        // Load user
        var user = await context.Users.FindAsync(criteria.UserId);

        _logger.LogInformation("Creating Letter Agreement document for LA#{Id}, Partner#{PartnerId}, State={State}",
            criteria.LetterAgreementId, criteria.SigningPartnerId, stateCode);

        // Create the main document using composition
        using var outputStream = new MemoryStream();

        // Build the document sections
        await BuildLetterAgreementDocumentAsync(outputStream, criteria, letterAgreement, signingPartner, user);

        outputStream.Position = 0;

        // Store in Azure File Share
        var fileName = $"LetterAgreement_{letterAgreement.LetterAgreementID}_{signingPartner.LetterAgreementSellerID}.docx";
        var result = await _generatedDocumentService.StoreGeneratedDocumentAsync(
            "LetterAgreement",
            letterAgreement.LetterAgreementID,
            "LetterAgreement",
            outputStream,
            fileName);

        _logger.LogInformation("Generated Letter Agreement stored at: {Path}", result.DocumentPath);

        return result;
    }

    private async Task BuildLetterAgreementDocumentAsync(
        MemoryStream outputStream,
        LetterAgreementCriteria criteria,
        LetterAgreement letterAgreement,
        LetterAgreementSeller signingPartner,
        User? user)
    {
        // Download main Letter Agreement template
        var templateStream = await DownloadTemplateAsync(criteria.LetterAgreementSource);
        if (templateStream == null)
        {
            throw new FileNotFoundException($"Letter Agreement template not found: {criteria.LetterAgreementSource}");
        }

        // Convert to .docx if needed and create working copy
        var docxPath = await EnsureDocxFormatAsync(templateStream, criteria.LetterAgreementSource);

        try
        {
            // Copy to output stream
            using var docxStream = File.OpenRead(docxPath);
            await docxStream.CopyToAsync(outputStream);
            outputStream.Position = 0;

            // Open for editing
            using var wordDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(outputStream, true);
            var body = wordDoc.MainDocumentPart?.Document.Body;

            if (body == null)
            {
                throw new InvalidOperationException("Document body is null");
            }

            // Merge global data
            MergeGlobalData(body);

            // Merge user data
            if (user != null)
            {
                MergeUserData(user, body);
            }

            // Merge Letter Agreement data
            MergeLetterAgreementData(letterAgreement, signingPartner, body);

            // Merge custom fields
            foreach (var field in criteria.CustomFields)
            {
                SearchAndReplace(body, field.Key, field.Value);
            }

            // Insert signature block based on partner type
            await InsertSignatureBlockAsync(wordDoc, criteria, signingPartner);

            // Save changes
            wordDoc.MainDocumentPart?.Document.Save();
        }
        finally
        {
            // Cleanup temp file
            if (File.Exists(docxPath) && docxPath.Contains(Path.GetTempPath()))
            {
                try { File.Delete(docxPath); } catch { /* ignore cleanup errors */ }
            }
        }
    }

    private void MergeLetterAgreementData(LetterAgreement la, LetterAgreementSeller sp, Body body)
    {
        // Letter Agreement fields
        SearchAndReplace(body, "<LetterAgreementID>", la.LetterAgreementID.ToString());
        SearchAndReplace(body, "<LetterAgreementName>", la.LetterAgreementName ?? "");
        SearchAndReplace(body, "<EffectiveDate>", DisplayDate(la.EffectiveDate));
        SearchAndReplace(body, "<TotalBonus>", DisplayAmount(la.TotalBonus));
        SearchAndReplace(body, "<TotalOfferAmount>", DisplayAmount(la.TotalOfferAmount));
        SearchAndReplace(body, "<TotalBonusAcceptanceLetterAmount>", DisplayAmount(la.TotalBonusAcceptanceLetterAmount));
        SearchAndReplace(body, "<StateName>", la.StateName ?? "");
        SearchAndReplace(body, "<StateCode>", la.StateCode ?? "");
        SearchAndReplace(body, "<AcquisitionNumber>", la.AcquisitionNumber ?? "");
        SearchAndReplace(body, "<AssignmentNumber>", la.AssignmentNumber ?? "");
        SearchAndReplace(body, "<ReferrerName>", la.ReferrerName ?? "");
        SearchAndReplace(body, "<DealNotes>", DisplayText(la.DealNotes));
        SearchAndReplace(body, "<DealTabs>", DisplayText(la.DealTabs));

        // Signing Partner fields
        SearchAndReplace(body, "<SellingPartnerName>", sp.SellerName ?? "");
        SearchAndReplace(body, "<SellerName>", sp.SellerName ?? "");
        SearchAndReplace(body, "<SellerAddress>", ConstructAddress(sp.AddressLine1, sp.AddressLine2, null, sp.City, sp.StateCode, sp.ZipCode));
        SearchAndReplace(body, "<SellerCity>", sp.City ?? "");
        SearchAndReplace(body, "<SellerState>", sp.StateCode ?? "");
        SearchAndReplace(body, "<SellerZip>", sp.ZipCode ?? "");
        SearchAndReplace(body, "<IsCompany>", sp.CompanyIndicator ? "Company" : "Individual");

        // Partner type for conditional logic
        bool isCompany = sp.CompanyIndicator;
        SearchAndReplace(body, "<PartnerType>", isCompany ? "Company" : "Individual");

        // Number to word conversion for bonus
        if (la.TotalBonus.HasValue)
        {
            SearchAndReplace(body, "<TotalBonusWords>", NumberToWordConverter.ConvertDollars(la.TotalBonus.Value));
        }
    }

    private async Task InsertSignatureBlockAsync(
        DocumentFormat.OpenXml.Packaging.WordprocessingDocument wordDoc,
        LetterAgreementCriteria criteria,
        LetterAgreementSeller signingPartner)
    {
        // Determine which signature template to use
        string signaturePath = signingPartner.CompanyIndicator
            ? criteria.LetterAgreementSignatureCompanySource
            : criteria.LetterAgreementSignatureIndividualSource;

        if (string.IsNullOrEmpty(signaturePath))
        {
            _logger.LogWarning("No signature template path configured for {Type}",
                signingPartner.CompanyIndicator ? "Company" : "Individual");
            return;
        }

        var signatureStream = await DownloadTemplateAsync(signaturePath);
        if (signatureStream == null)
        {
            _logger.LogWarning("Signature template not found: {Path}", signaturePath);
            return;
        }

        // Convert signature template to docx if needed
        var sigDocxPath = await EnsureDocxFormatAsync(signatureStream, signaturePath);

        try
        {
            // Use AltChunk to insert signature document at end
            var mainPart = wordDoc.MainDocumentPart!;
            var altChunkId = "sig" + Guid.NewGuid().ToString("N")[..8];

            var chunk = mainPart.AddAlternativeFormatImportPart(
                AlternativeFormatImportPartType.WordprocessingML, altChunkId);

            using var sigStream = File.OpenRead(sigDocxPath);
            chunk.FeedData(sigStream);

            // Add AltChunk reference at the end of the document
            var altChunk = new AltChunk { Id = altChunkId };
            mainPart.Document.Body?.AppendChild(altChunk);
        }
        finally
        {
            if (File.Exists(sigDocxPath) && sigDocxPath.Contains(Path.GetTempPath()))
            {
                try { File.Delete(sigDocxPath); } catch { /* ignore */ }
            }
        }
    }

    private string DetermineStateCode(LetterAgreement la)
    {
        // Use state from Letter Agreement, default to TX
        if (!string.IsNullOrEmpty(la.StateCode))
        {
            return la.StateCode.ToUpperInvariant();
        }
        return "TX";
    }

    private async Task<Stream?> DownloadTemplateAsync(string templatePath)
    {
        try
        {
            // Assume templates are stored in Azure File Share under LetterAgreement folder
            return await _fileService.DownloadFileAsync("document-templates", templatePath);
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("Template file not found: {Path}", templatePath);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading template: {Path}", templatePath);
            return null;
        }
    }

    private async Task<string> EnsureDocxFormatAsync(Stream sourceStream, string originalPath)
    {
        var extension = Path.GetExtension(originalPath).ToLowerInvariant();
        var tempPath = Path.Combine(Path.GetTempPath(), $"LATemp_{Guid.NewGuid()}{extension}");

        // Save stream to temp file
        using (var fileStream = File.Create(tempPath))
        {
            await sourceStream.CopyToAsync(fileStream);
        }

        if (extension == ".docx")
        {
            return tempPath;
        }

        if (extension == ".doc")
        {
            // Convert .doc to .docx using DocSharp.Binary
            var docxPath = Path.ChangeExtension(tempPath, ".docx");

            using (var reader = new StructuredStorageReader(tempPath))
            {
                var doc = new WordDocument(reader);
                using var docxStream = File.Create(docxPath);
                var docxDoc = DocSharp.Binary.OpenXmlLib.WordprocessingML.WordprocessingDocument.Create(
                    docxStream, DocSharp.Binary.OpenXmlLib.WordprocessingDocumentType.Document);
                Converter.Convert(doc, docxDoc);
                docxDoc.Dispose();
            }

            // Cleanup original .doc temp file
            try { File.Delete(tempPath); } catch { /* ignore */ }

            return docxPath;
        }

        throw new NotSupportedException($"Unsupported file format: {extension}");
    }

    #region Helper Methods

    private void MergeGlobalData(Body body)
    {
        SearchAndReplace(body, "<Date>", DateTime.Now.ToString("MMMM dd, yyyy"));
        SearchAndReplace(body, "<Day>", DateTime.Now.ToString("dd"));
        SearchAndReplace(body, "<Year>", DateTime.Now.ToString("yyyy"));
        SearchAndReplace(body, "<Month>", DateTime.Now.ToString("MM"));
        SearchAndReplace(body, "<MonthName>", DateTime.Now.ToString("MMMM"));
        SearchAndReplace(body, "<DayX>", GetCurrentDaySuffix());
    }

    private void MergeUserData(User user, Body body)
    {
        SearchAndReplace(body, "<UserFirstName>", user.FirstName ?? "");
        SearchAndReplace(body, "<UserLastName>", user.LastName ?? "");
        SearchAndReplace(body, "<UserName>", $"{user.FirstName} {user.LastName}".Trim());
    }

    private string GetCurrentDaySuffix()
    {
        int day = DateTime.Now.Day;
        string dayStr = day.ToString();
        return (day % 10, day) switch
        {
            (1, not 11) => dayStr + "st",
            (2, not 12) => dayStr + "nd",
            (3, not 13) => dayStr + "rd",
            _ => dayStr + "th"
        };
    }

    private void SearchAndReplace(Body body, string placeholder, string replacement)
    {
        // First try direct text replacement in runs
        foreach (var text in body.Descendants<Text>())
        {
            if (text.Text.Contains(placeholder))
            {
                text.Text = text.Text.Replace(placeholder, replacement);
            }
        }

        // Handle split placeholders across runs using paragraph-level flattening
        foreach (var para in body.Descendants<Paragraph>())
        {
            var fullText = string.Concat(para.Descendants<Text>().Select(t => t.Text));
            if (fullText.Contains(placeholder))
            {
                // Flatten and replace
                FlattenParagraphAndReplace(para, placeholder, replacement);
            }
        }
    }

    private void FlattenParagraphAndReplace(Paragraph para, string placeholder, string replacement)
    {
        var texts = para.Descendants<Text>().ToList();
        if (texts.Count == 0) return;

        var fullText = string.Concat(texts.Select(t => t.Text));
        if (!fullText.Contains(placeholder)) return;

        // Replace in the concatenated text
        var newText = fullText.Replace(placeholder, replacement);

        // Put all text in the first Text element, clear the rest
        texts[0].Text = newText;
        for (int i = 1; i < texts.Count; i++)
        {
            texts[i].Text = "";
        }
    }

    private string ConstructAddress(string? line1, string? line2, string? line3, string? city, string? state, string? zip)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(line1)) parts.Add(line1);
        if (!string.IsNullOrWhiteSpace(line2)) parts.Add(line2);
        if (!string.IsNullOrWhiteSpace(line3)) parts.Add(line3);

        var cityStateZip = new List<string>();
        if (!string.IsNullOrWhiteSpace(city)) cityStateZip.Add(city);
        if (!string.IsNullOrWhiteSpace(state)) cityStateZip.Add(state);
        if (cityStateZip.Count > 0 && !string.IsNullOrWhiteSpace(zip))
        {
            cityStateZip[^1] = cityStateZip[^1] + " " + zip;
        }
        if (cityStateZip.Count > 0)
        {
            parts.Add(string.Join(", ", cityStateZip));
        }

        return string.Join("\n", parts);
    }

    private string DisplayDate(DateTime? dt) => dt?.ToString("MM/dd/yyyy") ?? "";
    private string DisplayAmount(decimal? amt) => amt?.ToString("C") ?? "";
    private string DisplayDecimal(decimal? dec) => dec?.ToString() ?? "";
    private string DisplayText(string? text) => text ?? "";

    #endregion
}
