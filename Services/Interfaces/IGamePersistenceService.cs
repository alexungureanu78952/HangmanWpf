using HangmanWpf.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for saving and loading game sessions
/// </summary>
public interface IGamePersistenceService
{
    /// <summary>
    /// Save a game session to persistent storage
    /// </summary>
    Task SaveGameAsync(SavedGame savedGame);

    /// <summary>
    /// Load a saved game by ID
    /// </summary>
    Task<SavedGame?> LoadGameAsync(Guid userId, Guid gameId);

    /// <summary>
    /// Get all saved games for a user
    /// </summary>
    Task<List<SavedGame>> GetSavedGamesAsync(Guid userId);

    /// <summary>
    /// Delete a saved game
    /// </summary>
    Task DeleteSavedGameAsync(Guid userId, Guid gameId);

    /// <summary>
    /// Delete all saved games for a user (cascade on user deletion)
    /// </summary>
    Task DeleteAllUserGamesAsync(Guid userId);
}
