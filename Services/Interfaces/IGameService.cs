using HangmanWpf.Models;
using System;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for managing game logic and state
/// Handles word guessing, level tracking, win/loss conditions, and timer
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Start a new game with a word from the specified category
    /// </summary>
    Task StartGameAsync(string category);

    /// <summary>
    /// Make a guess (letter A-Z)
    /// Returns true if letter is in the word, false if wrong guess
    /// </summary>
    Task<bool> GuessLetterAsync(char letter);

    /// <summary>
    /// Check if the current word is completely guessed
    /// </summary>
    bool IsWordComplete();

    /// <summary>
    /// Check if the current word is lost (6 wrong guesses or timeout)
    /// </summary>
    bool IsWordLost();

    /// <summary>
    /// Check if the entire game is won (3 consecutive words guessed)
    /// </summary>
    bool IsGameWon();

    /// <summary>
    /// Get ASCII hangman art for the current stage (0-6 based on wrong count)
    /// </summary>
    string GetHangmanDisplay();

    /// <summary>
    /// Get current game session state
    /// </summary>
    GameSession GetCurrentSession();

    /// <summary>
    /// Get current level (0-3)
    /// </summary>
    int GetCurrentLevel();

    /// <summary>
    /// Increment level (called when word is won)
    /// </summary>
    void IncrementLevel();

    /// <summary>
    /// Reset level to 0 (called when word is lost)
    /// </summary>
    void ResetLevel();

    /// <summary>
    /// Get time remaining in seconds
    /// </summary>
    int GetTimeRemaining();

    /// <summary>
    /// Update time remaining (typically called per second from ViewModel)
    /// </summary>
    void UpdateTimeRemaining(int secondsRemaining);

    /// <summary>
    /// Start the countdown timer
    /// Callback fires every second, and when time runs out
    /// </summary>
    void StartTimer(Action<int> onTimeUpdate, Action onTimeoutCallback);

    /// <summary>
    /// Stop the countdown timer
    /// </summary>
    void StopTimer();

    /// <summary>
    /// Check if letter has already been guessed
    /// </summary>
    bool HasGuessedLetter(char letter);

    /// <summary>
    /// Get the display state of the word (e.g., "W_RD" for "WORD")
    /// </summary>
    string GetWordDisplay();

    /// <summary>
    /// Get number of wrong guesses (0-6)
    /// </summary>
    int GetWrongCount();

    /// <summary>
    /// Get total number of guessed letters
    /// </summary>
    int GetGuessedLetterCount();

    /// <summary>
    /// Get current category
    /// </summary>
    string GetCategory();

    /// <summary>
    /// Save game session to a SavedGame snapshot
    /// </summary>
    SavedGame CreateSaveSnapshot(Guid userId, string saveName);

    /// <summary>
    /// Restore game session from a SavedGame
    /// </summary>
    Task RestoreFromSaveAsync(SavedGame savedGame);
}
