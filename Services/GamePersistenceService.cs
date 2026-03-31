using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HangmanWpf.Services;

/// <summary>
/// Service for saving and loading game sessions
/// Stores saved games in: Resources/Data/SavedGames/{UserId}/{GameId}.json
/// </summary>
public class GamePersistenceService : IGamePersistenceService
{
    private const string SavedGamesBasePath = "Resources/Data/SavedGames";

    /// <summary>
    /// Save a game session to disk
    /// Creates directory structure if needed: SavedGames/{UserId}/
    /// </summary>
    public async Task SaveGameAsync(SavedGame savedGame)
    {
        if (savedGame == null)
            throw new ArgumentNullException(nameof(savedGame));

        var userPath = GetUserSavePath(savedGame.UserId);
        if (!Directory.Exists(userPath))
            Directory.CreateDirectory(userPath);

        var filePath = Path.Combine(userPath, $"{savedGame.SavedGameId}.json");
        var json = JsonConvert.SerializeObject(savedGame, Formatting.Indented);

        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Load a saved game by ID
    /// </summary>
    public async Task<SavedGame?> LoadGameAsync(Guid userId, Guid gameId)
    {
        var userPath = GetUserSavePath(userId);
        var filePath = Path.Combine(userPath, $"{gameId}.json");

        try
        {
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            var savedGame = JsonConvert.DeserializeObject<SavedGame>(json);
            return savedGame;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading game {gameId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get all saved games for a user
    /// </summary>
    public async Task<List<SavedGame>> GetSavedGamesAsync(Guid userId)
    {
        var userPath = GetUserSavePath(userId);
        var savedGames = new List<SavedGame>();

        if (!Directory.Exists(userPath))
            return savedGames;

        var gameFiles = Directory.GetFiles(userPath, "*.json");

        foreach (var filePath in gameFiles)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var savedGame = JsonConvert.DeserializeObject<SavedGame>(json);
                if (savedGame != null)
                    savedGames.Add(savedGame);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading saved game {filePath}: {ex.Message}");
            }
        }

        return savedGames;
    }

    /// <summary>
    /// Delete a specific saved game
    /// </summary>
    public async Task DeleteSavedGameAsync(Guid userId, Guid gameId)
    {
        var userPath = GetUserSavePath(userId);
        var filePath = Path.Combine(userPath, $"{gameId}.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Delete all saved games for a user (cascade on user deletion)
    /// </summary>
    public async Task DeleteAllUserGamesAsync(Guid userId)
    {
        var userPath = GetUserSavePath(userId);

        if (Directory.Exists(userPath))
        {
            try
            {
                Directory.Delete(userPath, recursive: true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting user save folder {userPath}: {ex.Message}");
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Internal: Get the directory path for a user's saved games
    /// </summary>
    private string GetUserSavePath(Guid userId)
    {
        var basePath = PathHelpers.GetRelativePath(SavedGamesBasePath);
        return Path.Combine(basePath, userId.ToString());
    }
}
