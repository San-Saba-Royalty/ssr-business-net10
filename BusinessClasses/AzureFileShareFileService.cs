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

public class AzureFileShareFileService : IFileService
{
    private readonly ShareServiceClient _shareServiceClient;
    private readonly ILogger<AzureFileShareFileService> _logger;
    private readonly string _fileShareName;

    public AzureFileShareFileService(IConfiguration configuration, ILogger<AzureFileShareFileService> logger)
    {
        _logger = logger;

        var azureFileShareConfig = configuration.GetSection("AzureFileShare");
        var accountName = azureFileShareConfig["AccountName"];
        var accountKey = azureFileShareConfig["AccountKey"];
        _fileShareName = azureFileShareConfig["FileShareName"] ?? "temp";

        if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountKey))
        {
            // Fallback for development if not configured yet
            _logger.LogWarning("AzureFileShare configuration is missing AccountName or AccountKey. File operations will fail.");
            _shareServiceClient = new ShareServiceClient("UseDevelopmentStorage=true");
        }
        else
        {
            // Build connection string from account name and key
            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";
            _shareServiceClient = new ShareServiceClient(connectionString);
        }
    }

    public async Task<string> SaveTempFileAsync(string fileName, byte[] content)
    {
        using var stream = new MemoryStream(content);
        return await SaveTempFileAsync(fileName, stream);
    }

    public async Task<string> SaveTempFileAsync(string fileName, Stream content)
    {
        var shareClient = _shareServiceClient.GetShareClient(_fileShareName);
        await shareClient.CreateIfNotExistsAsync();

        var directoryClient = shareClient.GetRootDirectoryClient();
        var fileId = Guid.NewGuid().ToString();
        var fileClient = directoryClient.GetFileClient(fileId);

        var (uploadStream, length, shouldDispose) = await PrepareUploadAsync(content);
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
                { "OriginalFileName", fileName }
            });
        }
        finally
        {
            if (shouldDispose) uploadStream.Dispose();
        }

        return fileId;
    }

    public (string FilePath, string ContentType, string FileName) GetTempFile(string fileId)
    {
        var shareClient = _shareServiceClient.GetShareClient(_fileShareName);
        var directoryClient = shareClient.GetRootDirectoryClient();
        var fileClient = directoryClient.GetFileClient(fileId);

        if (!fileClient.Exists())
        {
            throw new FileNotFoundException($"Temp file not found in share '{_fileShareName}': {fileId}");
        }

        var props = fileClient.GetProperties();
        var contentType = props.Value.ContentType;
        var fileName = props.Value.Metadata.TryGetValue("OriginalFileName", out var originalName)
            ? originalName
            : "unknown.dat";

        // Download to local temp for compatibility with existing FileStream logic in controller
        var tempPath = Path.Combine(Path.GetTempPath(), "SSR_Azure_Downloads", fileId);
        Directory.CreateDirectory(Path.GetDirectoryName(tempPath)!);

        var download = fileClient.Download();
        using (var fileStream = File.Create(tempPath))
        {
            download.Value.Content.CopyTo(fileStream);
        }

        return (tempPath, contentType, fileName);
    }

    public void CleanupTempFiles(TimeSpan maxAge)
    {
        try
        {
            var shareClient = _shareServiceClient.GetShareClient(_fileShareName);
            if (!shareClient.Exists()) return;

            var directoryClient = shareClient.GetRootDirectoryClient();
            foreach (var item in directoryClient.GetFilesAndDirectories())
            {
                if (item.IsDirectory) continue;

                var fileClient = directoryClient.GetFileClient(item.Name);
                var props = fileClient.GetProperties();
                var lastModified = props.Value.LastModified.UtcDateTime;

                if (DateTime.UtcNow - lastModified > maxAge)
                {
                    fileClient.DeleteIfExists();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temp files in Azure File Share");
        }
    }

    public async Task<string> UploadFileAsync(string containerName, string fileName, Stream content)
    {
        var shareClient = _shareServiceClient.GetShareClient(containerName);
        await shareClient.CreateIfNotExistsAsync();

        var (directoryClient, actualFileName) = await GetDirectoryForPathAsync(shareClient, fileName);
        var fileClient = directoryClient.GetFileClient(actualFileName);

        var (uploadStream, length, shouldDispose) = await PrepareUploadAsync(content);
        try
        {
            await fileClient.CreateAsync(length);
            await fileClient.UploadRangeAsync(new HttpRange(0, length), uploadStream);
            await fileClient.SetHttpHeadersAsync(new ShareFileSetHttpHeadersOptions
            {
                HttpHeaders = new ShareFileHttpHeaders { ContentType = GetMimeType(actualFileName) }
            });
        }
        finally
        {
            if (shouldDispose) uploadStream.Dispose();
        }

        return fileClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        var shareClient = _shareServiceClient.GetShareClient(containerName);
        var (directoryClient, actualFileName) = await GetDirectoryForPathAsync(shareClient, fileName, createDirectories: false);
        var fileClient = directoryClient.GetFileClient(actualFileName);

        if (!await fileClient.ExistsAsync())
        {
            throw new FileNotFoundException($"File {fileName} not found in share {containerName}");
        }

        var downloadInfo = await fileClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }

    public async Task<bool> DeleteFileAsync(string containerName, string fileName)
    {
        var shareClient = _shareServiceClient.GetShareClient(containerName);
        var (directoryClient, actualFileName) = await GetDirectoryForPathAsync(shareClient, fileName, createDirectories: false);
        var fileClient = directoryClient.GetFileClient(actualFileName);
        return await fileClient.DeleteIfExistsAsync();
    }

    private static async Task<(Stream Stream, long Length, bool ShouldDispose)> PrepareUploadAsync(Stream content)
    {
        if (content.CanSeek)
        {
            if (content.Position != 0) content.Position = 0;
            return (content, content.Length, false);
        }

        var buffer = new MemoryStream();
        await content.CopyToAsync(buffer);
        buffer.Position = 0;
        return (buffer, buffer.Length, true);
    }

    private static async Task<(ShareDirectoryClient Directory, string FileName)> GetDirectoryForPathAsync(
        ShareClient shareClient,
        string path,
        bool createDirectories = true)
    {
        var normalized = path.Replace('\\', '/');
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
        {
            return (shareClient.GetRootDirectoryClient(), parts.Length == 0 ? path : parts[0]);
        }

        var directoryClient = shareClient.GetRootDirectoryClient();
        foreach (var part in parts.Take(parts.Length - 1))
        {
            directoryClient = directoryClient.GetSubdirectoryClient(part);
            if (createDirectories)
            {
                await directoryClient.CreateIfNotExistsAsync();
            }
        }

        return (directoryClient, parts.Last());
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
