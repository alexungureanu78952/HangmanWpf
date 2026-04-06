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

    event Action? StatisticsChanged;


    Task<Statistics?> GetStatisticsAsync(Guid userId);


    Task UpdateStatisticsAsync(Guid userId, string username, string category, bool isWon);


    Task<List<Statistics>> GetAllStatisticsAsync();


    Task DeleteUserStatisticsAsync(Guid userId);
}
