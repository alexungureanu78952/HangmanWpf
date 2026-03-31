using System;
using System.Collections.Generic;

namespace HangmanWpf.Models;

/// <summary>
/// Represents a saved game snapshot that can be persisted to JSON.
/// Extends GameSession with metadata needed for save/load functionality.
/// </summary>
public class SavedGame : GameSession
{
    /// <summary>
    /// Unique identifier for this saved game
    /// </summary>
    public Guid SavedGameId { get; set; }

    /// <summary>
    /// User who saved this game
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Display name for the saved game (chosen by user when saving)
    /// </summary>
    public string SaveName { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when game was saved
    /// </summary>
    public DateTime SavedAt { get; set; }

    public SavedGame() : base()
    {
        SavedGameId = Guid.NewGuid();
        SavedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Create SavedGame from current GameSession
    /// </summary>
    public static SavedGame FromGameSession(GameSession session, Guid userId, string saveName)
    {
        return new SavedGame
        {
            Word = session.Word,
            Category = session.Category,
            GuessedLetters = new(session.GuessedLetters),
            Level = session.Level,
            WrongCount = session.WrongCount,
            TimeRemaining = session.TimeRemaining,
            StartedAt = session.StartedAt,
            UserId = userId,
            SaveName = saveName
        };
    }

    /// <summary>
    /// Restore GameSession from SavedGame
    /// </summary>
    public GameSession ToGameSession()
    {
        return new GameSession
        {
            Word = this.Word,
            Category = this.Category,
            GuessedLetters = new(this.GuessedLetters),
            Level = this.Level,
            WrongCount = this.WrongCount,
            TimeRemaining = this.TimeRemaining,
            StartedAt = this.StartedAt
        };
    }
}
