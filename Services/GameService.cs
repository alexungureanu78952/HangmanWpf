using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace HangmanWpf.Services;


public class GameService : IGameService
{
    private GameSession _currentSession;
    private readonly IWordService _wordService;
    private Timer? _gameTimer;
    private Action<int>? _onTimeUpdateCallback;
    private Action? _onTimeoutCallback;
    private int _timerElapsedSeconds;
    private int _timerStartSeconds;
    private int _activeTimerId;

    // ASCII Hangman art stages (0-6 wrong guesses)
    private static readonly string[] HangmanStages = new[]
    {
        // Stage 0 - No mistakes
        """
          ------
          |    |
          |
          |
          |
          |
        ---------------
        """,
        // Stage 1 - Head
        """
          ------
          |    |
          |    O
          |
          |
          |
        ---------------
        """,
        // Stage 2 - Body
        """
          ------
          |    |
          |    O
          |    |
          |
          |
        ---------------
        """,
        // Stage 3 - Left arm
        """
          ------
          |    |
          |    O
          |   \|
          |
          |
        ---------------
        """,
        // Stage 4 - Right arm
        """
          ------
          |    |
          |    O
          |   \|/
          |
          |
        ---------------
        """,
        // Stage 5 - Left leg
        """
          ------
          |    |
          |    O
          |   \|/
          |   /
          |
        ---------------
        """,
        // Stage 6 - Right leg (Game Over)
        """
          ------
          |    |
          |    O
          |   \|/
          |   / \
          |
        ---------------
        """
    };

    public GameService(IWordService wordService)
    {
        _wordService = wordService ?? throw new ArgumentNullException(nameof(wordService));
        _currentSession = new GameSession();
        _timerElapsedSeconds = 0;
        _timerStartSeconds = 30;
        _activeTimerId = 0;
    }


    public async Task StartGameAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        int currentLevel = _currentSession.Level;

        _currentSession = new GameSession
        {
            Category = category,
            Word = await _wordService.GetRandomWordAsync(category),
            GuessedLetters = new(),
            Level = currentLevel,
            WrongCount = 0,
            TimeRemaining = 30,
            StartedAt = DateTime.UtcNow
        };

        _timerElapsedSeconds = 0;
        StopTimer();
    }


    public async Task<bool> GuessLetterAsync(char letter)
    {
        letter = char.ToUpper(letter);


        if (_currentSession.HasGuessedLetter(letter))
            return false;


        _currentSession.GuessedLetters.Add(letter);


        bool isCorrect = _currentSession.Word.Contains(letter);

        if (!isCorrect)
        {
            _currentSession.WrongCount++;
        }

        await Task.CompletedTask;
        return isCorrect;
    }


    public bool IsWordComplete()
    {
        return _currentSession.IsWordComplete();
    }


    public bool IsWordLost()
    {
        return _currentSession.WrongCount >= 6 || _currentSession.TimeRemaining <= 0;
    }


    public bool IsGameWon()
    {
        return _currentSession.Level >= 3;
    }


    public GameSession GetCurrentSession()
    {
        return _currentSession;
    }


    public int GetCurrentLevel()
    {
        return _currentSession.Level;
    }


    public void IncrementLevel()
    {
        if (_currentSession.Level < 3)
            _currentSession.Level++;
    }


    public void ResetLevel()
    {
        _currentSession.Level = 0;
    }


    public int GetTimeRemaining()
    {
        return _currentSession.TimeRemaining;
    }


    public void UpdateTimeRemaining(int secondsRemaining)
    {
        _currentSession.TimeRemaining = Math.Max(0, secondsRemaining);
    }


    public void StartTimer(Action<int> onTimeUpdate, Action onTimeoutCallback)
    {
        StopTimer();

        _onTimeUpdateCallback = onTimeUpdate;
        _onTimeoutCallback = onTimeoutCallback;
        _timerElapsedSeconds = 0;
        _timerStartSeconds = Math.Max(1, _currentSession.TimeRemaining);
        _activeTimerId++;
        int timerId = _activeTimerId;


        _onTimeUpdateCallback?.Invoke(_timerStartSeconds);

        _gameTimer = new Timer(1000);
        _gameTimer.Elapsed += (s, e) => OnTimerTick(timerId);
        _gameTimer.AutoReset = true;
        _gameTimer.Start();
    }


    public void StopTimer()
    {
        _activeTimerId++;

        if (_gameTimer != null)
        {
            _gameTimer.Stop();
            _gameTimer.Dispose();
            _gameTimer = null;
        }
    }


    public bool HasGuessedLetter(char letter)
    {
        return _currentSession.HasGuessedLetter(letter);
    }


    public string GetWordDisplay()
    {
        return _currentSession.GetWordDisplay();
    }


    public string GetHangmanDisplay()
    {
        int stage = Math.Min(_currentSession.WrongCount, 6);
        return HangmanStages[stage];
    }


    public int GetWrongCount()
    {
        return _currentSession.WrongCount;
    }


    public int GetGuessedLetterCount()
    {
        return _currentSession.GuessedLetters.Count;
    }


    public string GetCategory()
    {
        return _currentSession.Category;
    }


    public SavedGame CreateSaveSnapshot(Guid userId, string saveName)
    {
        if (string.IsNullOrWhiteSpace(saveName))
            throw new ArgumentException("Save name cannot be empty", nameof(saveName));

        return SavedGame.FromGameSession(_currentSession, userId, saveName);
    }


    public async Task RestoreFromSaveAsync(SavedGame savedGame)
    {
        if (savedGame == null)
            throw new ArgumentNullException(nameof(savedGame));

        _currentSession = savedGame.ToGameSession();
        _timerElapsedSeconds = 0;

        await Task.CompletedTask;
    }


    private void OnTimerTick(int timerId)
    {
        if (timerId != _activeTimerId)
            return;

        _timerElapsedSeconds++;
        int timeRemaining = _timerStartSeconds - _timerElapsedSeconds;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            _currentSession.TimeRemaining = 0;
            StopTimer();
            _onTimeoutCallback?.Invoke();
        }
        else
        {
            _currentSession.TimeRemaining = timeRemaining;
            _onTimeUpdateCallback?.Invoke(timeRemaining);
        }
    }
}
