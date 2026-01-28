using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for View data access
/// </summary>
public class ViewRepository
{
    private readonly SsrDbContext _context;

    public ViewRepository(SsrDbContext context)
    {
        _context = context;
    }

    #region View Operations

    /// <summary>
    /// Get all views for a specific module
    /// </summary>
    public IQueryable<View> GetViewsAsync(string module = "Acquisition")
    {
        return _context.Views
            .Where(v => v.Module == module)
            .AsNoTracking();
    }

    /// <summary>
    /// Get view by ID with fields
    /// </summary>
    public async Task<View?> GetByIdAsync(int viewId)
    {
        return await _context.Views
            .Include(v => v.ViewFields!)
            .FirstOrDefaultAsync(v => v.ViewID == viewId);
    }

    /// <summary>
    /// Get view by ID without tracking
    /// </summary>
    public async Task<View?> GetByIdNoTrackingAsync(int viewId)
    {
        return await _context.Views
            .Include(v => v.ViewFields!)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.ViewID == viewId);
    }

    /// <summary>
    /// Add new view
    /// </summary>
    public async Task<View> AddAsync(View view)
    {
        _context.Views.Add(view);
        await _context.SaveChangesAsync();
        return view;
    }

    /// <summary>
    /// Update existing view
    /// </summary>
    public async Task<View> UpdateAsync(View view)
    {
        _context.Views.Update(view);
        await _context.SaveChangesAsync();
        return view;
    }

    /// <summary>
    /// Delete view (hard delete)
    /// </summary>
    public async Task DeleteAsync(View view)
    {
        _context.Views.Remove(view);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Hard delete view and its fields
    /// </summary>
    public async Task HardDeleteAsync(View view)
    {
        // Delete view fields first
        var viewFields = await _context.ViewFields
            .Where(vf => vf.ViewID == view.ViewID)
            .ToListAsync();

        _context.ViewFields.RemoveRange(viewFields);
        _context.Views.Remove(view);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Check if view name already exists within a module
    /// </summary>
    public async Task<bool> ViewNameExistsAsync(string viewName, string module = "Acquisition", int? excludeViewId = null)
    {
        var query = _context.Views
            .Where(v => v.ViewName == viewName && v.Module == module);

        if (excludeViewId.HasValue)
        {
            query = query.Where(v => v.ViewID != excludeViewId.Value);
        }

        return await query.AnyAsync();
    }

    #endregion

    #region Field Operations

    /// <summary>
    /// Get fields for a specific view
    /// </summary>
    public async Task<List<ViewField>> GetViewFieldsAsync(int viewId)
    {
        return await _context.ViewFields
            .Where(vf => vf.ViewID == viewId)
            .OrderBy(vf => vf.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion

    #region ViewField Operations

    /// <summary>
    /// Add view field
    /// </summary>
    public async Task<ViewField> AddViewFieldAsync(ViewField viewField)
    {
        _context.ViewFields.Add(viewField);
        await _context.SaveChangesAsync();
        return viewField;
    }

    /// <summary>
    /// Add multiple view fields
    /// </summary>
    public async Task AddViewFieldsAsync(IEnumerable<ViewField> viewFields)
    {
        _context.ViewFields.AddRange(viewFields);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove view field
    /// </summary>
    public async Task RemoveViewFieldAsync(ViewField viewField)
    {
        _context.ViewFields.Remove(viewField);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove all fields from a view
    /// </summary>
    public async Task RemoveAllViewFieldsAsync(int viewId)
    {
        var viewFields = await _context.ViewFields
            .Where(vf => vf.ViewID == viewId)
            .ToListAsync();

        _context.ViewFields.RemoveRange(viewFields);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Update view fields - removes existing and adds new
    /// </summary>
    public async Task UpdateViewFieldsAsync(int viewId, IEnumerable<ViewField> newViewFields)
    {
        // Remove existing
        var existingFields = await _context.ViewFields
            .Where(vf => vf.ViewID == viewId)
            .ToListAsync();

        _context.ViewFields.RemoveRange(existingFields);

        // Add new
        _context.ViewFields.AddRange(newViewFields);

        await _context.SaveChangesAsync();
    }

    #endregion

    #region User Page Preference Operations

    /// <summary>
    /// Get user's preferred view for a specific page
    /// </summary>
    public async Task<UserPagePreference?> GetUserPagePreferenceAsync(int userId, string pageName)
    {
        return await _context.UserPagePreferences
            .Include(p => p.View)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserID == userId && p.PageName == pageName);
    }

    /// <summary>
    /// Save or update user's page preference
    /// </summary>
    public async Task SaveUserPagePreferenceAsync(UserPagePreference preference)
    {
        var existing = await _context.UserPagePreferences
            .FirstOrDefaultAsync(p => p.UserID == preference.UserID && p.PageName == preference.PageName);

        if (existing == null)
        {
            _context.UserPagePreferences.Add(preference);
        }
        else
        {
            existing.ViewID = preference.ViewID;
            _context.UserPagePreferences.Update(existing);
        }

        await _context.SaveChangesAsync();
    }

    #endregion
}