using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Composes multiple Word documents into a single document.
/// Supports section breaks and preserves formatting from source documents.
/// </summary>
public class DocumentComposer
{
    private readonly ILogger<DocumentComposer>? _logger;

    public DocumentComposer(ILogger<DocumentComposer>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Merges multiple documents into the destination document using AltChunk.
    /// Each document is inserted at the end with an optional section break.
    /// </summary>
    /// <param name="destinationStream">Stream containing the main document</param>
    /// <param name="sourcePaths">Paths to source documents to append</param>
    /// <param name="addSectionBreaks">Whether to insert section breaks between documents</param>
    public void MergeDocuments(Stream destinationStream, IEnumerable<string> sourcePaths, bool addSectionBreaks = true)
    {
        using var wordDoc = WordprocessingDocument.Open(destinationStream, true);
        var mainPart = wordDoc.MainDocumentPart;
        if (mainPart?.Document.Body == null) return;

        foreach (var path in sourcePaths)
        {
            if (!File.Exists(path))
            {
                _logger?.LogWarning("Source document not found: {Path}", path);
                continue;
            }

            AppendDocument(wordDoc, path, addSectionBreaks);
        }

        mainPart.Document.Save();
    }

    /// <summary>
    /// Merges documents from streams into the destination document.
    /// </summary>
    public void MergeDocuments(Stream destinationStream, IEnumerable<(Stream Source, string Name)> sources, bool addSectionBreaks = true)
    {
        using var wordDoc = WordprocessingDocument.Open(destinationStream, true);
        var mainPart = wordDoc.MainDocumentPart;
        if (mainPart?.Document.Body == null) return;

        foreach (var (source, name) in sources)
        {
            AppendDocument(wordDoc, source, name, addSectionBreaks);
        }

        mainPart.Document.Save();
    }

    /// <summary>
    /// Appends a document from file path using AltChunk.
    /// </summary>
    private void AppendDocument(WordprocessingDocument wordDoc, string sourcePath, bool addSectionBreak)
    {
        var mainPart = wordDoc.MainDocumentPart!;
        var body = mainPart.Document.Body!;

        // Add section break before inserting
        if (addSectionBreak)
        {
            InsertSectionBreak(body);
        }

        // Create unique ID for this chunk
        var chunkId = "chunk_" + Guid.NewGuid().ToString("N")[..8];

        // Add the document as an alternative format import part
        var chunk = mainPart.AddAlternativeFormatImportPart(
            AlternativeFormatImportPartType.WordprocessingML, chunkId);

        using var fs = File.OpenRead(sourcePath);
        chunk.FeedData(fs);

        // Add AltChunk reference to body
        var altChunk = new AltChunk { Id = chunkId };
        body.AppendChild(altChunk);

        _logger?.LogDebug("Appended document: {Path}", sourcePath);
    }

    /// <summary>
    /// Appends a document from stream using AltChunk.
    /// </summary>
    private void AppendDocument(WordprocessingDocument wordDoc, Stream source, string name, bool addSectionBreak)
    {
        var mainPart = wordDoc.MainDocumentPart!;
        var body = mainPart.Document.Body!;

        if (addSectionBreak)
        {
            InsertSectionBreak(body);
        }

        var chunkId = "chunk_" + Guid.NewGuid().ToString("N")[..8];

        var chunk = mainPart.AddAlternativeFormatImportPart(
            AlternativeFormatImportPartType.WordprocessingML, chunkId);

        chunk.FeedData(source);

        var altChunk = new AltChunk { Id = chunkId };
        body.AppendChild(altChunk);

        _logger?.LogDebug("Appended document from stream: {Name}", name);
    }

    /// <summary>
    /// Inserts a section break (next page) before the next content.
    /// </summary>
    private void InsertSectionBreak(Body body)
    {
        var lastPara = body.Elements<Paragraph>().LastOrDefault();

        if (lastPara == null)
        {
            lastPara = new Paragraph();
            body.AppendChild(lastPara);
        }

        var paraProps = lastPara.GetFirstChild<ParagraphProperties>();
        if (paraProps == null)
        {
            paraProps = new ParagraphProperties();
            lastPara.PrependChild(paraProps);
        }

        var sectProps = new SectionProperties(
            new SectionType { Val = SectionMarkValues.NextPage }
        );

        paraProps.AppendChild(sectProps);
    }

    /// <summary>
    /// Clones elements from a source document into the destination body.
    /// This is an alternative to AltChunk that provides more control.
    /// </summary>
    public void CloneDocumentElements(Stream destinationStream, Stream sourceStream, bool addSectionBreak = true)
    {
        using var destDoc = WordprocessingDocument.Open(destinationStream, true);
        using var srcDoc = WordprocessingDocument.Open(sourceStream, false);

        var destBody = destDoc.MainDocumentPart?.Document.Body;
        var srcBody = srcDoc.MainDocumentPart?.Document.Body;

        if (destBody == null || srcBody == null) return;

        if (addSectionBreak)
        {
            InsertSectionBreak(destBody);
        }

        // Clone each element from source to destination
        foreach (var element in srcBody.ChildElements)
        {
            // Skip section properties at the end
            if (element is SectionProperties) continue;

            var clone = element.CloneNode(true);
            destBody.AppendChild(clone);
        }

        destDoc.MainDocumentPart?.Document.Save();
    }

    /// <summary>
    /// Replaces a placeholder with the content of another document.
    /// Useful for inserting dynamic sections at specific locations.
    /// </summary>
    public void InsertDocumentAtPlaceholder(Stream destinationStream, string placeholder, Stream sourceStream)
    {
        using var destDoc = WordprocessingDocument.Open(destinationStream, true);
        using var srcDoc = WordprocessingDocument.Open(sourceStream, false);

        var destBody = destDoc.MainDocumentPart?.Document.Body;
        var srcBody = srcDoc.MainDocumentPart?.Document.Body;

        if (destBody == null || srcBody == null) return;

        // Find paragraph containing placeholder
        var placeholderPara = destBody.Descendants<Paragraph>()
            .FirstOrDefault(p => p.InnerText.Contains(placeholder));

        if (placeholderPara == null)
        {
            _logger?.LogWarning("Placeholder '{Placeholder}' not found", placeholder);
            return;
        }

        // Clone source elements and insert after placeholder paragraph
        OpenXmlElement insertPoint = placeholderPara;
        foreach (var element in srcBody.ChildElements)
        {
            if (element is SectionProperties) continue;

            var clone = element.CloneNode(true);
            insertPoint.InsertAfterSelf(clone);
            insertPoint = clone;
        }

        // Remove the placeholder paragraph
        placeholderPara.Remove();

        destDoc.MainDocumentPart?.Document.Save();
    }
}
