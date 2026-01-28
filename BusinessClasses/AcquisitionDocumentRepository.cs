using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for acquisition document operations
/// </summary>
public class AcquisitionDocumentRepository
{
    private readonly SsrDbContext _context;

    public AcquisitionDocumentRepository(SsrDbContext context)
    {
        _context = context;
    }

    #region Document Types

    /// <summary>
    /// Get all document types for dropdown
    /// </summary>
    public IQueryable<DocumentType> GetDocumentTypesAsync()
    {
        return _context.DocumentTypes
            .OrderBy(dt => dt.DocumentTypeDesc);
    }

    /// <summary>
    /// Get document type by code
    /// </summary>
    public async Task<DocumentType?> GetDocumentTypeByCodeAsync(string documentTypeCode)
    {
        return await _context.DocumentTypes
            .FirstOrDefaultAsync(dt => dt.DocumentTypeCode == documentTypeCode);
    }

    #endregion

    #region Document Templates

    /// <summary>
    /// Get document templates by document type
    /// </summary>
    public IQueryable<DocumentTemplate> GetDocumentTemplatesByTypeAsync(string documentTypeCode)
    {
        return _context.DocumentTemplates
            .Where(dt => dt.DocumentTypeCode == documentTypeCode)
            .OrderBy(dt => dt.DocumentTemplateDesc);
    }

    /// <summary>
    /// Get document template by ID
    /// </summary>
    public async Task<DocumentTemplate?> GetDocumentTemplateByIdAsync(int documentTemplateId)
    {
        return await _context.DocumentTemplates
            .FirstOrDefaultAsync(dt => dt.DocumentTemplateID == documentTemplateId);
    }

    /// <summary>
    /// Get custom fields for a document template
    /// </summary>
    public async Task<List<DocTemplateCustomField>> GetTemplateCustomFieldsAsync(int documentTemplateId)
    {
        return await _context.DocTemplateCustomFields
            .Where(cf => cf.DocumentTemplateID == documentTemplateId)
            .ToListAsync();
    }

    #endregion

    #region Acquisition Documents

    /// <summary>
    /// Get documents for an acquisition
    /// </summary>
    public IQueryable<AcquisitionDocument> GetAcquisitionDocumentsAsync(int acquisitionId)
    {
        return _context.AcquisitionDocuments
            .Where(ad => ad.AcquisitionID == acquisitionId)
            .OrderByDescending(ad => ad.AcquisitionDocumentID);
    }

    /// <summary>
    /// Get acquisition document by ID
    /// </summary>
    public async Task<AcquisitionDocument?> GetAcquisitionDocumentByIdAsync(int acquisitionDocumentId)
    {
        return await _context.AcquisitionDocuments
            .FirstOrDefaultAsync(ad => ad.AcquisitionDocumentID == acquisitionDocumentId);
    }

    /// <summary>
    /// Get acquisition document by ID (handle placeholder)
    /// </summary>
    public async Task<AcquisitionDocument?> GetAcquisitionDocumentByHandleAsync(string handle)
    {
        // Note: Handle column doesn't exist in database yet
        // Using ID as fallback if handle is numeric
        if (int.TryParse(handle, out var id))
        {
            return await GetAcquisitionDocumentByIdAsync(id);
        }
        return null;
    }

    /// <summary>
    /// Add new acquisition document record
    /// </summary>
    public async Task<AcquisitionDocument> AddAcquisitionDocumentAsync(AcquisitionDocument document)
    {
        _context.AcquisitionDocuments.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    /// <summary>
    /// Add acquisition note
    /// </summary>
    public async Task AddAcquisitionNoteAsync(AcquisitionNote note)
    {
        _context.AcquisitionNotes.Add(note);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Delete acquisition document
    /// </summary>
    public async Task DeleteAcquisitionDocumentAsync(AcquisitionDocument document)
    {
        _context.AcquisitionDocuments.Remove(document);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Delete acquisition document by handle
    /// </summary>
    public async Task<bool> DeleteAcquisitionDocumentByHandleAsync(string handle)
    {
        var document = await GetAcquisitionDocumentByHandleAsync(handle);
        if (document == null)
            return false;

        _context.AcquisitionDocuments.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Multi-Table Search

    /// <summary>
    /// Search for acquisition documents
    /// </summary>
    /// <param name="searchText">Text to search for (in document location/filename)</param>
    /// <param name="includeDetails">Whether to include related acquisition details</param>
    /// <returns>List of search results</returns>
    public async Task<List<DocumentSearchResultItem>> SearchDocumentsAsync(string searchText, bool includeDetails)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return new List<DocumentSearchResultItem>();

        var query = _context.AcquisitionDocuments
            .Include(ad => ad.User)
            .AsQueryable();

        // Basic Filter
        query = query.Where(ad => ad.DocumentLocation != null && ad.DocumentLocation.Contains(searchText));

        if (includeDetails)
        {
            query = query
                .Include(ad => ad.Acquisition)
                .ThenInclude(a => a!.AcquisitionSellers) // No .ThenInclude(Seller) as it doesn't exist
                .Include(ad => ad.Acquisition)
                .ThenInclude(a => a!.AcquisitionCounties).ThenInclude(c => c.County)
                .Include(ad => ad.Acquisition)
                .ThenInclude(a => a!.AcquisitionOperators).ThenInclude(o => o.Operator)
                .Include(ad => ad.Acquisition)
                .ThenInclude(a => a!.AcquisitionBuyers).ThenInclude(b => b.Buyer);
        }

        var entities = await query
            .OrderByDescending(ad => ad.CreatedOn)
            .Take(100) // Limit results
            .ToListAsync();

        // Project to DTO
        var results = new List<DocumentSearchResultItem>();

        foreach (var doc in entities)
        {
            var item = new DocumentSearchResultItem
            {
                AcquisitionDocumentID = doc.AcquisitionDocumentID,
                AcquisitionID = doc.AcquisitionID,
                DocumentLocation = doc.DocumentLocation,
                CreatedOn = doc.CreatedOn,
                CreatedByName = doc.User?.UserName ?? "Unknown"
            };

            if (includeDetails && doc.Acquisition != null)
            {
                var acq = doc.Acquisition;

                // Sellers - use SellerName directly from AcquisitionSeller
                var sellers = acq.AcquisitionSellers
                    .Select(s => s.SellerName)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                item.SellerName = sellers.Any() ? string.Join(", ", sellers) : null;

                // Effective Date
                item.EffectiveDate = acq.EffectiveDate;

                // Counties
                var counties = acq.AcquisitionCounties.Select(c => c.County?.CountyName).Where(c => !string.IsNullOrEmpty(c)).ToList();
                item.Counties = counties.Any() ? string.Join(", ", counties) : null;

                // Operators
                var operators = acq.AcquisitionOperators.Select(o => o.Operator?.OperatorName).Where(o => !string.IsNullOrEmpty(o)).ToList();
                item.Operators = operators.Any() ? string.Join(", ", operators) : null;

                // Buyers
                var buyers = acq.AcquisitionBuyers.Select(b => b.Buyer?.BuyerName).Where(b => !string.IsNullOrEmpty(b)).ToList();
                item.Buyers = buyers.Any() ? string.Join(", ", buyers) : null;
            }

            results.Add(item);
        }

        return results;
    }

    #endregion

    #region Counties and Operators for Document Generation

    /// <summary>
    /// Get counties associated with an acquisition (for document generation)
    /// </summary>
    public IQueryable<County> GetAcquisitionCountiesAsync(int acquisitionId)
    {
        return _context.AcquisitionCounties
            .Where(ac => ac.AcquisitionID == acquisitionId)
            .Select(ac => ac.County!)
            .Distinct()
            .OrderBy(c => c.CountyName);
    }

    /// <summary>
    /// Get operators associated with an acquisition (for document generation)
    /// </summary>
    public IQueryable<Operator> GetAcquisitionOperatorsAsync(int acquisitionId)
    {
        return _context.AcquisitionOperators
            .Where(ao => ao.AcquisitionID == acquisitionId)
            .Select(ao => ao.Operator!)
            .Distinct()
            .OrderBy(o => o.OperatorName);
    }

    #endregion
}