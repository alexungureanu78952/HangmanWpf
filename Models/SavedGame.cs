using System;
using System.Collections.Generic;

namespace HangmanWpf.Models;

/// <summary>
/// Represents a saved game snapshot that can be persisted to JSON.
/// Extends GameSession with metadata needed for save/load functionality.
/// </summary>
public class SavedGame : GameSession
{

    public Guid SavedGameId { get; set; }


    public Guid UserId { get; set; }


    public string SaveName { get; set; } = string.Empty;


    public DateTime SavedAt { get; set; }

    public SavedGame() : base()
    {
        SavedGameId = Guid.NewGuid();
        SavedAt = DateTime.UtcNow;
    }


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
