using System;

namespace HangmanWpf.Models;

/// <summary>
/// Represents a user in the application.
/// This model is serialized to/from JSON in users.json
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user (GUID)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Username (single word, no spaces)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Relative path to user's avatar image
    /// Example: "Resources/Images/Avatar1.jpg" (never absolute paths)
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when user was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    public User()
    {
        UserId = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public User(string username, string imagePath) : this()
    {
        Username = username;
        ImagePath = imagePath;
    }
}
