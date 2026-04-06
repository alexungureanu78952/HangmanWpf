using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for managing dynamic theme switching at runtime
/// </summary>
public interface IThemeService
{

    Task ApplyThemeAsync(string themeName);


    Task<List<string>> GetAvailableThemesAsync();


    string GetCurrentTheme();
}
