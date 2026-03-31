using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace HangmanWpf.Services;

/// <summary>
/// Service for managing core hangman game logic
/// Handles word guessing, level tracking, timer, and win/loss conditions
/// </summary>
public class GameService : IGameService
{
    private GameSession _currentSession;
    private readonly IWordService _wordService;
    private Timer? _gameTimer;
    private Action<int>? _onTimeUpdateCallback;
    private Action? _onTimeoutCallback;
    private int _timerElapsedSeconds;

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
    }

    /// <summary>
    /// Start a new game with a word from the specified category
    /// </summary>
    public async Task StartGameAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        _currentSession = new GameSession
        {
            Category = category,
            Word = await _wordService.GetRandomWordAsync(category),
            GuessedLetters = new(),
            Level = 0,
            WrongCount = 0,
            TimeRemaining = 30,
            StartedAt = DateTime.UtcNow
        };

        _timerElapsedSeconds = 0;
        StopTimer();
    }

    /// <summary>
    /// Make a guess (letter A-Z)
    /// Returns true if letter is in the word, false if wrong guess
    /// </summary>
    public async Task<bool> GuessLetterAsync(char letter)
    {
        letter = char.ToUpper(letter);

        // Check if already guessed
        if (_currentSession.HasGuessedLetter(letter))
            return false; // Already guessed, invalid move

        // Add letter to guessed list
        _currentSession.GuessedLetters.Add(letter);

        // Check if letter is in word
        bool isCorrect = _currentSession.Word.Contains(letter);

        if (!isCorrect)
        {
            _currentSession.WrongCount++;
        }

        await Task.CompletedTask;
        return isCorrect;
    }

    /// <summary>
    /// Check if the current word is completely guessed
    /// </summary>
    public bool IsWordComplete()
    {
        return _currentSession.IsWordComplete();
    }

    /// <summary>
    /// Check if the current word is lost (6 wrong guesses or timeout)
    /// </summary>
    public bool IsWordLost()
    {
        return _currentSession.WrongCount >= 6 || _currentSession.TimeRemaining <= 0;
    }

    /// <summary>
    /// Check if the entire game is won (3 consecutive words guessed)
    /// </summary>
    public bool IsGameWon()
    {
        return _currentSession.Level >= 3;
    }

    /// <summary>
    /// Get current game session state
    /// </summary>
    public GameSession GetCurrentSession()
    {
        return _currentSession;
    }

    /// <summary>
    /// Get current level (0-3)
    /// </summary>
    public int GetCurrentLevel()
    {
        return _currentSession.Level;
    }

    /// <summary>
    /// Increment level when word is won
    /// </summary>
    public void IncrementLevel()
    {
        if (_currentSession.Level < 3)
            _currentSession.Level++;
    }

    /// <summary>
    /// Reset level to 0 when word is lost
    /// </summary>
    public void ResetLevel()
    {
        _currentSession.Level = 0;
    }

    /// <summary>
    /// Get time remaining in seconds
    /// </summary>
    public int GetTimeRemaining()
    {
        return _currentSession.TimeRemaining;
    }

    /// <summary>
    /// Update time remaining (called per second from ViewModel)
    /// </summary>
    public void UpdateTimeRemaining(int secondsRemaining)
    {
        _currentSession.TimeRemaining = Math.Max(0, secondsRemaining);
    }

    /// <summary>
    /// Start the countdown timer (fires every second)
    /// </summary>
    public void StartTimer(Action<int> onTimeUpdate, Action onTimeoutCallback)
    {
        _onTimeUpdateCallback = onTimeUpdate;
        _onTimeoutCallback = onTimeoutCallback;
        _timerElapsedSeconds = 0;

        _gameTimer = new Timer(1000); // Fire every 1 second
        _gameTimer.Elapsed += (s, e) => OnTimerTick();
        _gameTimer.AutoReset = true;
        _gameTimer.Start();
    }

    /// <summary>
    /// Stop the countdown timer
    /// </summary>
    public void StopTimer()
    {
        if (_gameTimer != null)
        {
            _gameTimer.Stop();
            _gameTimer.Dispose();
            _gameTimer = null;
        }
    }

    /// <summary>
    /// Check if letter has already been guessed
    /// </summary>
    public bool HasGuessedLetter(char letter)
    {
        return _currentSession.HasGuessedLetter(letter);
    }

    /// <summary>
    /// Get the display state of the word (e.g., "W_RD" for "WORD")
    /// </summary>
    public string GetWordDisplay()
    {
        return _currentSession.GetWordDisplay();
    }

    /// <summary>
    /// Get ASCII hangman display for current stage
    /// </summary>
    public string GetHangmanDisplay()
    {
        int stage = Math.Min(_currentSession.WrongCount, 6);
        return HangmanStages[stage];
    }

    /// <summary>
    /// Get number of wrong guesses (0-6)
    /// </summary>
    public int GetWrongCount()
    {
        return _currentSession.WrongCount;
    }

    /// <summary>
    /// Get total number of guessed letters
    /// </summary>
    public int GetGuessedLetterCount()
    {
        return _currentSession.GuessedLetters.Count;
    }

    /// <summary>
    /// Get current category
    /// </summary>
    public string GetCategory()
    {
        return _currentSession.Category;
    }

    /// <summary>
    /// Save game session to a SavedGame snapshot
    /// </summary>
    public SavedGame CreateSaveSnapshot(Guid userId, string saveName)
    {
        if (string.IsNullOrWhiteSpace(saveName))
            throw new ArgumentException("Save name cannot be empty", nameof(saveName));

        StopTimer(); // Stop timer before saving

        return SavedGame.FromGameSession(_currentSession, userId, saveName);
    }

    /// <summary>
    /// Restore game session from a SavedGame
    /// </summary>
    public async Task RestoreFromSaveAsync(SavedGame savedGame)
    {
        if (savedGame == null)
            throw new ArgumentNullException(nameof(savedGame));

        _currentSession = savedGame.ToGameSession();
        _timerElapsedSeconds = 0;

        await Task.CompletedTask;
    }

    /// <summary>
    /// Internal: Timer tick handler - updates time remaining
    /// </summary>
    private void OnTimerTick()
    {
        _timerElapsedSeconds++;
        int timeRemaining = 30 - _timerElapsedSeconds;

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
