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


public class StatisticsService : IStatisticsService
{
    private const string StatisticsFilePath = "Resources/Data/statistics.json";

    public event Action? StatisticsChanged;


    public async Task<Statistics?> GetStatisticsAsync(Guid userId)
    {
        var allStats = await GetAllStatisticsAsync();
        return allStats.FirstOrDefault(s => s.UserId == userId);
    }


    public async Task UpdateStatisticsAsync(Guid userId, string username, string category, bool isWon)
    {
        var allStats = await GetAllStatisticsAsync();
        var userStats = allStats.FirstOrDefault(s => s.UserId == userId);

        if (userStats == null)
        {
            userStats = new Statistics(userId, username);
            allStats.Add(userStats);
        }


        userStats.IncrementGamesPlayed(category);
        if (isWon)
            userStats.IncrementGamesWon(category);

        await SaveStatisticsAsync(allStats);
        StatisticsChanged?.Invoke();
    }


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
