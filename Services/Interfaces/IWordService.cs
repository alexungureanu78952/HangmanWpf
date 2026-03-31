using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for loading word lists by category
/// </summary>
public interface IWordService
{
    /// <summary>
    /// Get all words for a specific category
    /// </summary>
    Task<List<string>> GetWordsByCategoryAsync(string category);

    /// <summary>
    /// Get list of all available categories
    /// </summary>
    Task<List<string>> GetAllCategoriesAsync();

    /// <summary>
    /// Get a random word from the specified category
    /// </summary>
    Task<string> GetRandomWordAsync(string category);
}
