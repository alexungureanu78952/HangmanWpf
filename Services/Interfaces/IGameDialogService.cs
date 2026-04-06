using HangmanWpf.Models;
using System;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

public interface IAboutDialogService
{
    Task ShowAsync();
}

public interface IStatisticsDialogService
{
    Task ShowAsync();
}

public interface IGameSaveLoadDialogService
{
    Task<SavedGame?> ShowAsync(Guid userId);
}
