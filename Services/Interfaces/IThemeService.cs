using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for managing dynamic theme switching at runtime
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Apply a theme by name (loads JSON, applies to Application.Resources)
    /// </summary>
    Task ApplyThemeAsync(string themeName);

    /// <summary>
    /// Get list of available themes
    /// </summary>
    Task<List<string>> GetAvailableThemesAsync();

    /// <summary>
    /// Get the currently applied theme name
    /// </summary>
    string GetCurrentTheme();
}
