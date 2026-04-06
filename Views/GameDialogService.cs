using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HangmanWpf.Views;

public class AboutDialogService : IAboutDialogService
{
    public Task ShowAsync()
    {
        var dialog = new AboutWindow();
        TrySetOwner(dialog);
        dialog.ShowDialog();
        return Task.CompletedTask;
    }

    private static void TrySetOwner(Window dialog)
    {
        var owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (owner != null && owner != dialog)
        {
            dialog.Owner = owner;
        }
    }
}

public class StatisticsDialogService : IStatisticsDialogService
{
    public Task ShowAsync()
    {
        var dialog = new StatisticsWindow();
        TrySetOwner(dialog);
        dialog.ShowDialog();
        return Task.CompletedTask;
    }

    private static void TrySetOwner(Window dialog)
    {
        var owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (owner != null && owner != dialog)
        {
            dialog.Owner = owner;
        }
    }
}

public class GameSaveLoadDialogService : IGameSaveLoadDialogService
{
    public async Task<SavedGame?> ShowAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return null;
        }

        var dialog = new SaveLoadDialog();
        TrySetOwner(dialog);

        if (dialog.DataContext is not SaveLoadDialogViewModel viewModel)
        {
            return null;
        }

        SavedGame? loadedGame = null;

        void OnGameLoaded(SavedGame savedGame)
        {
            loadedGame = savedGame;
            dialog.Close();
        }

        void OnCancelRequested()
        {
            dialog.Close();
        }

        viewModel.Initialize(userId);
        viewModel.GameLoaded += OnGameLoaded;
        viewModel.CancellationRequested += OnCancelRequested;

        try
        {
            dialog.ShowDialog();
        }
        finally
        {
            viewModel.GameLoaded -= OnGameLoaded;
            viewModel.CancellationRequested -= OnCancelRequested;
        }

        return loadedGame;
    }

    private static void TrySetOwner(Window dialog)
    {
        var owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (owner != null && owner != dialog)
        {
            dialog.Owner = owner;
        }
    }
}
