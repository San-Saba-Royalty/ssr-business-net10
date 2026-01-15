using System;
using System.Collections.Generic;

namespace SSRBusiness.Entities;

public class CadData
{
    public string TableDisplayName { get; set; } = string.Empty;
    public string CountyNumber { get; set; } = string.Empty;
    public List<string> CountyNumberList { get; set; } = new List<string>();
    public string RrcLease { get; set; } = string.Empty;
    public string Lease { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Addr1 { get; set; } = string.Empty;
    public string Addr2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string St { get; set; } = string.Empty;
    public string Zip5 { get; set; } = string.Empty;
    public string Zip4 { get; set; } = string.Empty;
    public string LeaseName { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Intyp { get; set; } = string.Empty;
    public string Interest { get; set; } = string.Empty;
    public double? InterestValue { get; set; }
    public string Abstract { get; set; } = string.Empty;
    public string Survey { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Acres { get; set; } = string.Empty;

    // Helper properties for display if needed
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Addr1)) parts.Add(Addr1);
            if (!string.IsNullOrWhiteSpace(Addr2)) parts.Add(Addr2);
            if (!string.IsNullOrWhiteSpace(City)) parts.Add(City);
            if (!string.IsNullOrWhiteSpace(St)) parts.Add(St);
            if (!string.IsNullOrWhiteSpace(Zip5)) parts.Add(Zip5);
            return string.Join(", ", parts);
        }
    }
}
