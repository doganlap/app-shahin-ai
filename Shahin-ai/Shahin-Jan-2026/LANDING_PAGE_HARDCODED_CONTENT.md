# Landing Page Hardcoded Content Report

**File**: `src/GrcMvc/Views/Landing/Index.cshtml`  
**Date**: 2025-01-22  
**Status**: 🚨 **Found 15+ Hardcoded Items**

---

## 🚨 Critical Hardcoded Content

### 1. Framework Tags (Lines 538-548) - **NOT LOCALIZED**

**Location**: Lines 538-548  
**Issue**: Framework names are hardcoded in English, not using localization

```razor
<!-- CURRENT (WRONG) -->
<span class="framework-tag">NCA ECC-1:2018</span>
<span class="framework-tag">NCA CSCC-1:2019</span>
<span class="framework-tag">NCA DCC-1:2022</span>
<span class="framework-tag">SAMA Cybersecurity Framework</span>
<span class="framework-tag">PDPL Compliance</span>
<span class="framework-tag">ISO 27001:2022</span>
<span class="framework-tag">ISO 27701:2019</span>
<span class="framework-tag">NIST CSF 2.0</span>
<span class="framework-tag">PCI DSS 4.0</span>
<span class="framework-tag">SOC 2 Type II</span>
```

**Fix Required**:
- Use localization keys: `@L["Landing_Framework_NCA_ECC"]`
- OR load dynamically from database
- OR use a loop with localization

---

### 2. Challenge Statistics (Lines 103, 116, 129, 142) - **HARDCODED PERCENTAGES**

**Location**: Challenge Cards  
**Issue**: Statistics are hardcoded as strings

```razor
<!-- Line 103 -->
<div class="challenge-stat-value">73%</div>

<!-- Line 116 -->
<div class="challenge-stat-value">40%</div>

<!-- Line 129 -->
<div class="challenge-stat-value">5M+</div>

<!-- Line 142 -->
<div class="challenge-stat-value">68%</div>
```

**Fix Required**:
- Use model data: `@Model.ChallengeStats.DataFragmentationPercent`
- OR use localization: `@L["Landing_Challenge_DataFragmentation_Stat"]` = "73%"

---

### 3. AI Agents Count (Line 77) - **HARDCODED NUMBER**

**Location**: Line 77  
**Issue**: AI agents count is hardcoded

```razor
<!-- Line 77 -->
<div class="stat-value-animated counter-animated" data-target="9">0</div>
```

**Fix Required**:
- Use model: `data-target="@Model.Stats.AIAgents"`
- OR load from backend configuration

---

### 4. Regulator Names (Lines 506, 511, 516, 521, 526, 531) - **PARTIALLY HARDCODED**

**Location**: Regulator Cards  
**Issue**: English names hardcoded, Arabic uses localization (good)

```razor
<!-- Line 506 -->
<div class="regulator-name">NCA</div>  <!-- HARDCODED -->
<div class="regulator-name-ar">@L["Landing_Index_Regulator_NCA"]</div>  <!-- ✅ LOCALIZED -->

<!-- Line 511 -->
<div class="regulator-name">SAMA</div>  <!-- HARDCODED -->

<!-- Line 516 -->
<div class="regulator-name">CMA</div>  <!-- HARDCODED -->

<!-- Line 521 -->
<div class="regulator-name">CITC</div>  <!-- HARDCODED -->

<!-- Line 526 -->
<div class="regulator-name">SDAIA</div>  <!-- HARDCODED -->

<!-- Line 531 -->
<div class="regulator-name">PDPL</div>  <!-- HARDCODED -->
```

**Fix Required**:
- Use localization for English too: `@L["Landing_Index_Regulator_NCA_EN"]`
- OR load from database

---

## ✅ Already Localized (Good)

Most content is properly localized:
- ✅ Hero section (Lines 16-36)
- ✅ Stats labels (Lines 54, 62, 70, 78)
- ✅ Challenges section titles (Lines 100, 113, 126, 139)
- ✅ Features section (Lines 171-283)
- ✅ Differentiators (Lines 301-381)
- ✅ Timeline/How It Works (Lines 397-490)
- ✅ CTA section (Lines 570-607)

---

## 📋 Summary of Hardcoded Items

| Item | Location | Type | Severity | Fix Priority |
|------|----------|------|----------|--------------|
| Framework tags | Lines 538-548 | English text | 🔴 High | **URGENT** |
| Challenge stats | Lines 103, 116, 129, 142 | Percentages | 🟡 Medium | High |
| AI agents count | Line 77 | Number | 🟡 Medium | High |
| Regulator names (EN) | Lines 506, 511, 516, 521, 526, 531 | Abbreviations | 🟢 Low | Medium |

---

## 🔧 Recommended Fixes

### Fix 1: Framework Tags - Use Localization

**Add to `SharedResource.ar.resx`**:
```xml
<data name="Landing_Framework_NCA_ECC_2018" xml:space="preserve">
  <value>NCA ECC-1:2018</value>
</data>
<data name="Landing_Framework_NCA_CSCC_2019" xml:space="preserve">
  <value>NCA CSCC-1:2019</value>
</data>
<!-- ... more frameworks ... -->
```

**Update Index.cshtml**:
```razor
<span class="framework-tag">@L["Landing_Framework_NCA_ECC_2018"]</span>
<span class="framework-tag">@L["Landing_Framework_NCA_CSCC_2019"]</span>
<!-- ... etc ... -->
```

**OR Better: Load from Database Dynamically**:
```razor
@foreach (var framework in Model.PopularFrameworks.Take(10))
{
    <span class="framework-tag">@framework.Name</span>
}
```

---

### Fix 2: Challenge Statistics - Use Model Data

**Update LandingPageViewModel**:
```csharp
public class ChallengeStats
{
    public int DataFragmentationPercent { get; set; } = 73;
    public int TimeWastePercent { get; set; } = 40;
    public string ComplianceRisksValue { get; set; } = "5M+";
    public int SkillsGapPercent { get; set; } = 68;
}
```

**Update Index.cshtml**:
```razor
<div class="challenge-stat-value">@Model.ChallengeStats.DataFragmentationPercent%</div>
<div class="challenge-stat-value">@Model.ChallengeStats.TimeWastePercent%</div>
<div class="challenge-stat-value">@Model.ChallengeStats.ComplianceRisksValue</div>
<div class="challenge-stat-value">@Model.ChallengeStats.SkillsGapPercent%</div>
```

---

### Fix 3: AI Agents Count - Load from Config/Model

**Update Index.cshtml Line 77**:
```razor
<!-- CURRENT -->
<div class="stat-value-animated counter-animated" data-target="9">0</div>

<!-- FIXED -->
<div class="stat-value-animated counter-animated" data-target="@Model.Stats.AIAgents">0</div>
```

**Update LandingPageViewModel.Stats**:
```csharp
public int AIAgents { get; set; } = 9; // Or load from database
```

---

### Fix 4: Regulator Names - Add English Localization

**Add to `SharedResource.en.resx`**:
```xml
<data name="Landing_Index_Regulator_NCA_EN" xml:space="preserve">
  <value>NCA</value>
</data>
<data name="Landing_Index_Regulator_SAMA_EN" xml:space="preserve">
  <value>SAMA</value>
</data>
<!-- ... etc ... -->
```

**Update Index.cshtml**:
```razor
<div class="regulator-name">@L["Landing_Index_Regulator_NCA_EN"]</div>
<div class="regulator-name-ar">@L["Landing_Index_Regulator_NCA"]</div>
```

---

## 🎯 Priority Order

1. **URGENT**: Framework tags (Lines 538-548) - Not localized at all
2. **HIGH**: Challenge statistics - Should come from model/config
3. **MEDIUM**: AI agents count - Should be dynamic
4. **LOW**: Regulator English names - Already work, just not localized

---

## ✅ Verification Checklist

After fixes:
- [ ] All framework names use localization keys
- [ ] All statistics come from model/database
- [ ] All numbers are dynamic (not hardcoded)
- [ ] Both Arabic and English use localization
- [ ] Test with language switcher
- [ ] Verify on production build

---

**Last Updated**: 2025-01-22  
**Status**: Needs fixes  
**Estimated Fix Time**: 2-3 hours
