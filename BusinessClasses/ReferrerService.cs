using System.Text.RegularExpressions;

using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.Interfaces;

namespace SSRBusiness.BusinessClasses;

public class ReferrerService
{
    private readonly ReferrerRepository _referrerRepository;
    private readonly ReferrerFormRepository _referrerFormRepository;
    private readonly IFileService _fileService;
    private readonly SsrDbContext _context;

    public ReferrerService(
        ReferrerRepository referrerRepository,
        ReferrerFormRepository referrerFormRepository,
        IFileService fileService,
        SsrDbContext context) // Sometimes direct context access is needed for transactions
    {
        _referrerRepository = referrerRepository;
        _referrerFormRepository = referrerFormRepository;
        _fileService = fileService;
        _context = context;
    }

    public async Task<Referrer?> GetReferrerAsync(int id)
    {
        return await _referrerRepository.GetByIdAsync(id);
    }
    
    public async Task<List<Referrer>> GetAllReferrersAsync()
    {
        return await _referrerRepository.GetAllAsync();
    }

    public async Task SaveReferrerAsync(Referrer referrer)
    {
        if (string.IsNullOrWhiteSpace(referrer.ReferrerName)) throw new ArgumentException("Referrer Name is required.");

        if (await _referrerRepository.DoesReferrerNameExistAsync(referrer.ReferrerName, referrer.ReferrerID))
        {
            throw new ArgumentException($"Referrer '{referrer.ReferrerName}' already exists.");
        }

        if (!string.IsNullOrWhiteSpace(referrer.ContactEmail) && !IsValidEmail(referrer.ContactEmail))
        {
            throw new ArgumentException("Invalid email address.");
        }

        // Format TaxID
        if (!string.IsNullOrWhiteSpace(referrer.ReferrerTaxID))
        {
            if (referrer.ReferrerTypeCode == "FEIN")
            {
                if (!IsValidFEIN(referrer.ReferrerTaxID))
                    throw new ArgumentException("Invalid FEIN.");
                referrer.ReferrerTaxID = FormatFEIN(referrer.ReferrerTaxID);
            }
            else if (referrer.ReferrerTypeCode == "SSN")
            {
                if (!IsValidSSN(referrer.ReferrerTaxID))
                    throw new ArgumentException("Invalid SSN.");
                 referrer.ReferrerTaxID = FormatSSN(referrer.ReferrerTaxID);
            }
        }
        else
        {
             // Clear if empty
             referrer.ReferrerTaxID = string.Empty;
        }

        referrer.LastUpdatedOn = DateTime.Now;
        if (referrer.ReferrerID == 0)
        {
            referrer.CreatedOn = DateTime.Now;
            await _referrerRepository.AddAsync(referrer);
        }
        else
        {
            _referrerRepository.Update(referrer);
        }
        await _referrerRepository.SaveChangesAsync();
    }

    public async Task DeleteReferrerAsync(int id)
    {
        await _referrerRepository.DeleteWithGuardAsync(id);
    }

    // Forms Logic
    public async Task<List<ReferrerForm>> GetReferrerFormsAsync(int referrerId)
    {
        return await _referrerFormRepository.GetFormsByReferrerIdAsync(referrerId);
    }

    public async Task UploadReferrerFormAsync(ReferrerForm form, Stream fileStream, string fileName)
    {
        if (await _referrerFormRepository.FormYearExistsAsync(form.ReferrerID, form.FormYear, form.FormTypeCode))
        {
            throw new ArgumentException("There is already a form for the selected year and type.");
        }

        // Generating a unique ID for the file
        string fileId = Guid.NewGuid().ToString();
        string extension = Path.GetExtension(fileName);
        string storageName = $"{fileId}{extension}";
        
        // Where to store? Let's use a Data/ReferrerForms folder.
        // Ideally this path logic should be in FileService or Config.
        // Use CommonApplicationData but fallback to safe temp path if access denied/testing
        string uploadPath;
        try 
        {
             uploadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SSR", "ReferrerForms");
             if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
        }
        catch (UnauthorizedAccessException)
        {
             // Fallback for tests or restricted environments
             uploadPath = Path.Combine(Path.GetTempPath(), "SSR", "ReferrerForms");
             Directory.CreateDirectory(uploadPath);
        }

        string fullPath = Path.Combine(uploadPath, storageName);
        
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }
        
        form.DSFileID = fileId; 
        
        await _referrerFormRepository.AddAsync(form);
        await _referrerFormRepository.SaveChangesAsync();
    }
    
    // Helpers
    private bool IsValidEmail(string email)
    {
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch {
            return false;
        }
    }
    
    private bool IsValidFEIN(string input)
    {
        string digits = Regex.Replace(input, "[^0-9]", "");
        return digits.Length == 9;
    }
    
    private string FormatFEIN(string input)
    {
        string digits = Regex.Replace(input, "[^0-9]", "");
        if (digits.Length == 9)
            return $"{digits.Substring(0, 2)}-{digits.Substring(2)}";
        return input;
    }
    
    private bool IsValidSSN(string input)
    {
        string digits = Regex.Replace(input, "[^0-9]", "");
        return digits.Length == 9;
    }
    
    private string FormatSSN(string input)
    {
        string digits = Regex.Replace(input, "[^0-9]", "");
        if (digits.Length == 9)
            return $"{digits.Substring(0, 3)}-{digits.Substring(3, 2)}-{digits.Substring(5)}";
        return input;
    }
}
