using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using SSRBusiness.Entities;
using SSRBusiness.Interfaces;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Criteria for generating a Letter Agreement document.
/// Contains all template paths and generation parameters.
/// </summary>
public class LetterAgreementCriteria
{
    /// <summary>User ID performing the generation</summary>
    public int UserId { get; set; }

    /// <summary>Letter Agreement ID to generate document for</summary>
    public int LetterAgreementId { get; set; }

    /// <summary>Selected signing partner ID</summary>
    public int SigningPartnerId { get; set; }

    /// <summary>Custom field replacements</summary>
    public Dictionary<string, string> CustomFields { get; set; } = new();

    /// <summary>Destination document path (output)</summary>
    public string DestinationDocument { get; set; } = string.Empty;

    // Template source paths (retrieved from Azure File Share)
    /// <summary>Main letter agreement template</summary>
    public string LetterAgreementSource { get; set; } = string.Empty;

    /// <summary>Signature block for individual signers</summary>
    public string LetterAgreementSignatureIndividualSource { get; set; } = string.Empty;

    /// <summary>Signature block for company signers</summary>
    public string LetterAgreementSignatureCompanySource { get; set; } = string.Empty;

    /// <summary>Conveyance Exhibit A template</summary>
    public string ConveyanceExhibitASource { get; set; } = string.Empty;

    /// <summary>Conveyance template for producing properties</summary>
    public string ConveyanceProducingSource { get; set; } = string.Empty;

    /// <summary>Conveyance template for non-producing properties</summary>
    public string ConveyanceNonProducingSource { get; set; } = string.Empty;

    /// <summary>Conveyance signature for individual</summary>
    public string ConveyanceSignatureIndividualSource { get; set; } = string.Empty;

    /// <summary>Conveyance signature for company</summary>
    public string ConveyanceSignatureCompanySource { get; set; } = string.Empty;

    /// <summary>Exhibit A main template</summary>
    public string ExhibitASource { get; set; } = string.Empty;

    /// <summary>Exhibit A producing section</summary>
    public string ExhibitAProducingSource { get; set; } = string.Empty;

    /// <summary>Exhibit A non-producing section</summary>
    public string ExhibitANonProducingSource { get; set; } = string.Empty;

    /// <summary>State code for template selection (TX, LA, etc.)</summary>
    public string StateCode { get; set; } = "TX";
}
