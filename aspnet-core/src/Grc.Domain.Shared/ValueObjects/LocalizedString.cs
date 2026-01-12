using System;
namespace Grc.ValueObjects;

/// <summary>
/// Bilingual string value object for Arabic and English localization
/// </summary>
public class LocalizedString
{
    public string En { get; set; }
    public string Ar { get; set; }
    
    public LocalizedString() { }
    
    public LocalizedString(string en, string ar)
    {
        En = en;
        Ar = ar;
    }
    
    /// <summary>
    /// Gets the localized string based on culture code
    /// </summary>
    /// <param name="culture">Culture code (e.g., "en", "ar")</param>
    /// <returns>Localized string</returns>
    public string Get(string culture) => culture == "ar" ? Ar : En;
    
    /// <summary>
    /// Gets the localized string based on current culture
    /// </summary>
    /// <returns>Localized string</returns>
    public string GetCurrent()
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture.Name;
        return Get(culture.StartsWith("ar") ? "ar" : "en");
    }
    
    /// <summary>
    /// Checks if the localized string is empty
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(En) && string.IsNullOrWhiteSpace(Ar);
}

