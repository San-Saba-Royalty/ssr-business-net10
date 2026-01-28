using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SSRBusiness.Entities;
using System.Text.RegularExpressions;
using DocSharp.Binary.DocFileFormat;
using DocSharp.Binary.OpenXmlLib.WordprocessingML;
using DocSharp.Binary.WordprocessingMLMapping;
using DocSharp.Binary.StructuredStorage.Reader;

namespace SSRBusiness.BusinessClasses;

public class WordTemplateEngine
{
    /// <summary>
    /// Converts a .doc file to .docx format using DocSharp.Binary (b2xtranslator).
    /// Returns the path to a .docx file (either original or converted temp file).
    /// </summary>
    /// <param name="sourcePath">Path to the source .doc or .docx file</param>
    /// <returns>Tuple of (docxPath, needsCleanup) where needsCleanup indicates if we created a temp file</returns>
    private (string docxPath, bool needsCleanup) EnsureDocxFormat(string sourcePath)
    {
        var extension = Path.GetExtension(sourcePath);

        if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
        {
            return (sourcePath, false); // Already .docx, no conversion needed
        }

        if (extension.Equals(".doc", StringComparison.OrdinalIgnoreCase))
        {
            // Convert .doc to .docx using DocSharp.Binary (b2xtranslator)
            var docxPath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(sourcePath)}_{Guid.NewGuid()}.docx");

            using (var reader = new StructuredStorageReader(sourcePath))
            {
                var doc = new WordDocument(reader);

                // Create the .docx output
                using var docxStream = File.Create(docxPath);
                var docxDoc = DocSharp.Binary.OpenXmlLib.WordprocessingML.WordprocessingDocument.Create(docxStream, DocSharp.Binary.OpenXmlLib.WordprocessingDocumentType.Document);

                // Use the Converter to transform .doc to .docx
                Converter.Convert(doc, docxDoc);

                docxDoc.Dispose();
            }

            return (docxPath, true);
        }

        throw new NotSupportedException($"Unsupported file format: {extension}. Only .doc and .docx files are supported.");
    }

    /// <summary>
    /// Public method to convert a .doc file to .docx format if necessary.
    /// Returns the path to a .docx file (either original or converted temp file).
    /// </summary>
    /// <param name="sourcePath">Path to the source .doc or .docx file</param>
    /// <returns>Tuple of (docxPath, needsCleanup) where needsCleanup indicates if we created a temp file</returns>
    public (string docxPath, bool needsCleanup) ConvertToDocxIfNeeded(string sourcePath)
    {
        return EnsureDocxFormat(sourcePath);
    }

    public void CreateMergeDocument(string sourcePath, string destPath, Acquisition acquisition, User? user, string? documentTemplateID = null)
    {
        CreateMergeDocument(sourcePath, destPath, acquisition, user, null, documentTemplateID);
    }

    /// <summary>
    /// Creates a merged document with context-aware entity selection.
    /// When context specifies a CountyId, OperatorId, or BuyerId, only that entity is merged
    /// for singular placeholders, matching legacy behavior.
    /// </summary>
    public void CreateMergeDocument(string sourcePath, string destPath, Acquisition acquisition, User? user, DocumentGenerationContext? context, string? documentTemplateID = null)
    {
        if (!File.Exists(sourcePath)) throw new FileNotFoundException("Template not found", sourcePath);

        // Convert .doc to .docx if necessary
        var (docxSourcePath, needsCleanup) = EnsureDocxFormat(sourcePath);

        try
        {
            // Copy template to destination (ensure .docx extension)
            var actualDestPath = Path.ChangeExtension(destPath, ".docx");
            File.Copy(docxSourcePath, actualDestPath, true);

            using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(actualDestPath, true))
            {
                var body = doc.MainDocumentPart?.Document.Body;
                if (body != null)
                {
                    MergeGlobalData(body);
                    if (user != null) MergeUserData(user, body);
                    MergeAcquisitionData(acquisition, body);

                    // Merge sub-entities
                    foreach (var seller in acquisition.AcquisitionSellers)
                    {
                        MergeAcquisitionSellerData(seller, body);
                    }

                    foreach (var buyer in acquisition.AcquisitionBuyers)
                    {
                        MergeBuyerData(buyer, documentTemplateID, body);
                    }

                    // Context-aware County/Operator merging:
                    // If context specifies a specific ID, merge only that entity for singular placeholders.
                    // Otherwise, fall back to first-found behavior.
                    if (context?.CountyId.HasValue == true)
                    {
                        var selectedCounty = acquisition.AcquisitionCounties
                            .FirstOrDefault(c => c.AcquisitionCountyID == context.CountyId.Value);
                        if (selectedCounty != null)
                        {
                            MergeSingleCountyData(selectedCounty, documentTemplateID, body, acquisition.AcquisitionID);
                        }
                    }
                    else if (acquisition.AcquisitionCounties.Any())
                    {
                        MergeSingleCountyData(acquisition.AcquisitionCounties.First(), documentTemplateID, body, acquisition.AcquisitionID);
                    }

                    if (context?.OperatorId.HasValue == true)
                    {
                        var selectedOperator = acquisition.AcquisitionOperators
                            .FirstOrDefault(o => o.AcquisitionOperatorID == context.OperatorId.Value);
                        if (selectedOperator != null)
                        {
                            MergeSingleOperatorData(selectedOperator, documentTemplateID, body, acquisition.AcquisitionID);
                        }
                    }
                    else if (acquisition.AcquisitionOperators.Any())
                    {
                        MergeSingleOperatorData(acquisition.AcquisitionOperators.First(), documentTemplateID, body, acquisition.AcquisitionID);
                    }

                    MergeAcquisitionUnitData(acquisition, body);
                    MergeAcquisitionCountyData(acquisition, body); // Lists
                    MergeAcquisitionOperatorData(acquisition, body); // Lists

                    // Merge custom fields from context if provided
                    if (context?.CustomFields?.Count > 0)
                    {
                        MergeCustomFields(doc, context.CustomFields);
                    }

                    // Merge barcode-specific context fields if provided
                    if (context != null)
                    {
                        MergeContextData(context, body);
                    }

                    // Save changes
                    doc.MainDocumentPart?.Document.Save();
                }
            }
        }
        finally
        {
            // Cleanup converted temp file
            if (needsCleanup && File.Exists(docxSourcePath))
            {
                try { File.Delete(docxSourcePath); } catch { /* Ignore cleanup errors */ }
            }
        }
    }

    /// <summary>
    /// Merges custom fields from DocTemplateCustomField definitions.
    /// </summary>
    private void MergeCustomFields(DocumentFormat.OpenXml.Packaging.WordprocessingDocument doc, Dictionary<string, string> customFields)
    {
        foreach (var (tag, value) in customFields)
        {
            SearchAndReplace(doc.MainDocumentPart!.Document.Body!, $"<{tag}>", value);
        }
    }


    private void MergeGlobalData(Body body)
    {
        SearchAndReplace(body, "<Date>", DateTime.Now.ToString("MMMM dd, yyyy"));
        SearchAndReplace(body, "<Day>", DateTime.Now.ToString("dd"));
        SearchAndReplace(body, "<Year>", DateTime.Now.ToString("yyyy"));
        SearchAndReplace(body, "<Month>", DateTime.Now.ToString("MM"));
        SearchAndReplace(body, "<MonthName>", DateTime.Now.ToString("MMMM"));
        SearchAndReplace(body, "<DayX>", GetCurrentDaySuffix());
    }

    private string GetCurrentDaySuffix()
    {
        string day = DateTime.Now.Day.ToString();
        int dayNum = DateTime.Now.Day;
        switch (dayNum)
        {
            case 1: case 21: case 31: return day + "st";
            case 2: case 22: return day + "nd";
            case 3: case 23: return day + "rd";
            default: return day + "th";
        }
    }

    private void MergeUserData(User user, Body body)
    {
        SearchAndReplace(body, "<UserFirstName>", user.FirstName ?? "");
        SearchAndReplace(body, "<UserLastName>", user.LastName ?? "");

        // Support for <user> placeholder - use full name or fallback to username
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrEmpty(fullName))
        {
            fullName = user.UserName ?? user.Email ?? "";
        }
        SearchAndReplace(body, "<user>", fullName);
    }

    private void MergeAcquisitionData(Acquisition acq, Body body)
    {
        SearchAndReplace(body, "<AcquisitionID>", acq.AcquisitionID.ToString());
        SearchAndReplace(body, "<AcquisitionName>", acq.ToString() ?? ""); // Need meaningful string 
        SearchAndReplace(body, "<EffectiveDate>", DisplayDate(acq.EffectiveDate));
        SearchAndReplace(body, "<BuyerEffectiveDate>", DisplayDate(acq.BuyerEffectiveDate));
        SearchAndReplace(body, "<DueDate>", DisplayDate(acq.DueDate));
        SearchAndReplace(body, "<TotalBonus>", DisplayAmount(acq.TotalBonus));
        SearchAndReplace(body, "<DraftFee>", DisplayAmount(acq.DraftFee));
        SearchAndReplace(body, "<TotalBonusAndFee>", DisplayAmount(acq.TotalBonusAndFee));
        SearchAndReplace(body, "<Draft+Fee>", DisplayAmount(acq.TotalBonusAndFee));
        SearchAndReplace(body, "<DraftCheckNumber>", DisplayText(acq.DraftCheckNumber));
        SearchAndReplace(body, "<DraftNumber>", DisplayText(acq.DraftCheckNumber));
        SearchAndReplace(body, "<CheckNumber>", DisplayText(acq.DraftCheckNumber));
        SearchAndReplace(body, "<AcquisitionNumber>", DisplayText(acq.AcquisitionNumber));
        SearchAndReplace(body, "<Assignee>", DisplayText(acq.Assignee));
        SearchAndReplace(body, "<InvoiceNumber>", DisplayText(acq.InvoiceNumber));
        SearchAndReplace(body, "<InvoiceDate>", DisplayDate(acq.InvoiceDate));
        SearchAndReplace(body, "<InvoiceDueDate>", DisplayDate(acq.InvoiceDueDate));
        SearchAndReplace(body, "<InvoicePaidDate>", DisplayDate(acq.InvoicePaidDate));
        SearchAndReplace(body, "<InvoiceTotal>", DisplayAmount(acq.InvoiceTotal));
        SearchAndReplace(body, "<Commission>", DisplayAmount(acq.Commission));
        SearchAndReplace(body, "<DeedDate>", DisplayDate(acq.DeedDate));
        SearchAndReplace(body, "<ClosingLetterDate>", DisplayDate(acq.ClosingLetterDate));
        SearchAndReplace(body, "<LienAmount>", DisplayAmount(acq.LienAmount));

        // Additional Acquisition Placeholders (missing from original)
        SearchAndReplace(body, "<AcqEffDtOrNow>", DisplayDate(acq.EffectiveDate ?? DateTime.Now));
        SearchAndReplace(body, "<AcqEffDtOrNow:Long>", DisplayDateLong(acq.EffectiveDate ?? DateTime.Now));
        SearchAndReplace(body, "<EffectiveDate:Long>", DisplayDateLong(acq.EffectiveDate));
        SearchAndReplace(body, "<DueDate:Long>", DisplayDateLong(acq.DueDate));
        SearchAndReplace(body, "<ClosingDate>", DisplayDate(acq.ClosingDate));
        SearchAndReplace(body, "<ClosingDate:Long>", DisplayDateLong(acq.ClosingDate));
        SearchAndReplace(body, "<PaidDate>", DisplayDate(acq.PaidDate));
        SearchAndReplace(body, "<PaidDate:Long>", DisplayDateLong(acq.PaidDate));
        SearchAndReplace(body, "<CoverSheetDate>", DisplayDate(DateTime.Now));
        SearchAndReplace(body, "<BorrowerName>", DisplayText(acq.BorrowerName));
        SearchAndReplace(body, "<BorrowerAddress>", DisplayText(acq.BorrowerAddress));
        SearchAndReplace(body, "<TotalBonusWords>", NumberToWordConverter.ConvertDollars(acq.TotalBonus ?? 0));
        SearchAndReplace(body, "<DraftFeeWords>", NumberToWordConverter.ConvertDollars(acq.DraftFee ?? 0));
    }

    private void MergeAcquisitionSellerData(AcquisitionSeller seller, Body body)
    {
        // For multiple sellers, this simplistic approach replaces ALL occurrences with the FIRST seller's data if called in loop.
        // However, standard mail merge usually implies one seller context or uses List fields.
        // Legacy "MergeAcquisitionSellerData" took sellerID, implying single seller target.
        // We will assume the logic handles current iteration or assumes 1-1 mapping for documents generated per seller.
        // But for "Acquisition Cover Sheet" (many sellers), it might be different.
        // Implementation here merely provides the replacement capability.

        SearchAndReplace(body, "<SellerName>", seller.SellerName ?? "");
        SearchAndReplace(body, "<SellerLastName>", seller.SellerLastName ?? "");
        SearchAndReplace(body, "<SellerAddress>", ConstructAddress(seller.AddressLine1, seller.AddressLine2, seller.AddressLine3, seller.City, seller.StateCode, seller.ZipCode));
        SearchAndReplace(body, "<SellerCity>", seller.City ?? "");
        SearchAndReplace(body, "<SellerState>", seller.StateCode ?? "");
        SearchAndReplace(body, "<SellerZip>", seller.ZipCode ?? "");
        SearchAndReplace(body, "<SellerContactEmail>", seller.ContactEmail ?? "");
        SearchAndReplace(body, "<SellerContactPhone>", seller.ContactPhone ?? "");
        SearchAndReplace(body, "<SellerContactFax>", seller.ContactFax ?? "");

        // Additional Seller Placeholders (missing from original)
        SearchAndReplace(body, "<SellerAddressBlock>", ConstructAddressBlock(seller.SellerName, seller.AddressLine1, seller.AddressLine2, seller.AddressLine3, seller.City, seller.StateCode, seller.ZipCode));
        SearchAndReplace(body, "<SellerAddressBlockCSZ>", $"{seller.City}, {seller.StateCode} {seller.ZipCode}");
        SearchAndReplace(body, "<SellerForeignAddress>", seller.ForeignAddress ?? "");
        SearchAndReplace(body, "<SellerDeceasedName>", seller.DeceasedName ?? "");
        SearchAndReplace(body, "<SellerSpouseName>", seller.SpouseName ?? "");
        SearchAndReplace(body, "<SellerSSN>", seller.SSN ?? "");
        SearchAndReplace(body, "<SellerSSNMask>", MaskSSN(seller.SSN));
        SearchAndReplace(body, "<SellerMaritalStatus>", seller.MaritalStatus ?? "");
        SearchAndReplace(body, "<SellerOwnershipType>", seller.OwnershipType ?? "");
        SearchAndReplace(body, "<SellerVestingName>", seller.VestingName ?? seller.SellerName ?? "");
    }

    private void MergeBuyerData(AcquisitionBuyer acqBuyer, string? templateID, Body body)
    {
        var buyer = acqBuyer.Buyer;
        if (buyer == null) return;

        // Contact logic
        string contactName = "", contactEmail = "";
        // We need BuyerContacts. 
        // Assuming loaded.
        if (templateID != null && buyer.BuyerContacts != null)
        {
            // Looking for int DocumentTemplateID parsing from string?
            if (int.TryParse(templateID, out int tid))
            {
                var contact = buyer.BuyerContacts.FirstOrDefault(c => c.DocumentTemplateID == tid);
                if (contact != null)
                {
                    contactName = contact.ContactName ?? "";
                    contactEmail = contact.ContactEmail ?? "";
                    // Legacy BuyerContact had limited fields, unlike County/Operator. 
                    // We added Address fields to Entity but legacy might not have used them or used Buyer's address.
                }
            }
        }

        SearchAndReplace(body, "<BuyerName>", buyer.BuyerName);
        SearchAndReplace(body, "<BuyerAddress>", ConstructAddress(buyer.AddressLine1, buyer.AddressLine2, buyer.AddressLine3, buyer.City, buyer.StateCode, buyer.ZipCode));
        SearchAndReplace(body, "<BuyerContactName>", contactName);
        SearchAndReplace(body, "<BuyerContactEmail>", contactEmail);
    }

    private void MergeSingleCountyData(AcquisitionCounty acqCounty, string? templateID, Body body, int acqID)
    {
        var county = acqCounty.County;
        if (county == null) return;

        string contactName = county.ContactName ?? "";
        string attention = "";
        string address = ConstructAddress(county.AddressLine1, county.AddressLine2, null, county.City, county.StateCode, county.ZipCode);
        string phone = county.ContactPhone ?? "";
        string fax = county.ContactFax ?? "";
        string email = county.ContactEmail ?? "";

        if (templateID != null && int.TryParse(templateID, out int tid) && county.CountyContacts != null)
        {
            var contact = county.CountyContacts.FirstOrDefault(c => c.DocumentTemplateID == tid);
            if (contact != null)
            {
                contactName = contact.ContactName ?? contactName;
                email = contact.ContactEmail ?? email;
                attention = contact.Attention ?? attention;
                phone = contact.ContactPhone ?? phone;
                fax = contact.ContactFax ?? fax;
                // Address override if contact has address
                if (!string.IsNullOrEmpty(contact.AddressLine1))
                    address = ConstructAddress(contact.AddressLine1, contact.AddressLine2, contact.AddressLine3, contact.City, contact.StateCode, contact.ZipCode);
            }
        }

        SearchAndReplace(body, "<CountyName>", county.CountyName);
        SearchAndReplace(body, "<CountyContactName>", contactName);
        SearchAndReplace(body, "<CountyAttn>", attention);
        SearchAndReplace(body, "<CountyAddress>", address);
        SearchAndReplace(body, "<CountyContactPhone>", phone);
        SearchAndReplace(body, "<CountyContactFax>", fax);
        SearchAndReplace(body, "<CountyContactEmail>", email);
        SearchAndReplace(body, "<RecordingFee>", DisplayAmount(county.RecordingFeeFirstPage));
        SearchAndReplace(body, "<CourtFee>", DisplayAmount(county.CourtFee));

        SearchAndReplace(body, "<CountyReturnedDate>", DisplayDate(acqCounty.DeedReturnedDate));
        SearchAndReplace(body, "<CountySentDate>", DisplayDate(acqCounty.DeedSentDate));
        SearchAndReplace(body, "<Book>", acqCounty.RecordingBook ?? "");
        SearchAndReplace(body, "<Page>", acqCounty.RecordingPage ?? "");

        // Generates lists for this county (Operators in this county)
        // Legacy: <OperUnitList>, <UnitOperList>
        // We need to implement string generation from AcquisitionUnits related to this County.
        // Doing simplified version:
        // Needs Acquisition -> AcquisitionUnits -> AcqUnitCounties -> filtered by CountyID

        // Merge CAG (County Appraisal Group) data if available
        MergeCountyAppraisalGroupData(county, body);
    }

    /// <summary>
    /// Merges County Appraisal Group (CAG) placeholders.
    /// Uses the most recent effective appraisal group for the county.
    /// </summary>
    private void MergeCountyAppraisalGroupData(County county, Body body)
    {
        // Get the most current effective appraisal group for this county
        var currentCag = county.CountyAppraisalGroups
            .Where(cag => cag.EffectiveDate <= DateTime.Now)
            .OrderByDescending(cag => cag.EffectiveDate)
            .FirstOrDefault();

        if (currentCag?.AppraisalGroup == null)
        {
            // Clear CAG placeholders if no appraisal group exists
            SearchAndReplace(body, "<CAG>", "");
            SearchAndReplace(body, "<CAGName>", "");
            SearchAndReplace(body, "<CAGContactName>", "");
            SearchAndReplace(body, "<CAGAttn>", "");
            SearchAndReplace(body, "<CAGContactEmail>", "");
            SearchAndReplace(body, "<CAGContactPhone>", "");
            SearchAndReplace(body, "<CAGContactFax>", "");
            SearchAndReplace(body, "<CAGAddress>", "");
            SearchAndReplace(body, "<CAGState>", "");
            SearchAndReplace(body, "<CAGStateName>", "");
            SearchAndReplace(body, "<CAGStateCode>", "");
            return;
        }

        var ag = currentCag.AppraisalGroup;

        // Build CAG address block
        var agAddress = new StringBuilder();
        if (!string.IsNullOrEmpty(ag.AddressLine1))
            agAddress.AppendLine(ag.AddressLine1);
        if (!string.IsNullOrEmpty(ag.AddressLine2))
            agAddress.AppendLine(ag.AddressLine2);
        if (!string.IsNullOrEmpty(ag.AddressLine3))
            agAddress.AppendLine(ag.AddressLine3);

        // City, State Zip line
        var cityStateZip = new StringBuilder();
        if (!string.IsNullOrEmpty(ag.City))
            cityStateZip.Append(ag.City);
        if (!string.IsNullOrEmpty(ag.StateCode))
        {
            if (cityStateZip.Length > 0) cityStateZip.Append(", ");
            cityStateZip.Append(ag.StateCode);
        }
        if (!string.IsNullOrEmpty(ag.ZipCode))
        {
            if (cityStateZip.Length > 0) cityStateZip.Append("   ");
            cityStateZip.Append(ag.ZipCode);
        }
        agAddress.Append(cityStateZip);

        // Build full CAG block (Name, Attention, Address)
        var cagBlock = new StringBuilder();
        cagBlock.AppendLine(ag.AppraisalGroupName);
        if (!string.IsNullOrEmpty(ag.Attention))
            cagBlock.AppendLine(ag.Attention);
        cagBlock.Append(agAddress);

        // Get state name from county's state relationship
        string stateName = county.State?.StateName ?? ag.StateCode ?? "";

        // Replace CAG placeholders
        SearchAndReplace(body, "<CAG>", cagBlock.ToString());
        SearchAndReplace(body, "<CAGName>", ag.AppraisalGroupName);
        SearchAndReplace(body, "<CAGContactName>", DisplayText(ag.ContactName));
        SearchAndReplace(body, "<CAGAttn>", DisplayText(ag.Attention));
        SearchAndReplace(body, "<CAGContactEmail>", DisplayText(ag.ContactEmail));
        SearchAndReplace(body, "<CAGContactPhone>", DisplayText(ag.ContactPhone));
        SearchAndReplace(body, "<CAGContactFax>", DisplayText(ag.ContactFax));
        SearchAndReplace(body, "<CAGAddress>", agAddress.ToString());
        SearchAndReplace(body, "<CAGState>", DisplayText(stateName));
        SearchAndReplace(body, "<CAGStateName>", DisplayText(stateName));
        SearchAndReplace(body, "<CAGStateCode>", DisplayText(ag.StateCode));
    }

    private void MergeSingleOperatorData(AcquisitionOperator acqOper, string? templateID, Body body, int acqID)
    {
        var oper = acqOper.Operator;
        if (oper == null) return;

        string contactName = oper.ContactName ?? "";
        string attention = "";
        string address = ConstructAddress(oper.AddressLine1, oper.AddressLine2, null, oper.City, oper.StateCode, oper.ZipCode);
        // ... Similar logic for OperatorContacts

        if (templateID != null && int.TryParse(templateID, out int tid) && oper.OperatorContacts != null)
        {
            var contact = oper.OperatorContacts.FirstOrDefault(c => c.DocumentTemplateID == tid);
            if (contact != null)
            {
                contactName = contact.ContactName ?? contactName;
                attention = contact.Attention ?? attention;
                if (!string.IsNullOrEmpty(contact.AddressLine1))
                    address = ConstructAddress(contact.AddressLine1, contact.AddressLine2, contact.AddressLine3, contact.City, contact.StateCode, contact.ZipCode);
            }
        }

        SearchAndReplace(body, "<OperatorName>", oper.OperatorName);
        SearchAndReplace(body, "<OperatorContactName>", contactName);
        SearchAndReplace(body, "<OperatorAttn>", attention);
        SearchAndReplace(body, "<OperatorAddress>", address);

        SearchAndReplace(body, "<OperNotifyNoRecDt>", DisplayDate(acqOper.NotifiedDateNoRec));
        SearchAndReplace(body, "<OperNotifyRecDt>", DisplayDate(acqOper.NotifiedDateRec));
        SearchAndReplace(body, "<OperDOReceivedDt>", DisplayDate(acqOper.DOReceivedDate));
    }

    // Lists
    private void MergeAcquisitionUnitData(Acquisition acq, Body body)
    {
        if (acq.AcquisitionUnits == null || !acq.AcquisitionUnits.Any()) return;

        var units = acq.AcquisitionUnits.ToList();
        var unitNames = units
            .Select(u => u.UnitName)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        SearchAndReplace(body, "<UnitList>", string.Join(", ", unitNames));
        SearchAndReplace(body, "<UnitListVertical>", string.Join("\v", unitNames)); // \v for manual line break

        var legals = units
            .Select(u => u.LegalDescription)
            .Where(l => !string.IsNullOrEmpty(l))
            .Distinct()
            .ToList();

        SearchAndReplace(body, "<UnitLegalDescription>", string.Join("; ", legals));
        SearchAndReplace(body, "<UnitLegalDescriptionList>", string.Join("\v", legals));

        // Single unit placeholders (for first unit if multiple)
        var firstUnit = units.FirstOrDefault();
        if (firstUnit != null)
        {
            SearchAndReplace(body, "<UnitName>", firstUnit.UnitName ?? "");
            SearchAndReplace(body, "<UnitInterest>", DisplayPercent(firstUnit.OwnershipInterest));
            SearchAndReplace(body, "<UnitTypeCode>", firstUnit.UnitTypeCode ?? "");
            SearchAndReplace(body, "<UnRecDt>", DisplayDate(firstUnit.RecordingDate));
            SearchAndReplace(body, "<UnVol#>", firstUnit.VolumeNumber ?? "");
            SearchAndReplace(body, "<UnPage#>", firstUnit.PageNumber ?? "");
            SearchAndReplace(body, "<GrossAcres>", DisplayAcres(firstUnit.GrossAcres));
            SearchAndReplace(body, "<NetAcres>", DisplayAcres(firstUnit.NetAcres));
            SearchAndReplace(body, "<Surveys>", firstUnit.Surveys ?? "");
            SearchAndReplace(body, "<UnitNMPI>", DisplayDecimal(firstUnit.NMPI));
            SearchAndReplace(body, "<UnitDecimalsText>", DisplayDecimal(firstUnit.Decimals));
        }

        // County/State lists from units
        var countyStates = units
            .SelectMany(u => u.AcqUnitCounties.Where(c => c.County != null))
            .Select(uc => $"{uc.County!.CountyName}, {uc.County.StateCode}")
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        SearchAndReplace(body, "<UnitCountyStateList>", string.Join(", ", countyStates));
        SearchAndReplace(body, "<UnitCountyStateListTbl>", string.Join("\v", countyStates));

        // Interest type names
        var interestTypes = units
            .Where(u => u.UnitType != null)
            .Select(u => u.UnitType!.UnitTypeDesc)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .ToList();
        SearchAndReplace(body, "<UnitInterestTypeNameList>", string.Join(", ", interestTypes));
    }

    private void MergeAcquisitionCountyData(Acquisition acq, Body body)
    {
        if (acq.AcquisitionCounties == null || !acq.AcquisitionCounties.Any()) return;

        var countyNames = acq.AcquisitionCounties
            .Select(c => c.County?.CountyName)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        SearchAndReplace(body, "<CountyList>", string.Join(", ", countyNames));
        SearchAndReplace(body, "<CountyListVertical>", string.Join("\v", countyNames));
    }

    private void MergeAcquisitionOperatorData(Acquisition acq, Body body)
    {
        if (acq.AcquisitionOperators == null || !acq.AcquisitionOperators.Any()) return;

        var operatorNames = acq.AcquisitionOperators
            .Select(o => o.Operator?.OperatorName)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        SearchAndReplace(body, "<OperatorList>", string.Join(", ", operatorNames));
        SearchAndReplace(body, "<OperatorListVertical>", string.Join("\v", operatorNames));
    }

    private void MergeContextData(DocumentGenerationContext context, Body body)
    {
        // Handle barcode-specific placeholders
        if (!string.IsNullOrEmpty(context.DocumentTypeCode))
        {
            SearchAndReplace(body, "<DocumentType>", context.DocumentTypeCode);
        }

        if (!string.IsNullOrEmpty(context.DocumentDescription))
        {
            SearchAndReplace(body, "<DocumentDescription>", context.DocumentDescription);
        }

        // Handle other context-specific placeholders as needed
        if (!string.IsNullOrEmpty(context.StateCode))
        {
            SearchAndReplace(body, "<StateCode>", context.StateCode);
        }
    }

    // --- Helpers ---
    private string ConstructAddress(string? l1, string? l2, string? l3, string? city, string? state, string? zip)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(l1)) sb.AppendLine(l1);
        if (!string.IsNullOrEmpty(l2)) sb.AppendLine(l2);
        if (!string.IsNullOrEmpty(l3)) sb.AppendLine(l3);
        sb.Append($"{city}, {state} {zip}");
        return sb.ToString();
    }

    private string ConstructAddressBlock(string? name, string? l1, string? l2, string? l3, string? city, string? state, string? zip)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(name)) sb.AppendLine(name);
        if (!string.IsNullOrEmpty(l1)) sb.AppendLine(l1);
        if (!string.IsNullOrEmpty(l2)) sb.AppendLine(l2);
        if (!string.IsNullOrEmpty(l3)) sb.AppendLine(l3);
        sb.Append($"{city}, {state} {zip}");
        return sb.ToString();
    }

    private static string MaskSSN(string? ssn)
    {
        if (string.IsNullOrEmpty(ssn) || ssn.Length < 4) return "";
        // Return only last 4 digits with XXX-XX- prefix
        return $"XXX-XX-{ssn[^4..]}";
    }

    private string DisplayDate(DateTime? dt) => dt.HasValue ? dt.Value.ToString("MM/dd/yyyy") : "";
    private string DisplayDate(DateTime dt) => dt.ToString("MM/dd/yyyy");
    private string DisplayDateLong(DateTime? dt) => dt.HasValue ? dt.Value.ToString("MMMM d, yyyy") : "";
    private string DisplayDateLong(DateTime dt) => dt.ToString("MMMM d, yyyy");
    private string DisplayDateWithOrdinal(DateTime? dt)
    {
        if (!dt.HasValue) return "";
        var d = dt.Value;
        var suffix = GetOrdinalSuffix(d.Day);
        return $"{d:MMMM} {d.Day}{suffix}, {d.Year}";
    }
    private static string GetOrdinalSuffix(int day) => (day % 10, day / 10 % 10) switch
    {
        (1, not 1) => "st",
        (2, not 1) => "nd",
        (3, not 1) => "rd",
        _ => "th"
    };
    private string DisplayAmount(decimal? amt) => amt.HasValue ? amt.Value.ToString("##,##0.00") : "";
    private string DisplayPercent(decimal? pct) => pct.HasValue ? pct.Value.ToString("0.########") : "";
    private string DisplayAcres(decimal? acres) => acres.HasValue ? acres.Value.ToString("0.00") : "";
    private string DisplayDecimal(decimal? dec) => dec.HasValue ? dec.Value.ToString("G") : "";
    private string DisplayText(string? txt) => txt ?? "";

    /// <summary>
    /// Performs case-insensitive string replacement for all occurrences
    /// </summary>
    private string ReplaceIgnoreCase(string source, string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(oldValue))
            return source;

        var result = source;
        var index = 0;

        while ((index = result.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            result = result.Remove(index, oldValue.Length).Insert(index, newValue);
            index += newValue.Length; // Move past the replacement
        }

        return result;
    }

    // Robust Search and Replace
    private void SearchAndReplace(Body body, string placeholder, string replacement)
    {
        if (replacement == null) replacement = "";

        // Find all paragraphs containing the placeholder text (spanning runs)
        foreach (var paragraph in body.Descendants<Paragraph>())
        {
            ReplaceTextInParagraph(paragraph, placeholder, replacement);
        }
    }

    private void ReplaceTextInParagraph(Paragraph paragraph, string placeholder, string replacement)
    {
        var text = paragraph.InnerText;
        if (!text.Contains(placeholder, StringComparison.OrdinalIgnoreCase)) return;

        // The placeholder exists in this paragraph.
        // To properly replace while respecting runs, we need to map the Runs.
        // Simplest Robust Strategy: 
        // 1. Identify start and end indices of the placeholder in the inner text.
        // 2. Iterate runs to find which ones cover these indices.
        // 3. Modfy those runs.

        // Note: multiple occurrences?
        // We loop until no occurrences.

        while (paragraph.InnerText.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
        {
            // Re-calculate text and indices
            string docText = paragraph.InnerText;
            int index = docText.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase);
            if (index < 0) break;

            var runs = paragraph.Descendants<Run>().ToList();
            int currentPos = 0;

            Run? startRun = null;
            Run? endRun = null;
            int startRunOffset = 0;
            // int endRunOffset = 0;

            // Find Runs involved
            foreach (var run in runs)
            {
                string runText = run.InnerText;
                int runLen = runText.Length;

                if (startRun == null && currentPos + runLen > index)
                {
                    startRun = run;
                    startRunOffset = index - currentPos;
                }

                if (startRun != null && currentPos + runLen >= index + placeholder.Length)
                {
                    endRun = run;
                    // endRunOffset = (index + placeholder.Length) - currentPos;
                    break;
                }

                currentPos += runLen;
            }

            if (startRun != null && endRun != null)
            {
                // Create replacement structure
                // If startRun == endRun, simple replace
                if (startRun == endRun)
                {
                    var textElem = startRun.GetFirstChild<Text>();
                    if (textElem != null)
                        textElem.Text = ReplaceIgnoreCase(textElem.Text, placeholder, replacement);
                }
                else
                {
                    // Split across runs.
                    // 1. Truncate startRun text after offset
                    // 2. Truncate endRun text before offset end
                    // 3. Remove intermediate runs
                    // 4. Append replacement to startRun (safe) or insert new run

                    // Helper to get text element (Run might have other children)
                    var startText = startRun.GetFirstChild<Text>();
                    var endText = endRun.GetFirstChild<Text>();

                    if (startText != null)
                    {
                        startText.Text = startText.Text.Substring(0, startRunOffset) + replacement;
                    }

                    // Clear intermediate runs
                    bool delete = false;
                    foreach (var r in runs)
                    {
                        if (r == startRun) { delete = true; continue; }
                        if (r == endRun) { delete = false; break; }
                        if (delete) r.Remove();
                    }

                    // Fix end run: Remove the part that was part of placeholder
                    // Calculate local index in endRun
                    // We need exact mapping. 
                    // Using the "Flatten" approach logic for the specific matched runs is safer? 
                    // Actually, if we just remove the text from endRun corresponding to remainder of placeholder:

                    // Remainder length:
                    // Total placeholder length - (length taken from start run) - (length of intermediate runs)
                    // But we modified startRun already.

                    // Re-approach:
                    // Just empty the text of intermediate runs and endRun's used part.
                    // Then append replacement to startRun.

                    // However, endRun might contain text AFTER key.
                    int endRunStartIndex = (index + placeholder.Length) - (currentPos); // relative to endRun start? No.
                                                                                        // We need to track positions better.

                    // Fallback to "Dumb Flattening" for the involved runs:
                    // Consolidate all text from Start to End run into StartRun, then replace, then remove others.
                    // This preserves StartRun formatting.

                }

                // Fallback implementation for split helper:
                // Since complex logic is error prone without unit tests,
                // I will use a simplified "Replace Text Node" approach if contained,
                // and "Flatten Paragraph" if split.

                // If we found it wasn't simple (startRun != endRun), we FLATTEN the paragraph for safety.
                if (startRun != endRun)
                {
                    FlattenParagraph(paragraph);
                    // Loop will continue and next time it should be in one run (mostly).
                }
            }
        }
    }

    private void FlattenParagraph(Paragraph p)
    {
        // Consolidate all text into the first run.
        var runs = p.Descendants<Run>().ToList();
        if (runs.Count <= 1) return;

        var firstRun = runs.First();
        var sb = new StringBuilder();
        foreach (var r in runs) sb.Append(r.InnerText);

        var text = firstRun.GetFirstChild<Text>();
        if (text == null)
        {
            text = new Text();
            firstRun.AppendChild(text);
        }
        text.Text = sb.ToString();

        // Remove other runs
        for (int i = 1; i < runs.Count; i++) runs[i].Remove();
    }
}
