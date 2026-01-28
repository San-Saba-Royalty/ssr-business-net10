using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSRBusiness.Interfaces;

namespace SSRBusiness.BusinessClasses;

public class AzureBlobFileService : IFileService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobFileService> _logger;
    private const string TempContainerName = "temp";

    public AzureBlobFileService(IConfiguration configuration, ILogger<AzureBlobFileService> logger)
    {
        _logger = logger;
        var connectionString = configuration.GetSection("AzureStorage:ConnectionString").Value;

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback for development if not configured yet, ensuring it doesn't crash on startup but will fail on usage
            _logger.LogWarning("AzureStorage:ConnectionString is missing. File operations will fail.");
            connectionString = "UseDevelopmentStorage=true";
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> SaveTempFileAsync(string fileName, byte[] content)
    {
        using var stream = new MemoryStream(content);
        return await SaveTempFileAsync(fileName, stream);
    }

    public async Task<string> SaveTempFileAsync(string fileName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(TempContainerName);
        await containerClient.CreateIfNotExistsAsync();

        var fileId = Guid.NewGuid().ToString();
        var blobClient = containerClient.GetBlobClient(fileId);

        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetMimeType(fileName)
            },
            Metadata = new Dictionary<string, string>
            {
                { "OriginalFileName", fileName }
            }
        };

        await blobClient.UploadAsync(content, blobUploadOptions);
        return fileId;
    }

    public (string FilePath, string ContentType, string FileName) GetTempFile(string fileId)
    {
        // For Azure, we can't return a local file path easily for the existing controller logic 
        // which expects a physical path.
        // However, the DownloadController uses FileStream to read the path.
        // If we want to support the existing controller without changing it significantly, 
        // we might need to download the blob to a temp file locally first.

        var containerClient = _blobServiceClient.GetBlobContainerClient(TempContainerName);
        var blobClient = containerClient.GetBlobClient(fileId);

        if (!blobClient.Exists())
        {
            throw new FileNotFoundException($"Temp file blob not found: {fileId}");
        }

        var props = blobClient.GetProperties();
        var contentType = props.Value.ContentType;
        var fileName = props.Value.Metadata.TryGetValue("OriginalFileName", out var originalName)
            ? originalName
            : "unknown.dat";

        // Download to local temp for compatibility with existing FileStream logic in controller
        var tempPath = Path.Combine(Path.GetTempPath(), "SSR_Azure_Downloads", fileId);
        Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);

        blobClient.DownloadTo(tempPath);

        return (tempPath, contentType, fileName);
    }

    public void CleanupTempFiles(TimeSpan maxAge)
    {
        // NOTE: Lifecycle management policies in Azure are better for this.
        // But implementing manual cleanup for parity.
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(TempContainerName);
            if (!containerClient.Exists()) return;

            var blobs = containerClient.GetBlobs(new GetBlobsOptions { Traits = BlobTraits.Metadata });
            foreach (var blob in blobs)
            {
                if (DateTime.UtcNow - blob.Properties.CreatedOn > maxAge)
                {
                    containerClient.DeleteBlobIfExists(blob.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temp blobs");
        }
    }

    public async Task<string> UploadFileAsync(string containerName, string fileName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobClient = containerClient.GetBlobClient(fileName);

        // Check if exists to handle versioning? 
        // The Service layer handles versioning naming (e.g. file_v1.pdf), 
        // so here we just upload/overwrite.

        var headers = new BlobHttpHeaders { ContentType = GetMimeType(fileName) };
        await blobClient.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers });

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob {fileName} not found in {containerName}");
        }

        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }

    public async Task<bool> DeleteFileAsync(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }

    private string GetMimeType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".htm" => "text/html",
            ".html" => "text/html",
            ".txt" => "text/plain",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".rtf" => "application/rtf",
            ".pdf" => "application/pdf",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}
