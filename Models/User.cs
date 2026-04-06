using System;

namespace HangmanWpf.Models;

/// <summary>
/// Represents a user in the application.
/// This model is serialized to/from JSON in users.json
/// </summary>
public class User
{

    public Guid UserId { get; set; }


    public string Username { get; set; } = string.Empty;


    public string ImagePath { get; set; } = string.Empty;


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
