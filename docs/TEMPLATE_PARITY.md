# Template Processing Parity Report

## Executive Summary

This document compares template processing in **SSRBusiness.NET10** (OpenXML + DocSharp) against **MineralAcquisitionWeb** (Aspose.Words). The modern implementation provides **~25% feature parity** with critical gaps in Letter Agreement support, document composition, and advanced formatting.

---

## Architecture Comparison

| Aspect | Legacy (Aspose) | Modern (OpenXML) |
|--------|-----------------|------------------|
| **Library** | Aspose.Words (paid license) | OpenXML SDK + DocSharp.Binary |
| **Template Storage** | Local file system | Azure File Share |
| **Output Storage** | Local file system | Azure Blob Storage |
| **.doc Support** | Native Aspose support | DocSharp conversion to .docx |
| **Lines of Code** | ~7,000 (two engines) | ~550 (one engine) |

---

## Feature Parity Matrix

### ✅ Implemented Features (Parity Achieved)

| Feature | Legacy | Modern | Status |
|---------|--------|--------|--------|
| Basic Acquisition merge | ✅ | ✅ | **Parity** |
| Global data placeholders | ✅ | ✅ | **Parity** |
| User data placeholders | ✅ | ✅ | **Parity** |
| Acquisition data merge | ✅ | ✅ | **Parity** |
| Seller/Buyer/County/Operator/Unit merge | ✅ | ✅ | **Parity** |
| Custom fields | ✅ | ✅ | **Parity** |
| .doc to .docx conversion | ✅ | ✅ | **Parity** |
| Paragraph flattening (split-run fix) | N/A | ✅ | **Modern-only** |

### ❌ Missing Features (Gaps)

| Feature | Legacy | Modern | Priority |
|---------|--------|--------|----------|
| **Letter Agreement Documents** | ✅ | ❌ | 🔴 Critical |
| **Conveyance Documents** | ✅ | ❌ | 🔴 Critical |
| **Exhibit A Generation** | ✅ | ❌ | 🔴 Critical |
| **Multi-document composition** | ✅ | ❌ | 🔴 Critical |
| Sub-document insertion (IReplacingCallback) | ✅ | ❌ | 🟡 High |
| Multiple document copies | ✅ | ❌ | 🟡 High |
| Barcode parsing/generation | ✅ | ❌ | 🟡 High |
| Number-to-word conversion | ✅ | ❌ | 🟡 High |
| State-specific logic (LA handling) | ✅ | ❌ | 🟡 High |
| Check statement cover sheets | ✅ | ❌ | 🟢 Medium |
| Operator document merge | ✅ | ❌ | 🟢 Medium |
| County appraisal group (CAG) lists | ✅ | ❌ | 🟢 Medium |
| Table row formatting (Chr(7)) | ✅ | ❌ | 🟢 Medium |
| Signing partner merge | ✅ | ❌ | 🟢 Medium |

---

## Placeholder Coverage Analysis

### Acquisition Placeholders

| Category | Legacy Count | Modern Count | Gap |
|----------|--------------|--------------|-----|
| Global | 8 | 8 | 0 |
| User | 12 | 12 | 0 |
| Acquisition | 45 | 32 | **13** |
| Seller | 28 | 18 | **10** |
| Buyer | 15 | 10 | **5** |
| County | 18 | 12 | **6** |
| Operator | 22 | 8 | **14** |
| Unit | 18 | 10 | **8** |
| **Total** | **166** | **110** | **56** |

### Missing Critical Placeholders

```text
# Acquisition
<AcqEffDtOrNow>, <AcqEffDtOrNow:Long>, <BorrowerName>, <BorrowerAddress>
<CoverSheetDate>, <ClosingDate:Long>, <PaidDate>, <PaidDate:Long>

# Seller  
<SellerAddressBlock>, <SellerAddressBlockCSZ>, <SellerForeignAddress>
<SellerDeceasedName>, <SellerSpouseName>

# Operator
<OperNotifyNoRecDt>, <OperNotifyRecDt>, <OperDOReceivedDt>
<UnitCountyStateList>, <UnitCountyStateListTbl>, <UnitInterestTypeNameList>
<CAGList>, <CAGListTbl>

# Unit
<UnitName>, <UnitInterest>, <UnitTypeCode>, <UnRecDt>, <UnVol#>, <UnPage#>
<GrossAcres>, <NetAcres>, <Surveys>
```

---

## Wiring Analysis

### AzureFileShareFileService Integration

> [!IMPORTANT]
> `AzureFileShareFileService` is correctly registered in DI but the wiring needs verification.

**Current Registration** (`DependencyInjection.cs`):
```csharp
// Line 93 - AzureBlobFileService for document output
services.AddSingleton<IFileService, AzureBlobFileService>();

// Line 137 - AzureFileShareFileService for templates (scoped)
services.AddScoped<IFileService, AzureFileShareFileService>();
```

**Issue**: Dual registration may cause resolution conflicts. The last registration wins, so `AzureFileShareFileService` is currently used for `IFileService` injection.

**Verification Needed**:
- Confirm `CoverSheetService` receives correct implementation
- Ensure `DocumentController` uses File Share for templates and Blob for output

---

## Implementation Plan for 100% Parity

### Phase 1: Letter Agreement Engine (Estimated: 3-4 weeks)

1. **Create `LetterAgreementTemplateEngine.cs`**
   - Port `CreateLetterAgreement` method
   - Implement `MergeLetterAgreementLetter` 
   - Add signing partner merge logic
   - Support state-specific template selection (LA vs. other states)

2. **Implement Document Composition**
   - Create `DocumentComposer.cs` for multi-section documents
   - Port `InsertDocument` and `InsertDocumentWithOutFormatting` patterns
   - Handle section breaks and formatting preservation

3. **Add Conveyance/Exhibit Support**
   - `MergeConveyanceDocument` implementation
   - `CreateLetterAgreementExhibitA` implementation
   - County group handling logic

### Phase 2: Extended Acquisition Support (Estimated: 2 weeks)

1. **Complete Placeholder Set**
   - Add missing 56 placeholders to `WordTemplateEngine`
   - Implement `:Long` date format variants
   - Add table formatting (vertical lists with Chr(7))

2. **Multi-Document Features**
   - Barcode parsing via font-based approach or third-party library
   - Multiple copy generation (`CreateMultipleMergeDocument`)
   - Check statement cover sheets

### Phase 3: Helper Functions (Estimated: 1 week)

1. **Number Conversion**
   ```csharp
   public static string ConvertDecimalToWord(decimal? input)
   public static string ConvertIntegerToWord(int? input, string units)
   ```

2. **Formatting Helpers**
   - `DisplayPercent`, `DisplayAcres`, `DisplayInterest`
   - `GetCurrentDaySuffix` (ordinal day formatting)

### Phase 4: Service Wiring (Estimated: 3 days)

1. **Named Service Registration**
   ```csharp
   services.AddKeyedSingleton<IFileService, AzureBlobFileService>("blob");
   services.AddKeyedScoped<IFileService, AzureFileShareFileService>("fileshare");
   ```

2. **Controller Updates**
   - Inject appropriate service based on use case
   - Templates from File Share, outputs to Blob

---

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| OpenXML lacks IReplacingCallback equivalent | High | Use manual paragraph navigation + AltChunk for document insertion |
| Barcode font dependency | Medium | Use QRCoder library or maintain font-based approach |
| Complex table formatting | Medium | Test extensively with production templates |
| Performance at scale | Low | Implement connection pooling for Azure services |

---

## Recommendations

1. **Prioritize Letter Agreement** - This is the largest functional gap affecting business operations
2. **Incremental Migration** - Add features to `WordTemplateEngine` rather than parallel engines
3. **Test with Production Templates** - Legacy templates may have undocumented placeholder patterns
4. **Consider Hybrid Approach** - Evaluate if Aspose.Words for .NET could be retained for specific complex scenarios

---

## Appendix: Legacy Code References

| File | Lines | Purpose |
|------|-------|---------|
| `AsposeDocumentMergeEngine.vb` | 4,310 | Acquisition documents |
| `LetterAgreementDocumentMergeEngine.vb` | 2,689 | Letter agreements |
| `WordTemplateEngine.cs` | 552 | Modern acquisition only |
| `AzureFileShareFileService.cs` | 248 | Template retrieval |
| `CoverSheetService.cs` | 125 | Service orchestration |
