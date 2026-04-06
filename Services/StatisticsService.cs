using HangmanWpf.Models;
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
/// Service for managing game statistics persistence (statistics.json)
/// </summary>
public class StatisticsService : IStatisticsService
{
    private const string StatisticsFilePath = "Resources/Data/statistics.json";

    public event Action? StatisticsChanged;

    /// <summary>
    /// Get statistics for a specific user
    /// </summary>
    public async Task<Statistics?> GetStatisticsAsync(Guid userId)
    {
        var allStats = await GetAllStatisticsAsync();
        return allStats.FirstOrDefault(s => s.UserId == userId);
    }

    /// <summary>
    /// Update statistics after game completes (win or loss)
    /// Creates user stats if doesn't exist
    /// </summary>
    public async Task UpdateStatisticsAsync(Guid userId, string username, string category, bool isWon)
    {
        var allStats = await GetAllStatisticsAsync();
        var userStats = allStats.FirstOrDefault(s => s.UserId == userId);

        if (userStats == null)
        {
            userStats = new Statistics(userId, username);
            allStats.Add(userStats);
        }

        // Update category stats
        userStats.IncrementGamesPlayed(category);
        if (isWon)
            userStats.IncrementGamesWon(category);

        await SaveStatisticsAsync(allStats);
        StatisticsChanged?.Invoke();
    }

    /// <summary>
    /// Get all statistics for all users
    /// </summary>
    public async Task<List<Statistics>> GetAllStatisticsAsync()
    {
        var filePath = PathHelpers.GetRelativePath(StatisticsFilePath);

        try
        {
            if (!File.Exists(filePath))
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(new List<Statistics>(), Formatting.Indented));
                return new List<Statistics>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            var stats = JsonConvert.DeserializeObject<List<Statistics>>(json) ?? new List<Statistics>();
            return stats;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading statistics.json: {ex.Message}");
            return new List<Statistics>();
        }
    }

    /// <summary>
    /// Delete all statistics for a user (cascade on user deletion)
    /// </summary>
    public async Task DeleteUserStatisticsAsync(Guid userId)
    {
        var allStats = await GetAllStatisticsAsync();
        var userStats = allStats.FirstOrDefault(s => s.UserId == userId);

        if (userStats != null)
        {
            allStats.Remove(userStats);
            await SaveStatisticsAsync(allStats);
            StatisticsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Internal: Save statistics to JSON
    /// </summary>
    private async Task SaveStatisticsAsync(List<Statistics> statistics)
    {
        var filePath = PathHelpers.GetRelativePath(StatisticsFilePath);
        var json = JsonConvert.SerializeObject(statistics, Formatting.Indented);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, json);
    }
}
