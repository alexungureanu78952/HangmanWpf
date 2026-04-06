using System;
using System.Collections.Generic;
using System.Linq;

namespace HangmanWpf.Models;

/// <summary>
/// Represents the current state of a hangman game session.
/// This is the runtime state during gameplay (not persisted unless saved).
/// </summary>
public class GameSession
{

    public string Word { get; set; } = string.Empty;


    public string Category { get; set; } = string.Empty;


    public List<char> GuessedLetters { get; set; } = new();

    /// <summary>
    /// Current level (0-3 range, where 3 = game won)
    /// Level = number of consecutive words correctly guessed
    /// Resets to 0 if a word is lost
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Number of incorrect guesses made (0-6)
    /// 6 incorrect guesses ends the current word
    /// </summary>
    public int WrongCount { get; set; }

    /// <summary>
    /// Time remaining for current word (in seconds)
    /// Starts at 30, counts down each second
    /// When reaches 0 → word is lost
    /// </summary>
    public int TimeRemaining { get; set; } = 30;


    public DateTime StartedAt { get; set; }

    public GameSession()
    {
        StartedAt = DateTime.UtcNow;
    }


    public string GetWordDisplay()
    {
        return string.Concat(Word.Select(letter =>
            GuessedLetters.Contains(letter) ? letter : '_'));
    }


    public bool HasGuessedLetter(char letter)
    {
        return GuessedLetters.Contains(char.ToUpper(letter));
    }


    public bool IsWordComplete()
    {
        return Word.All(letter => GuessedLetters.Contains(letter));
    }
}
