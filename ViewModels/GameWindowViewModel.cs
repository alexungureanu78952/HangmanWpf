using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for GameWindow
/// Manages game state, guessing, timer, save/load operations
/// </summary>
public class GameWindowViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    private readonly IWordService _wordService;
    private readonly IGamePersistenceService _persistenceService;
    private readonly IStatisticsService _statisticsService;
    private readonly IThemeService _themeService;

    private User _currentUser;
    private string _category;
    private string _wordDisplay;
    private string _hangmanDisplay;
    private int _timerSeconds;
    private int _level;
    private int _wrongCount;
    private bool _gameInProgress;
    private bool _wordComplete;
    private bool _gameWon;
    private ObservableCollection<string> _categories;
    private ObservableCollection<char> _guessedLetters;
    private string _statusMessage;

    public User CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string WordDisplay
    {
        get => _wordDisplay;
        set => SetProperty(ref _wordDisplay, value);
    }

    public string HangmanDisplay
    {
        get => _hangmanDisplay;
        set => SetProperty(ref _hangmanDisplay, value);
    }

    public int TimerSeconds
    {
        get => _timerSeconds;
        set => SetProperty(ref _timerSeconds, value);
    }

    public int Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    public int WrongCount
    {
        get => _wrongCount;
        set => SetProperty(ref _wrongCount, value);
    }

    public bool GameInProgress
    {
        get => _gameInProgress;
        set => SetProperty(ref _gameInProgress, value);
    }

    public bool WordComplete
    {
        get => _wordComplete;
        set => SetProperty(ref _wordComplete, value);
    }

    public bool GameWon
    {
        get => _gameWon;
        set => SetProperty(ref _gameWon, value);
    }

    public ObservableCollection<string> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public ObservableCollection<char> GuessedLetters
    {
        get => _guessedLetters;
        set => SetProperty(ref _guessedLetters, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand StartNewGameCommand { get; }
    public ICommand GuessLetterCommand { get; }
    public ICommand SaveGameCommand { get; }
    public ICommand LoadGameCommand { get; }
    public ICommand ChangeThemeCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action? GameClosed;

    public GameWindowViewModel(
        IGameService gameService,
        IWordService wordService,
        IGamePersistenceService persistenceService,
        IStatisticsService statisticsService,
        IThemeService themeService)
    {
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _wordService = wordService ?? throw new ArgumentNullException(nameof(wordService));
        _persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

        _currentUser = new User();
        _category = "Movies";
        _wordDisplay = "";
        _hangmanDisplay = "";
        _timerSeconds = 30;
        _level = 0;
        _wrongCount = 0;
        _gameInProgress = false;
        _wordComplete = false;
        _gameWon = false;
        _categories = new ObservableCollection<string>();
        _guessedLetters = new ObservableCollection<char>();
        _statusMessage = "Welcome to Hangman!";

        StartNewGameCommand = new AsyncRelayCommand(StartNewGameAsync);
        GuessLetterCommand = new RelayCommand(param => GuessLetter(param), param => GameInProgress && !WordComplete);
        SaveGameCommand = new AsyncRelayCommand(SaveGameAsync, () => GameInProgress);
        LoadGameCommand = new AsyncRelayCommand(LoadGameAsync, () => true);
        ChangeThemeCommand = new AsyncRelayCommand(async () => await ChangeThemeAsync("DarkRed"));
        ChangeCategoryCommand = new RelayCommand(param => OnChangeCategory(param));
        CancelCommand = new RelayCommand(_ => OnCancel());

        _ = LoadCategoriesAsync();
    }

    /// <summary>
    /// Initialize ViewModel with current user
    /// </summary>
    public void Initialize(User user)
    {
        CurrentUser = user;
    }

    /// <summary>
    /// Load available word categories
    /// </summary>
    private async System.Threading.Tasks.Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _wordService.GetAllCategoriesAsync();
            Categories = new ObservableCollection<string>(categories);
            if (categories.Count > 0)
                Category = categories[0];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
        }
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    private async System.Threading.Tasks.Task StartNewGameAsync()
    {
        try
        {
            _gameService.StopTimer();
            await _gameService.StartGameAsync(Category);

            GameInProgress = true;
            WordComplete = false;
            WordDisplay = _gameService.GetWordDisplay();
            HangmanDisplay = _gameService.GetHangmanDisplay();
            TimerSeconds = 30;
            Level = _gameService.GetCurrentLevel();
            WrongCount = _gameService.GetWrongCount();
            GuessedLetters.Clear();
            StatusMessage = "Game started! Guess a letter.";

            _gameService.StartTimer(
                onTimeUpdate: seconds => TimerSeconds = seconds,
                onTimeoutCallback: OnTimeout
            );
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error starting game: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Handle letter guess
    /// </summary>
    private async void GuessLetter(object? parameter)
    {
        if (parameter == null || !GameInProgress)
            return;

        string paramStr = parameter.ToString() ?? "";
        if (paramStr.Length != 1 || !char.IsLetter(paramStr[0]))
            return;

        char letter = char.ToUpper(paramStr[0]);

        // Check if already guessed
        if (_gameService.HasGuessedLetter(letter))
        {
            StatusMessage = $"'{letter}' already guessed!";
            return;
        }

        try
        {
            bool isCorrect = await _gameService.GuessLetterAsync(letter);
            GuessedLetters.Add(letter);

            WordDisplay = _gameService.GetWordDisplay();
            HangmanDisplay = _gameService.GetHangmanDisplay();
            WrongCount = _gameService.GetWrongCount();

            if (isCorrect)
                StatusMessage = $"'{letter}' is correct!";
            else
                StatusMessage = $"'{letter}' is wrong!";

            // Check win/loss conditions
            if (_gameService.IsWordLost())
            {
                _gameService.StopTimer();
                GameInProgress = false;
                await HandleWordLoss();
            }
            else if (_gameService.IsWordComplete())
            {
                _gameService.StopTimer();
                GameInProgress = false;
                await HandleWordWin();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Handle word loss
    /// </summary>
    private async System.Threading.Tasks.Task HandleWordLoss()
    {
        _gameService.ResetLevel();
        Level = _gameService.GetCurrentLevel();
        await _statisticsService.UpdateStatisticsAsync(CurrentUser.UserId, CurrentUser.Username, Category, false);
        StatusMessage = $"Game Over! Word was: {_gameService.GetCurrentSession().Word}. Level reset to 0.";
    }

    /// <summary>
    /// Handle word win
    /// </summary>
    private async System.Threading.Tasks.Task HandleWordWin()
    {
        _gameService.IncrementLevel();
        Level = _gameService.GetCurrentLevel();

        if (_gameService.IsGameWon())
        {
            GameWon = true;
            await _statisticsService.UpdateStatisticsAsync(CurrentUser.UserId, CurrentUser.Username, Category, true);
            StatusMessage = "Congratulations! You won the game!";
        }
        else
        {
            StatusMessage = $"Word guessed! Level: {Level}/3. Play again?";
        }
    }

    /// <summary>
    /// Handle timer timeout
    /// </summary>
    private void OnTimeout()
    {
        GameInProgress = false;
        _ = HandleWordLoss();
    }

    /// <summary>
    /// Save current game
    /// </summary>
    private async System.Threading.Tasks.Task SaveGameAsync()
    {
        if (!GameInProgress)
            return;

        try
        {
            var savedGame = _gameService.CreateSaveSnapshot(CurrentUser.UserId, $"Game_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
            await _persistenceService.SaveGameAsync(savedGame);
            StatusMessage = "Game saved!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving game: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Load a saved game
    /// </summary>
    private async System.Threading.Tasks.Task LoadGameAsync()
    {
        try
        {
            var savedGames = await _persistenceService.GetSavedGamesAsync(CurrentUser.UserId);
            if (savedGames.Count == 0)
            {
                StatusMessage = "No saved games found.";
                return;
            }

            // For now, load the first saved game
            var loadedGame = savedGames[0];
            await _gameService.RestoreFromSaveAsync(loadedGame);

            GameInProgress = true;
            WordComplete = false;
            WordDisplay = _gameService.GetWordDisplay();
            HangmanDisplay = _gameService.GetHangmanDisplay();
            TimerSeconds = _gameService.GetTimeRemaining();
            Level = _gameService.GetCurrentLevel();
            WrongCount = _gameService.GetWrongCount();
            GuessedLetters.Clear();
            foreach (var letter in _gameService.GetCurrentSession().GuessedLetters)
                GuessedLetters.Add(letter);

            StatusMessage = "Game loaded!";

            _gameService.StartTimer(
                onTimeUpdate: seconds => TimerSeconds = seconds,
                onTimeoutCallback: OnTimeout
            );
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading game: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Change theme
    /// </summary>
    private async System.Threading.Tasks.Task ChangeThemeAsync(string themeName)
    {
        try
        {
            await _themeService.ApplyThemeAsync(themeName);
            StatusMessage = $"Theme changed to {themeName}!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error changing theme: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// Handle cancel/close window
    /// </summary>
    private void OnCancel()
    {
        _gameService.StopTimer();
        GameClosed?.Invoke();
    }

    /// <summary>
    /// Change category and reset game state
    /// </summary>
    private void OnChangeCategory(object? parameter)
    {
        if (parameter == null)
            return;

        string category = parameter.ToString() ?? "Movies";
        if (Category != category)
        {
            Category = category;
            StatusMessage = $"Category changed to: {category}. Start a new game!";

            // Stop current game if in progress
            if (GameInProgress)
            {
                _gameService.StopTimer();
            }

            // Reset game state
            GameInProgress = false;
            WordComplete = false;
            GameWon = false;
            WordDisplay = "";
            HangmanDisplay = "";
            GuessedLetters.Clear();
            Level = 0;
            WrongCount = 0;
        }
    }
}
