using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HangmanWpf.Services;

/// <summary>
/// Service for managing user persistence (users.json).
/// Implements cascade deletion (user + stats + saved games).
/// </summary>
public class UserService : IUserService
{
    private const string UsersFilePath = "Resources/Data/users.json";
    private const string UserImagesPath = "Resources/Data/UserImages";
    private readonly IStatisticsService _statisticsService;
    private readonly IGamePersistenceService _gamePersistenceService;

    public UserService(IStatisticsService statisticsService, IGamePersistenceService gamePersistenceService)
    {
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _gamePersistenceService = gamePersistenceService ?? throw new ArgumentNullException(nameof(gamePersistenceService));
    }

    /// <summary>
    /// Create a new user and persist to users.json
    /// </summary>
    public async Task<User> CreateUserAsync(string username, string imagePath)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (username.Any(char.IsWhiteSpace))
            throw new ArgumentException("Username must be a single word", nameof(username));

        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Image path cannot be empty", nameof(imagePath));

        var persistedImagePath = CopyImageToDataFolder(imagePath);

        var user = new User(username, persistedImagePath);
        var users = await GetAllUsersAsync();

        // Check duplicate username
        if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"User '{username}' already exists");

        users.Add(user);
        await SaveUsersAsync(users);

        return user;
    }

    private string CopyImageToDataFolder(string imagePath)
    {
        var extension = Path.GetExtension(imagePath).ToLowerInvariant();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".gif" && extension != ".png")
            throw new ArgumentException("Supported image types are jpg, jpeg, gif, png", nameof(imagePath));

        var sourcePath = Path.IsPathRooted(imagePath)
            ? imagePath
            : PathHelpers.GetRelativePath(imagePath);

        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Selected image file does not exist", sourcePath);

        var relativeTargetDir = UserImagesPath;
        var absoluteTargetDir = PathHelpers.GetRelativePath(relativeTargetDir);
        if (!Directory.Exists(absoluteTargetDir))
            Directory.CreateDirectory(absoluteTargetDir);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var absoluteTargetPath = Path.Combine(absoluteTargetDir, fileName);
        File.Copy(sourcePath, absoluteTargetPath, overwrite: true);

        return $"{relativeTargetDir}/{fileName}".Replace('\\', '/');
    }

    /// <summary>
    /// Get all users from users.json
    /// </summary>
    public async Task<List<User>> GetAllUsersAsync()
    {
        var filePath = PathHelpers.GetRelativePath(UsersFilePath);

        try
        {
            if (!File.Exists(filePath))
            {
                // Create empty file if it doesn't exist
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(new List<User>(), Formatting.Indented));
                return new List<User>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            var users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            return users;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading users.json: {ex.Message}");
            return new List<User>();
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        var users = await GetAllUsersAsync();
        return users.FirstOrDefault(u => u.UserId == userId);
    }

    /// <summary>
    /// Delete a user with cascade deletion:
    /// 1. Remove from users.json
    /// 2. Delete from statistics.json
    /// 3. Delete saved games folder
    /// </summary>
    public async Task DeleteUserAsync(Guid userId)
    {
        var users = await GetAllUsersAsync();
        var userToDelete = users.FirstOrDefault(u => u.UserId == userId);

        if (userToDelete == null)
            return;

        // Remove user from list
        users.Remove(userToDelete);
        await SaveUsersAsync(users);

        // Cascade: delete statistics
        await _statisticsService.DeleteUserStatisticsAsync(userId);

        // Cascade: delete saved games
        await _gamePersistenceService.DeleteAllUserGamesAsync(userId);
    }

    /// <summary>
    /// Check if user exists
    /// </summary>
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        return user != null;
    }

    /// <summary>
    /// Internal: Save users list to JSON
    /// </summary>
    private async Task SaveUsersAsync(List<User> users)
    {
        var filePath = PathHelpers.GetRelativePath(UsersFilePath);
        var json = JsonConvert.SerializeObject(users, Formatting.Indented);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, json);
    }
}
