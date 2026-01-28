using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSRBusiness.Interfaces;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Service for storing and retrieving generated documents from Azure File Share.
/// Uses the 'generated-documents' share with organized folder structure:
/// /{EntityType}/{EntityId}/{DocumentType}/{timestamp}_{filename}
/// </summary>
public class GeneratedDocumentService : IGeneratedDocumentService
{
    private readonly ShareServiceClient _shareServiceClient;
    private readonly ILogger<GeneratedDocumentService> _logger;
    private readonly string _fileShareName;
    private readonly string _baseUrl;

    public GeneratedDocumentService(IConfiguration configuration, ILogger<GeneratedDocumentService> logger)
    {
        _logger = logger;

        var azureFileShareConfig = configuration.GetSection("AzureFileShare");
        var accountName = azureFileShareConfig["AccountName"];
        var accountKey = azureFileShareConfig["AccountKey"];
        _fileShareName = azureFileShareConfig["GeneratedDocumentsShare"] ?? "generated-documents";

        if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountKey))
        {
            _logger.LogWarning("AzureFileShare configuration is missing AccountName or AccountKey. File operations will fail.");
            _shareServiceClient = new ShareServiceClient("UseDevelopmentStorage=true");
            _baseUrl = "http://localhost";
        }
        else
        {
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";
            _shareServiceClient = new ShareServiceClient(connectionString);
            _baseUrl = $"https://{accountName}.file.core.windows.net/{_fileShareName}";
        }
    }

    public async Task<GeneratedDocumentResult> StoreGeneratedDocumentAsync(
        string entityType,
        int entityId,
        string documentTypeName,
        Stream documentContent,
        string fileName)
    {
        var shareClient = _shareServiceClient.GetShareClient(_fileShareName);
        await shareClient.CreateIfNotExistsAsync();

        // Create organized folder structure: /{EntityType}/{EntityId}/{DocumentType}/
        var folderPath = $"{entityType}/{entityId}/{documentTypeName}";
        var directoryClient = await EnsureDirectoryExistsAsync(shareClient, folderPath);

        // Generate unique filename with timestamp
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var uniqueFileName = $"{timestamp}_{SanitizeFileName(fileName)}";
        var fileClient = directoryClient.GetFileClient(uniqueFileName);

        // Prepare content for upload
        var (uploadStream, length, shouldDispose) = await PrepareUploadAsync(documentContent);
        try
        {
            await fileClient.CreateAsync(length);
            await fileClient.UploadRangeAsync(new HttpRange(0, length), uploadStream);
            await fileClient.SetHttpHeadersAsync(new ShareFileSetHttpHeadersOptions
            {
                HttpHeaders = new ShareFileHttpHeaders { ContentType = GetMimeType(fileName) }
            });
            await fileClient.SetMetadataAsync(new Dictionary<string, string>
            {
                { "OriginalFileName", fileName },
                { "EntityType", entityType },
                { "EntityId", entityId.ToString() },
                { "DocumentTypeName", documentTypeName },
                { "GeneratedAt", DateTime.UtcNow.ToString("O") }
            });
        }
        finally
        {
            if (shouldDispose) uploadStream.Dispose();
        }

        var documentPath = $"{folderPath}/{uniqueFileName}";
        _logger.LogInformation("Stored generated document: {DocumentPath}", documentPath);

        return new GeneratedDocumentResult(
            DocumentPath: documentPath,
            DownloadUrl: GetDocumentUrl(documentPath),
            FileName: fileName,
            GeneratedAt: DateTime.UtcNow);
    }

    public async Task<Stream?> RetrieveDocumentAsync(string documentPath)
    {
        try
        {
            var shareClient = _shareServiceClient.GetShareClient(_fileShareName);

            // Parse path into directory and file components
            var lastSlash = documentPath.LastIndexOf('/');
            var directoryPath = lastSlash > 0 ? documentPath[..lastSlash] : "";
            var fileName = lastSlash >= 0 ? documentPath[(lastSlash + 1)..] : documentPath;

            ShareDirectoryClient directoryClient;
            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryClient = shareClient.GetRootDirectoryClient();
            }
            else
            {
                directoryClient = shareClient.GetDirectoryClient(directoryPath);
            }

            var fileClient = directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
            {
                _logger.LogWarning("Generated document not found: {DocumentPath}", documentPath);
                return null;
            }

            var download = await fileClient.DownloadAsync();
            var memoryStream = new MemoryStream();
            await download.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving generated document: {DocumentPath}", documentPath);
            return null;
        }
    }

    public string GetDocumentUrl(string documentPath)
    {
        // Return an API endpoint URL that will retrieve the document
        // The actual Azure File Share URL requires SAS tokens, so we use our API endpoint
        return $"/api/generated-document/{Uri.EscapeDataString(documentPath)}";
    }

    public Task<string?> GetDownloadUrlAsync(string documentPath)
    {
        // Async wrapper around GetDocumentUrl for async workflows
        return Task.FromResult<string?>(GetDocumentUrl(documentPath));
    }


    public async Task<IEnumerable<GeneratedDocumentInfo>> ListDocumentsAsync(string entityType, int entityId)
    {
        var results = new List<GeneratedDocumentInfo>();

        try
        {
            var shareClient = _shareServiceClient.GetShareClient(_fileShareName);
            var basePath = $"{entityType}/{entityId}";
            var baseDirectory = shareClient.GetDirectoryClient(basePath);

            if (!await baseDirectory.ExistsAsync())
            {
                return results;
            }

            // Enumerate all document type subdirectories
            await foreach (var item in baseDirectory.GetFilesAndDirectoriesAsync())
            {
                if (item.IsDirectory)
                {
                    var docTypeDirectory = baseDirectory.GetSubdirectoryClient(item.Name);
                    await foreach (var file in docTypeDirectory.GetFilesAndDirectoriesAsync())
                    {
                        if (!file.IsDirectory)
                        {
                            var fileClient = docTypeDirectory.GetFileClient(file.Name);
                            var props = await fileClient.GetPropertiesAsync();

                            var originalFileName = props.Value.Metadata.TryGetValue("OriginalFileName", out var fn) ? fn : file.Name;
                            var generatedAtStr = props.Value.Metadata.TryGetValue("GeneratedAt", out var ga) ? ga : null;
                            var generatedAt = generatedAtStr != null ? DateTime.Parse(generatedAtStr) : props.Value.LastModified.DateTime;

                            results.Add(new GeneratedDocumentInfo(
                                DocumentPath: $"{basePath}/{item.Name}/{file.Name}",
                                FileName: originalFileName,
                                DocumentTypeName: item.Name,
                                SizeBytes: file.FileSize ?? 0,
                                GeneratedAt: generatedAt));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing documents for {EntityType}/{EntityId}", entityType, entityId);
        }

        return results.OrderByDescending(d => d.GeneratedAt);
    }

    #region Private Helpers

    private async Task<ShareDirectoryClient> EnsureDirectoryExistsAsync(ShareClient shareClient, string folderPath)
    {
        var parts = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentDirectory = shareClient.GetRootDirectoryClient();

        foreach (var part in parts)
        {
            currentDirectory = currentDirectory.GetSubdirectoryClient(part);
            await currentDirectory.CreateIfNotExistsAsync();
        }

        return currentDirectory;
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "document" : sanitized;
    }

    private static async Task<(Stream Stream, long Length, bool ShouldDispose)> PrepareUploadAsync(Stream content)
    {
        if (content.CanSeek)
        {
            content.Position = 0;
            return (content, content.Length, false);
        }

        // Copy to memory stream if source isn't seekable
        var ms = new MemoryStream();
        await content.CopyToAsync(ms);
        ms.Position = 0;
        return (ms, ms.Length, true);
    }

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension switch
        {
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".rtf" => "application/rtf",
            _ => "application/octet-stream"
        };
    }

    #endregion
}
