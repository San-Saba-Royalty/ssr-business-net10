using System;
using System.IO;
using System.Threading.Tasks;

namespace SSRBusiness.Interfaces;

public interface IFileService
{
    /// <summary>
    /// Saves bytes to a temporary file and returns a unique file ID.
    /// </summary>
    Task<string> SaveTempFileAsync(string fileName, byte[] content);

    /// <summary>
    /// Saves a stream to a temporary file and returns a unique file ID.
    /// </summary>
    Task<string> SaveTempFileAsync(string fileName, Stream content);

    /// <summary>
    /// Retrieves the file path and metadata for a given file ID.
    /// Returns (FilePath, ContentType, FileName) or throws if not found.
    /// </summary>
    (string FilePath, string ContentType, string FileName) GetTempFile(string fileId);

    /// <summary>
    /// Clean up old temp files (can be called by a background service).
    /// </summary>
    void CleanupTempFiles(TimeSpan maxAge);
}
