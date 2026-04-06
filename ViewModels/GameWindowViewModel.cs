using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;


public class GameWindowViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    private readonly IWordService _wordService;
    private readonly IGamePersistenceService _persistenceService;
    private readonly IStatisticsService _statisticsService;
    private readonly IThemeService _themeService;
    private readonly IAboutDialogService _aboutDialogService;
    private readonly IStatisticsDialogService _statisticsDialogService;
    private readonly IGameSaveLoadDialogService _saveLoadDialogService;
    private readonly IUiDispatcher _uiDispatcher;

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
    private ObservableCollection<LetterTileViewModel> _letterTiles;
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
        set
        {
            if (SetProperty(ref _gameInProgress, value))
            {
                RefreshCommandStates();
            }
        }
    }

    public bool WordComplete
    {
        get => _wordComplete;
        set
        {
            if (SetProperty(ref _wordComplete, value))
            {
                RefreshCommandStates();
            }
        }
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

    public ObservableCollection<LetterTileViewModel> LetterTiles
    {
        get => _letterTiles;
        set => SetProperty(ref _letterTiles, value);
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
    public ICommand ShowStatisticsCommand { get; }
    public ICommand ShowAboutCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action? GameClosed;

    public GameWindowViewModel(
        IGameService gameService,
        IWordService wordService,
        IGamePersistenceService persistenceService,
        IStatisticsService statisticsService,
        IThemeService themeService,
        IAboutDialogService aboutDialogService,
        IStatisticsDialogService statisticsDialogService,
        IGameSaveLoadDialogService saveLoadDialogService,
        IUiDispatcher uiDispatcher)
    {
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _wordService = wordService ?? throw new ArgumentNullException(nameof(wordService));
        _persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _aboutDialogService = aboutDialogService ?? throw new ArgumentNullException(nameof(aboutDialogService));
        _statisticsDialogService = statisticsDialogService ?? throw new ArgumentNullException(nameof(statisticsDialogService));
        _saveLoadDialogService = saveLoadDialogService ?? throw new ArgumentNullException(nameof(saveLoadDialogService));
        _uiDispatcher = uiDispatcher ?? throw new ArgumentNullException(nameof(uiDispatcher));

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
        _letterTiles = new ObservableCollection<LetterTileViewModel>();
        _guessedLetters = new ObservableCollection<char>();
        _statusMessage = "Welcome to Hangman!";

        StartNewGameCommand = new AsyncRelayCommand(StartNewGameAsync);
        GuessLetterCommand = new RelayCommand(param => GuessLetter(param), param => GameInProgress && !WordComplete);
        SaveGameCommand = new AsyncRelayCommand(SaveGameAsync, () => GameInProgress);
        LoadGameCommand = new AsyncRelayCommand(OpenSavedGameAsync);
        ChangeThemeCommand = new AsyncRelayCommand<string>(ChangeThemeAsync);
        ChangeCategoryCommand = new RelayCommand(param => OnChangeCategory(param));
        ShowStatisticsCommand = new AsyncRelayCommand(ShowStatisticsAsync);
        ShowAboutCommand = new AsyncRelayCommand(ShowAboutAsync);
        CancelCommand = new RelayCommand(_ => OnCancel());

        _ = LoadCategoriesAsync();
        InitializeLetterTiles();
    }


    public void Initialize(User user)
    {
        CurrentUser = user;
    }

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


    private async System.Threading.Tasks.Task StartNewGameAsync()
    {
        try
        {
            _gameService.StopTimer();
            await _gameService.StartGameAsync(Category);

            GameInProgress = true;
            WordComplete = false;
            GameWon = false;
            WordDisplay = _gameService.GetWordDisplay();
            HangmanDisplay = _gameService.GetHangmanDisplay();
            TimerSeconds = 30;
            Level = _gameService.GetCurrentLevel();
            WrongCount = _gameService.GetWrongCount();
            GuessedLetters.Clear();
            ResetLetterTiles();
            StatusMessage = "Game started! Guess a letter.";

            _gameService.StartTimer(
                onTimeUpdate: seconds => _uiDispatcher.Invoke(() => TimerSeconds = seconds),
                onTimeoutCallback: () => _uiDispatcher.Invoke(OnTimeout)
            );

            RefreshCommandStates();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error starting game: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }


    private async void GuessLetter(object? parameter)
    {
        if (parameter == null || !GameInProgress)
            return;

        string paramStr = parameter.ToString() ?? "";
        if (paramStr.Length != 1 || !char.IsLetter(paramStr[0]))
            return;

        char letter = char.ToUpper(paramStr[0]);


        if (_gameService.HasGuessedLetter(letter))
        {
            StatusMessage = $"'{letter}' already guessed!";
            return;
        }

        try
        {
            bool isCorrect = await _gameService.GuessLetterAsync(letter);
            GuessedLetters.Add(letter);
            UpdateLetterTile(letter, isCorrect);

            WordDisplay = _gameService.GetWordDisplay();
            HangmanDisplay = _gameService.GetHangmanDisplay();
            WrongCount = _gameService.GetWrongCount();

            if (isCorrect)
                StatusMessage = $"'{letter}' is correct!";
            else
                StatusMessage = $"'{letter}' is wrong!";


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

    private async System.Threading.Tasks.Task HandleWordLoss()
    {
        _gameService.ResetLevel();
        Level = _gameService.GetCurrentLevel();
        await _statisticsService.UpdateStatisticsAsync(CurrentUser.UserId, CurrentUser.Username, Category, false);
        StatusMessage = $"Game Over! Word was: {_gameService.GetCurrentSession().Word}. Level reset to 0.";
    }

    private async System.Threading.Tasks.Task HandleWordWin()
    {
        _gameService.IncrementLevel();
        Level = _gameService.GetCurrentLevel();

        if (_gameService.IsGameWon())
        {
            GameWon = true;
            await _statisticsService.UpdateStatisticsAsync(CurrentUser.UserId, CurrentUser.Username, Category, true);


            _gameService.ResetLevel();
            Level = _gameService.GetCurrentLevel();
            StatusMessage = "Congratulations! You won the game! Counter reset to 0. Click New Game.";
        }
        else
        {
            StatusMessage = $"Word guessed! Level: {Level}/3. Click New Game to continue.";
        }
    }


    private void OnTimeout()
    {

        if (!GameInProgress || _gameService.GetTimeRemaining() > 0)
        {
            return;
        }

        GameInProgress = false;
        _ = HandleWordLoss();
    }

    private async System.Threading.Tasks.Task SaveGameAsync()
    {
        if (!GameInProgress)
            return;

        if (CurrentUser.UserId == Guid.Empty)
        {
            StatusMessage = "Select a valid user before saving.";
            return;
        }

        try
        {
            var saveName = $"{Category}_L{Level}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            var savedGame = _gameService.CreateSaveSnapshot(CurrentUser.UserId, saveName);
            await _persistenceService.SaveGameAsync(savedGame);
            StatusMessage = $"Game saved: {saveName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving game: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private async System.Threading.Tasks.Task OpenSavedGameAsync()
    {
        if (CurrentUser.UserId == Guid.Empty)
        {
            return;
        }

        var loadedGame = await _saveLoadDialogService.ShowAsync(CurrentUser.UserId);
        if (loadedGame != null)
        {
            await RestoreSavedGameAsync(loadedGame);
        }
    }

    private System.Threading.Tasks.Task ShowStatisticsAsync()
    {
        return _statisticsDialogService.ShowAsync();
    }

    private System.Threading.Tasks.Task ShowAboutAsync()
    {
        return _aboutDialogService.ShowAsync();
    }

    public async System.Threading.Tasks.Task RestoreSavedGameAsync(SavedGame loadedGame)
    {
        if (loadedGame == null)
        {
            return;
        }

        try
        {
            await _gameService.RestoreFromSaveAsync(loadedGame);

            GameInProgress = true;
            WordComplete = false;
            GameWon = false;
            WordDisplay = _gameService.GetWordDisplay();
            HangmanDisplay = _gameService.GetHangmanDisplay();
            TimerSeconds = _gameService.GetTimeRemaining();
            Level = _gameService.GetCurrentLevel();
            WrongCount = _gameService.GetWrongCount();
            GuessedLetters.Clear();
            ResetLetterTiles();
            foreach (var letter in _gameService.GetCurrentSession().GuessedLetters)
            {
                GuessedLetters.Add(letter);
                UpdateLetterTile(letter, _gameService.GetCurrentSession().Word.Contains(letter));
            }

            StatusMessage = $"Game loaded: {loadedGame.SaveName}";

            _gameService.StartTimer(
                onTimeUpdate: seconds => _uiDispatcher.Invoke(() => TimerSeconds = seconds),
                onTimeoutCallback: () => _uiDispatcher.Invoke(OnTimeout)
            );

            RefreshCommandStates();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading game: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private void RefreshCommandStates()
    {
        if (SaveGameCommand is AsyncRelayCommand saveCommand)
        {
            saveCommand.RaiseCanExecuteChanged();
        }

        if (GuessLetterCommand is RelayCommand guessCommand)
        {
            guessCommand.RaiseCanExecuteChanged();
        }
    }

    private async System.Threading.Tasks.Task ChangeThemeAsync(string? themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName))
        {
            return;
        }

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

    private void OnCancel()
    {
        _gameService.StopTimer();
        GameClosed?.Invoke();
    }

    private void OnChangeCategory(object? parameter)
    {
        if (parameter == null)
            return;

        string category = parameter.ToString() ?? "Movies";
        if (Category != category)
        {
            Category = category;
            StatusMessage = $"Category changed to: {category}. Start a new game!";

            if (GameInProgress)
            {
                _gameService.StopTimer();
            }

            _gameService.ResetLevel();
            GameInProgress = false;
            WordComplete = false;
            GameWon = false;
            WordDisplay = "";
            HangmanDisplay = "";
            GuessedLetters.Clear();
            ResetLetterTiles();
            Level = _gameService.GetCurrentLevel();
            WrongCount = 0;
        }
    }

    private void InitializeLetterTiles()
    {
        LetterTiles = new ObservableCollection<LetterTileViewModel>(
            Enumerable.Range('A', 26).Select(letter => new LetterTileViewModel((char)letter)));
    }

    private void ResetLetterTiles()
    {
        foreach (var tile in LetterTiles)
        {
            tile.State = LetterGuessState.Available;
        }
    }

    private void UpdateLetterTile(char letter, bool isCorrect)
    {
        var tile = LetterTiles.FirstOrDefault(t => t.Letter == letter);
        if (tile == null)
        {
            return;
        }

        tile.State = isCorrect ? LetterGuessState.Correct : LetterGuessState.Wrong;
    }
}
