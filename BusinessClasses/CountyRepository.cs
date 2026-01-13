using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for County data access
/// </summary>
public class CountyRepository
{
    private readonly SsrDbContext _context;

    public CountyRepository(SsrDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all counties as IQueryable for filtering/sorting
    /// </summary>
    public IQueryable<County> GetCountiesAsync()
    {
        return _context.Counties.AsNoTracking();
    }

    /// <summary>
    /// Get county by ID
    /// </summary>
    public async Task<County?> GetByIdAsync(int countyId)
    {
        return await _context.Counties
            .FirstOrDefaultAsync(c => c.CountyID == countyId);
    }

    /// <summary>
    /// Add new county
    /// </summary>
    public async Task<County> AddAsync(County county)
    {
        _context.Counties.Add(county);
        await _context.SaveChangesAsync();
        return county;
    }

    /// <summary>
    /// Update existing county
    /// </summary>
    public async Task<County> UpdateAsync(County county)
    {
        _context.Counties.Update(county);
        await _context.SaveChangesAsync();
        return county;
    }

    /// <summary>
    /// Delete county
    /// </summary>
    public async Task DeleteAsync(County county)
    {
        _context.Counties.Remove(county);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Check if county name already exists
    /// </summary>
    public async Task<bool> CountyNameExistsAsync(string countyName, int? excludeCountyId = null)
    {
        var query = _context.Counties
            .Where(c => c.CountyName == countyName);

        if (excludeCountyId.HasValue)
        {
            query = query.Where(c => c.CountyID != excludeCountyId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Check if county has associated acquisitions
    /// </summary>
    public async Task<bool> HasAssociatedAcquisitionsAsync(int countyId)
    {
        return await _context.AcquisitionCounties
            .AnyAsync(ac => ac.CountyID == countyId);
    }
}