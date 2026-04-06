using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HangmanWpf.Views;

public class GameDialogService : IGameDialogService
{
    public Task ShowAboutDialogAsync()
    {
        var dialog = new AboutWindow();
        TrySetOwner(dialog);
        dialog.ShowDialog();
        return Task.CompletedTask;
    }

    public Task ShowStatisticsDialogAsync()
    {
        var dialog = new StatisticsWindow();
        TrySetOwner(dialog);
        dialog.ShowDialog();
        return Task.CompletedTask;
    }

    public Task<SavedGame?> ShowLoadGameDialogAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Task.FromResult<SavedGame?>(null);
        }

        var dialog = new SaveLoadDialog();
        TrySetOwner(dialog);

        if (dialog.DataContext is not SaveLoadDialogViewModel viewModel)
        {
            return Task.FromResult<SavedGame?>(null);
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

        return Task.FromResult(loadedGame);
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
