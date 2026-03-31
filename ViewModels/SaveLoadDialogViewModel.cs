using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for SaveLoadDialog
/// Manages saving and loading game sessions
/// </summary>
public class SaveLoadDialogViewModel : ViewModelBase
{
    private readonly IGamePersistenceService _persistenceService;
    private Guid _userId;
    private ObservableCollection<SavedGame> _savedGames;
    private SavedGame? _selectedGame;
    private string _saveName;
    private bool _isLoading;

    public ObservableCollection<SavedGame> SavedGames
    {
        get => _savedGames;
        set => SetProperty(ref _savedGames, value);
    }

    public SavedGame? SelectedGame
    {
        get => _selectedGame;
        set
        {
            SetProperty(ref _selectedGame, value);
            ((RelayCommand)LoadGameCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteGameCommand).RaiseCanExecuteChanged();
        }
    }

    public string SaveName
    {
        get => _saveName;
        set => SetProperty(ref _saveName, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadSavedGamesCommand { get; }
    public ICommand SaveGameCommand { get; }
    public ICommand LoadGameCommand { get; }
    public ICommand DeleteGameCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<SavedGame>? GameLoaded;
    public event Action? CancellationRequested;

    public SaveLoadDialogViewModel(IGamePersistenceService persistenceService)
    {
        _persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        _userId = Guid.Empty;
        _savedGames = new ObservableCollection<SavedGame>();
        _selectedGame = null;
        _saveName = string.Empty;
        _isLoading = false;

        LoadSavedGamesCommand = new AsyncRelayCommand(LoadSavedGamesAsync);
        SaveGameCommand = new AsyncRelayCommand(SaveGameAsync, () => !string.IsNullOrWhiteSpace(SaveName));
        LoadGameCommand = new RelayCommand(_ => OnLoadGame(), _ => SelectedGame != null);
        DeleteGameCommand = new RelayCommand(_ => OnDeleteGame(), _ => SelectedGame != null);
        CancelCommand = new RelayCommand(_ => OnCancel());
    }

    /// <summary>
    /// Initialize with user ID
    /// </summary>
    public void Initialize(Guid userId)
    {
        _userId = userId;
        _ = LoadSavedGamesAsync();
    }

    /// <summary>
    /// Load all saved games for the user
    /// </summary>
    private async System.Threading.Tasks.Task LoadSavedGamesAsync()
    {
        if (_userId == Guid.Empty)
            return;

        IsLoading = true;
        try
        {
            var games = await _persistenceService.GetSavedGamesAsync(_userId);
            SavedGames = new ObservableCollection<SavedGame>(games);
            SelectedGame = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading saved games: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Save a game (placeholder - actual save done in GameWindowViewModel)
    /// </summary>
    private async System.Threading.Tasks.Task SaveGameAsync()
    {
        await System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// Load the selected game
    /// </summary>
    private void OnLoadGame()
    {
        if (SelectedGame != null)
        {
            GameLoaded?.Invoke(SelectedGame);
        }
    }

    /// <summary>
    /// Delete the selected game
    /// </summary>
    private async void OnDeleteGame()
    {
        if (SelectedGame == null)
            return;

        try
        {
            await _persistenceService.DeleteSavedGameAsync(_userId, SelectedGame.SavedGameId);
            await LoadSavedGamesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting game: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle cancel
    /// </summary>
    private void OnCancel()
    {
        CancellationRequested?.Invoke();
    }
}
