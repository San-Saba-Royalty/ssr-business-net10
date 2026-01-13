using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for DisplayField data access
/// </summary>
public class DisplayFieldRepository
{
    private readonly SsrDbContext _context;

    public DisplayFieldRepository(SsrDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all display fields ordered by display order
    /// </summary>
    public async Task<List<DisplayField>> GetDisplayFieldsAsync()
    {
        return await _context.DisplayFields
            .OrderBy(f => f.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get display field by ID
    /// </summary>
    public async Task<DisplayField?> GetByIdAsync(int fieldId)
    {
        return await _context.DisplayFields
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FieldID == fieldId);
    }
}
