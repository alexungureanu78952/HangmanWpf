using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HangmanWpf.Services;

/// <summary>
/// Service for managing dynamic theme switching at runtime
/// Loads theme definitions from JSON and applies colors to Application.Resources
/// Enables theme switching without recompilation
/// </summary>
public class ThemeService : IThemeService
{
    private const string ThemesBasePath = "Resources/Themes";
    private string _currentTheme = "DarkPurple"; // Default theme

    /// <summary>
    /// Apply a theme by name (loads JSON and applies to Application.Resources)
    /// </summary>
    public async Task ApplyThemeAsync(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName))
            throw new ArgumentException("Theme name cannot be empty", nameof(themeName));

        try
        {
            var themePath = GetThemeFilePath(themeName);

            if (!File.Exists(themePath))
                throw new FileNotFoundException($"Theme file not found: {themePath}");

            var json = await File.ReadAllTextAsync(themePath);
            var themeDefinition = JsonConvert.DeserializeObject<ThemeDefinition>(json);

            if (themeDefinition == null || themeDefinition.Colors == null)
                throw new InvalidOperationException("Invalid theme definition");

            // Apply colors to Application.Resources
            // Convert hex color strings to SolidColorBrush objects
            foreach (var colorEntry in themeDefinition.Colors)
            {
                try
                {
                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorEntry.Value));
                    brush.Freeze(); // Freeze for better performance
                    Application.Current.Resources[colorEntry.Key] = brush;
                }
                catch (Exception colorEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error converting color '{colorEntry.Key}' ({colorEntry.Value}): {colorEx.Message}");
                }
            }

            _currentTheme = themeName;
            System.Diagnostics.Debug.WriteLine($"Theme '{themeName}' applied successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying theme '{themeName}': {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get list of available themes (by scanning theme files)
    /// </summary>
    public async Task<List<string>> GetAvailableThemesAsync()
    {
        var themesPath = PathHelpers.GetRelativePath(ThemesBasePath);
        var themes = new List<string>();

        try
        {
            if (!Directory.Exists(themesPath))
            {
                System.Diagnostics.Debug.WriteLine($"Themes directory not found: {themesPath}");
                return themes;
            }

            var themeFiles = Directory.GetFiles(themesPath, "*Theme.json");

            foreach (var filePath in themeFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    var themeDefinition = JsonConvert.DeserializeObject<ThemeDefinition>(json);

                    if (themeDefinition != null && !string.IsNullOrEmpty(themeDefinition.Name))
                        themes.Add(themeDefinition.Name);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error reading theme file {filePath}: {ex.Message}");
                }
            }

            return themes;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error scanning themes directory: {ex.Message}");
            return themes;
        }
    }

    /// <summary>
    /// Get the currently applied theme name
    /// </summary>
    public string GetCurrentTheme()
    {
        return _currentTheme;
    }

    /// <summary>
    /// Internal: Get full path to theme file
    /// </summary>
    private string GetThemeFilePath(string themeName)
    {
        var themesPath = PathHelpers.GetRelativePath(ThemesBasePath);
        return Path.Combine(themesPath, $"{themeName}Theme.json");
    }
}
