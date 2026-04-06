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

    Task SaveGameAsync(SavedGame savedGame);


    Task<SavedGame?> LoadGameAsync(Guid userId, Guid gameId);


    Task<List<SavedGame>> GetSavedGamesAsync(Guid userId);


    Task DeleteSavedGameAsync(Guid userId, Guid gameId);


    Task DeleteAllUserGamesAsync(Guid userId);
}
