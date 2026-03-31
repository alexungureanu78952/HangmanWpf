using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HangmanWpf.Services;

/// <summary>
/// Service for loading word lists by category
/// Reads from Resources/Words/AllCategories.json
/// </summary>
public class WordService : IWordService
{
    private const string WordsFilePath = "Resources/Words/AllCategories.json";
    private Dictionary<string, List<string>>? _wordCache;

    /// <summary>
    /// Get all words for a specific category
    /// </summary>
    public async Task<List<string>> GetWordsByCategoryAsync(string category)
    {
        var allWords = await LoadAllWordsAsync();

        if (allWords.TryGetValue(category, out var words))
            return new List<string>(words);

        System.Diagnostics.Debug.WriteLine($"Category '{category}' not found in word lists");
        return new List<string>();
    }

    /// <summary>
    /// Get list of all available categories
    /// </summary>
    public async Task<List<string>> GetAllCategoriesAsync()
    {
        var allWords = await LoadAllWordsAsync();
        return allWords.Keys.ToList();
    }

    /// <summary>
    /// Get a random word from the specified category
    /// </summary>
    public async Task<string> GetRandomWordAsync(string category)
    {
        var words = await GetWordsByCategoryAsync(category);

        if (words.Count == 0)
            throw new InvalidOperationException($"No words available for category '{category}'");

        var random = new Random();
        var randomIndex = random.Next(words.Count);
        return words[randomIndex].ToUpper();
    }

    /// <summary>
    /// Internal: Load all words from JSON (cached)
    /// </summary>
    private async Task<Dictionary<string, List<string>>> LoadAllWordsAsync()
    {
        // Return cached data if available
        if (_wordCache != null)
            return _wordCache;

        var filePath = PathHelpers.GetRelativePath(WordsFilePath);

        try
        {
            if (!File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine($"Word file not found: {filePath}");
                return new Dictionary<string, List<string>>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            _wordCache = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json) 
                ?? new Dictionary<string, List<string>>();

            return _wordCache;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading words.json: {ex.Message}");
            return new Dictionary<string, List<string>>();
        }
    }

    /// <summary>
    /// Clear word cache (useful for testing)
    /// </summary>
    public void ClearCache()
    {
        _wordCache = null;
    }
}
