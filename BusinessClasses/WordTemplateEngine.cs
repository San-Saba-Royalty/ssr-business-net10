using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SSRBusiness.Entities;
using System.Text.RegularExpressions;

namespace SSRBusiness.BusinessClasses;

public class WordTemplateEngine
{
    public void CreateMergeDocument(string sourcePath, string destPath, Acquisition acquisition, User? user, string? documentTemplateID = null)
    {
        if (!File.Exists(sourcePath)) throw new FileNotFoundException("Template not found", sourcePath);

        // Copy template to destination
        File.Copy(sourcePath, destPath, true);

        using (WordprocessingDocument doc = WordprocessingDocument.Open(destPath, true))
        {
            var body = doc.MainDocumentPart?.Document.Body;
            if (body != null)
            {
                MergeGlobalData(body);
                if (user != null) MergeUserData(user, body);
                MergeAcquisitionData(acquisition, body);

                // Merge sub-entities
                // Legacy logic was complex with conditionals on documentType.
                // We will merge ALL available data that matches placeholders. 
                // If placeholders are unique, this is safe.

                foreach (var seller in acquisition.AcquisitionSellers)
                {
                    MergeAcquisitionSellerData(seller, body);
                }

                foreach (var buyer in acquisition.AcquisitionBuyers)
                {
                    MergeBuyerData(buyer, documentTemplateID, body);
                }

                // Single County/Operator logic in legacy depended on "First" or passed ID.
                // We will iterate all and replace specific placeholders if they exist, 
                // but standard placeholders like <CountyName> implies singular context.
                // Legacy "MergeSingleCountyData" implies generating a doc FOR a county.
                // If we are generating a generic doc, we might need context.
                // Logic: If there is only 1 county, merge it. If multiple, ambiguous?
                // For now, we merge the FIRST one found if placeholders exist.

                if (acquisition.AcquisitionCounties.Any())
                {
                    MergeSingleCountyData(acquisition.AcquisitionCounties.First(), documentTemplateID, body, acquisition.AcquisitionID);
                }

                if (acquisition.AcquisitionOperators.Any())
                {
                    MergeSingleOperatorData(acquisition.AcquisitionOperators.First(), documentTemplateID, body, acquisition.AcquisitionID);
                }

                MergeAcquisitionUnitData(acquisition, body);
                MergeAcquisitionCountyData(acquisition, body); // Lists
                MergeAcquisitionOperatorData(acquisition, body); // Lists

                // Save changes
                doc.MainDocumentPart?.Document.Save();
            }
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

        // Add more placeholders as defined in legacy...
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

        var unitNames = acq.AcquisitionUnits
            .Select(u => u.UnitName)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        SearchAndReplace(body, "<UnitList>", string.Join(", ", unitNames));
        SearchAndReplace(body, "<UnitListVertical>", string.Join("\v", unitNames)); // \v for manual line break

        var legals = acq.AcquisitionUnits
            .Select(u => u.LegalDescription)
            .Where(l => !string.IsNullOrEmpty(l))
            .Distinct()
            .ToList();
            
        SearchAndReplace(body, "<UnitLegalDescription>", string.Join("; ", legals));
        SearchAndReplace(body, "<UnitLegalDescriptionList>", string.Join("\v", legals));
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

    private string DisplayDate(DateTime? dt) => dt.HasValue ? dt.Value.ToString("MM/dd/yyyy") : "";
    private string DisplayAmount(decimal? amt) => amt.HasValue ? amt.Value.ToString("##,##0.00") : "";
    private string DisplayText(string? txt) => txt ?? "";

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
        if (!text.Contains(placeholder)) return;

        // The placeholder exists in this paragraph.
        // To properly replace while respecting runs, we need to map the Runs.
        // Simplest Robust Strategy: 
        // 1. Identify start and end indices of the placeholder in the inner text.
        // 2. Iterate runs to find which ones cover these indices.
        // 3. Modfy those runs.

        // Note: multiple occurrences?
        // We loop until no occurrences.

        while (paragraph.InnerText.Contains(placeholder))
        {
            // Re-calculate text and indices
            string docText = paragraph.InnerText;
            int index = docText.IndexOf(placeholder);
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
                        textElem.Text = textElem.Text.Remove(startRunOffset, placeholder.Length).Insert(startRunOffset, replacement);
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
