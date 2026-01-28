using System.IO;
using System.Threading.Tasks;

namespace SSRBusiness.Interfaces;

/// <summary>
/// Service for storing and retrieving generated documents from Azure File Share.
/// Generated documents are stored in the 'generated-documents' share with organized paths.
/// </summary>
public interface IGeneratedDocumentService
{
    /// <summary>
    /// Stores a generated document in Azure File Share under an organized path structure.
    /// </summary>
    /// <param name="entityType">Type of entity (e.g., "Acquisition", "LetterAgreement")</param>
    /// <param name="entityId">The entity ID</param>
    /// <param name="documentTypeName">Document type (e.g., "County", "Operator", "LetterAgreement")</param>
    /// <param name="documentContent">The document content stream</param>
    /// <param name="fileName">The original file name</param>
    /// <returns>Result containing the storage path and download URL</returns>
    Task<GeneratedDocumentResult> StoreGeneratedDocumentAsync(
        string entityType,
        int entityId,
        string documentTypeName,
        Stream documentContent,
        string fileName);

    /// <summary>
    /// Retrieves a generated document by its storage path.
    /// </summary>
    /// <param name="documentPath">Full path within the generated-documents share</param>
    /// <returns>The document stream, or null if not found</returns>
    Task<Stream?> RetrieveDocumentAsync(string documentPath);

    /// <summary>
    /// Gets the download URL for a generated document.
    /// </summary>
    /// <param name="documentPath">Full path within the generated-documents share</param>
    /// <returns>URL that can be used to download the document</returns>
    string GetDocumentUrl(string documentPath);

    /// <summary>
    /// Asynchronously gets the download URL for a generated document.
    /// </summary>
    /// <param name="documentPath">Full path within the generated-documents share</param>
    /// <returns>URL that can be used to download the document</returns>
    Task<string?> GetDownloadUrlAsync(string documentPath);


    /// <summary>
    /// Lists all generated documents for a specific entity.
    /// </summary>
    /// <param name="entityType">Type of entity</param>
    /// <param name="entityId">The entity ID</param>
    /// <returns>List of document metadata</returns>
    Task<IEnumerable<GeneratedDocumentInfo>> ListDocumentsAsync(string entityType, int entityId);
}

/// <summary>
/// Result from storing a generated document.
/// </summary>
public record GeneratedDocumentResult(
    string DocumentPath,
    string DownloadUrl,
    string FileName,
    DateTime GeneratedAt);

/// <summary>
/// Metadata about a generated document.
/// </summary>
public record GeneratedDocumentInfo(
    string DocumentPath,
    string FileName,
    string DocumentTypeName,
    long SizeBytes,
    DateTime GeneratedAt);
