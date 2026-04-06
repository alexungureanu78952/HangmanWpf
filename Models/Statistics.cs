using System;
using System.Collections.Generic;
using System.Linq;

namespace HangmanWpf.Models;

/// <summary>
/// Represents statistics for a single category for a user.
/// Example: User "Corina" has played 5 "Movies" games, won 3.
/// </summary>
public class CategoryStats
{
    /// <summary>
    /// Category name (e.g., "Movies", "Cars", "Rivers")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Total games played in this category
    /// (counts only games that are completed: won or lost)
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Total games won in this category
    /// (won = 3 consecutive words guessed)
    /// </summary>
    public int GamesWon { get; set; }



    public CategoryStats(string category)
    {
        Category = category;
    }
}

/// <summary>
/// Represents aggregated statistics for a single user across all categories.
/// Serialized to statistics.json
/// </summary>
public class Statistics
{
    /// <summary>
    /// User ID these statistics belong to
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Username for display purposes
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Statistics per category
    /// </summary>
    public List<CategoryStats> CategoryStats { get; set; } = new();

    public Statistics()
    {
    }

    public Statistics(Guid userId, string username)
    {
        UserId = userId;
        Username = username;
    }

    /// <summary>
    /// Get or create stats for a category
    /// </summary>
    public CategoryStats GetOrCreateCategory(string category)
    {
        var existing = CategoryStats.FirstOrDefault(c => c.Category == category);
        if (existing != null)
            return existing;

        var newCategory = new CategoryStats(category);
        CategoryStats.Add(newCategory);
        return newCategory;
    }

    /// <summary>
    /// Increment games played for a category
    /// </summary>
    public void IncrementGamesPlayed(string category)
    {
        GetOrCreateCategory(category).GamesPlayed++;
    }

    /// <summary>
    /// Increment games won for a category
    /// </summary>
    public void IncrementGamesWon(string category)
    {
        GetOrCreateCategory(category).GamesWon++;
    }
}
