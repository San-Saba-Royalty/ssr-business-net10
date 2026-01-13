using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SSRBusiness.Interfaces;

namespace SSRBusiness.BusinessClasses;

public class FileService : IFileService
{
    private readonly string _tempDirectory;
    private readonly ILogger<FileService> _logger;
    private readonly ConcurrentDictionary<string, (string FileName, string ContentType)> _fileMetadata = new();

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
        // Create a specific temp directory for the application
        _tempDirectory = Path.Combine(Path.GetTempPath(), "SSR_Downloads");
        if (!Directory.Exists(_tempDirectory))
        {
            Directory.CreateDirectory(_tempDirectory);
        }
    }

    public async Task<string> SaveTempFileAsync(string fileName, byte[] content)
    {
        var fileId = Guid.NewGuid().ToString();
        var filePath = Path.Combine(_tempDirectory, fileId);

        await File.WriteAllBytesAsync(filePath, content);

        _fileMetadata[fileId] = (fileName, GetMimeType(fileName));
        
        return fileId;
    }

    public async Task<string> SaveTempFileAsync(string fileName, Stream content)
    {
        var fileId = Guid.NewGuid().ToString();
        var filePath = Path.Combine(_tempDirectory, fileId);

        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            await content.CopyToAsync(fileStream);
        }

        _fileMetadata[fileId] = (fileName, GetMimeType(fileName));

        return fileId;
    }

    public (string FilePath, string ContentType, string FileName) GetTempFile(string fileId)
    {
        var filePath = Path.Combine(_tempDirectory, fileId);
        
        if (!File.Exists(filePath) || !_fileMetadata.TryGetValue(fileId, out var metadata))
        {
            throw new FileNotFoundException($"Temp file not found: {fileId}");
        }

        return (filePath, metadata.ContentType, metadata.FileName);
    }

    public void CleanupTempFiles(TimeSpan maxAge)
    {
        try
        {
            var directory = new DirectoryInfo(_tempDirectory);
            var now = DateTime.UtcNow;

            foreach (var file in directory.GetFiles())
            {
                if (now - file.CreationTimeUtc > maxAge)
                {
                    try
                    {
                        file.Delete();
                        // Also try to remove from metadata, though it's transient in memory
                        _fileMetadata.TryRemove(file.Name, out _);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old temp file {FilePath}", file.FullName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during temp file cleanup");
        }
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
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Updated from legacy
            ".rtf" => "application/rtf", // Updated from legacy "application/msword"
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
