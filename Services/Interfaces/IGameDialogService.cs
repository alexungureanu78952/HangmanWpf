using HangmanWpf.Models;
using System;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

public interface IGameDialogService
{
    Task ShowAboutDialogAsync();
    Task ShowStatisticsDialogAsync();
    Task<SavedGame?> ShowLoadGameDialogAsync(Guid userId);
}
