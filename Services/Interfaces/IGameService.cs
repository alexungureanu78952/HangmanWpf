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

    Task StartGameAsync(string category);


    Task<bool> GuessLetterAsync(char letter);


    bool IsWordComplete();


    bool IsWordLost();

    /// <summary>
    /// Check if the entire game is won (3 consecutive words guessed)
    /// </summary>
    bool IsGameWon();

    /// <summary>
    /// Get ASCII hangman art for the current stage (0-6 based on wrong count)
    /// </summary>
    string GetHangmanDisplay();


    GameSession GetCurrentSession();

    /// <summary>
    /// Get current level (0-3)
    /// </summary>
    int GetCurrentLevel();


    void IncrementLevel();


    void ResetLevel();


    int GetTimeRemaining();


    void UpdateTimeRemaining(int secondsRemaining);


    void StartTimer(Action<int> onTimeUpdate, Action onTimeoutCallback);


    void StopTimer();


    bool HasGuessedLetter(char letter);


    string GetWordDisplay();

    int GetWrongCount();


    int GetGuessedLetterCount();


    string GetCategory();


    SavedGame CreateSaveSnapshot(Guid userId, string saveName);


    Task RestoreFromSaveAsync(SavedGame savedGame);
}
