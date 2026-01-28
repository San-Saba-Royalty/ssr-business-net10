using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Service for logging changes to Acquisitions and Letter Agreements.
/// Matches the legacy LogAcquisitionChange format for parity with MineralAcquisitionWeb.
/// </summary>
public interface IAcquisitionChangeService
{
    /// <summary>
    /// Logs a change to an acquisition field.
    /// </summary>
    Task LogAcquisitionChangeAsync(int userId, string changeTypeCode, int acquisitionId, 
        string fieldName, string? newValue, string? oldValue);
    
    /// <summary>
    /// Logs multiple field changes for an acquisition.
    /// </summary>
    Task LogAcquisitionChangesAsync(int userId, string changeTypeCode, int acquisitionId, 
        IEnumerable<(string FieldName, string? NewValue, string? OldValue)> changes);
    
    /// <summary>
    /// Logs a change to a letter agreement field.
    /// </summary>
    Task LogLetterAgreementChangeAsync(int userId, string changeTypeCode, int letterAgreementId, 
        string fieldName, string? newValue, string? oldValue);
    
    /// <summary>
    /// Gets the change history for an acquisition.
    /// </summary>
    Task<IList<AcquisitionChange>> GetAcquisitionChangeHistoryAsync(int acquisitionId);
    
    /// <summary>
    /// Gets the change history for a letter agreement.
    /// </summary>
    Task<IList<LetterAgreementChange>> GetLetterAgreementChangeHistoryAsync(int letterAgreementId);
}

/// <summary>
/// Change type codes matching the legacy system.
/// </summary>
public static class ChangeTypeCodes
{
    public const string Edit = "EDIT";
    public const string Create = "NEW";
    public const string Delete = "DEL";
    public const string StatusChange = "STAT";
    public const string DocumentGeneration = "DOC";
    public const string NoteAdded = "NOTE";
}

public class AcquisitionChangeService : IAcquisitionChangeService
{
    private readonly IDbContextFactory<SsrDbContext> _contextFactory;

    public AcquisitionChangeService(IDbContextFactory<SsrDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task LogAcquisitionChangeAsync(int userId, string changeTypeCode, int acquisitionId,
        string fieldName, string? newValue, string? oldValue)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var change = new AcquisitionChange
        {
            AcquisitionId = acquisitionId,
            UserId = userId,
            ChangeTypeCode = changeTypeCode,
            ChangeDate = DateTime.Now,
            FieldName = TruncateToLength(fieldName, 100),
            NewValue = TruncateToLength(newValue, 500),
            OldValue = TruncateToLength(oldValue, 500)
        };

        context.AcquisitionChanges.Add(change);
        await context.SaveChangesAsync();
    }

    public async Task LogAcquisitionChangesAsync(int userId, string changeTypeCode, int acquisitionId,
        IEnumerable<(string FieldName, string? NewValue, string? OldValue)> changes)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var now = DateTime.Now;

        foreach (var (fieldName, newValue, oldValue) in changes)
        {
            // Only log if there's an actual change
            if (newValue != oldValue)
            {
                var change = new AcquisitionChange
                {
                    AcquisitionId = acquisitionId,
                    UserId = userId,
                    ChangeTypeCode = changeTypeCode,
                    ChangeDate = now,
                    FieldName = TruncateToLength(fieldName, 100),
                    NewValue = TruncateToLength(newValue, 500),
                    OldValue = TruncateToLength(oldValue, 500)
                };
                context.AcquisitionChanges.Add(change);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task LogLetterAgreementChangeAsync(int userId, string changeTypeCode, int letterAgreementId,
        string fieldName, string? newValue, string? oldValue)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var change = new LetterAgreementChange
        {
            LetterAgreementId = letterAgreementId,
            UserId = userId,
            ChangeTypeCode = changeTypeCode,
            ChangeDate = DateTime.Now,
            FieldName = TruncateToLength(fieldName, 100),
            NewValue = TruncateToLength(newValue, 500),
            OldValue = TruncateToLength(oldValue, 500)
        };

        context.LetterAgreementChanges.Add(change);
        await context.SaveChangesAsync();
    }

    public async Task<IList<AcquisitionChange>> GetAcquisitionChangeHistoryAsync(int acquisitionId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.AcquisitionChanges
            .Include(c => c.User)
            .Include(c => c.ChangeType)
            .Where(c => c.AcquisitionId == acquisitionId)
            .OrderByDescending(c => c.ChangeDate)
            .ThenByDescending(c => c.AcquisitionChangeId)
            .ToListAsync();
    }

    public async Task<IList<LetterAgreementChange>> GetLetterAgreementChangeHistoryAsync(int letterAgreementId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.LetterAgreementChanges
            .Include(c => c.User)
            .Include(c => c.ChangeType)
            .Where(c => c.LetterAgreementId == letterAgreementId)
            .OrderByDescending(c => c.ChangeDate)
            .ThenByDescending(c => c.LetterAgreementChangeId)
            .ToListAsync();
    }

    private static string? TruncateToLength(string? value, int maxLength)
    {
        if (value == null) return null;
        return value.Length > maxLength ? value[..maxLength] : value;
    }
}
