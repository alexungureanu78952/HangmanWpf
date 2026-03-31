using System;
using System.Collections.Generic;

namespace HangmanWpf.Models;

/// <summary>
/// Represents a theme definition with color palette.
/// Loaded from JSON (DarkPurpleTheme.json, DarkRedTheme.json, etc.)
/// Colors are applied to WPF Application.Resources as DynamicResources.
/// This allows theme switching at runtime without recompilation.
/// </summary>
public class ThemeDefinition
{
    /// <summary>
    /// Theme name (e.g., "DarkPurple", "DarkRed")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dictionary of color key → hex value mappings.
    /// Example: "PrimaryBackground" → "#1a1a2e"
    /// These are loaded into Application.Current.Resources
    /// and referenced via {DynamicResource PrimaryBackground} in XAML
    /// </summary>
    public Dictionary<string, string> Colors { get; set; } = new();

    public ThemeDefinition()
    {
    }

    public ThemeDefinition(string name)
    {
        Name = name;
    }
}
