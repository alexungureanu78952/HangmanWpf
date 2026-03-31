using HangmanWpf.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for managing game statistics persistence
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Get statistics for a specific user
    /// </summary>
    Task<Statistics?> GetStatisticsAsync(Guid userId);

    /// <summary>
    /// Update statistics after a game completes
    /// </summary>
    Task UpdateStatisticsAsync(Guid userId, string username, string category, bool isWon);

    /// <summary>
    /// Get all statistics for all users
    /// </summary>
    Task<List<Statistics>> GetAllStatisticsAsync();

    /// <summary>
    /// Delete all statistics for a user (cascade on user deletion)
    /// </summary>
    Task DeleteUserStatisticsAsync(Guid userId);
}
