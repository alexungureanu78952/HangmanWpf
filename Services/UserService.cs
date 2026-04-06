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


    public async Task<List<User>> GetAllUsersAsync()
    {
        var filePath = PathHelpers.GetRelativePath(UsersFilePath);

        try
        {
            if (!File.Exists(filePath))
            {

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


    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        var users = await GetAllUsersAsync();
        return users.FirstOrDefault(u => u.UserId == userId);
    }


    public async Task DeleteUserAsync(Guid userId)
    {
        var users = await GetAllUsersAsync();
        var userToDelete = users.FirstOrDefault(u => u.UserId == userId);

        if (userToDelete == null)
            return;


        users.Remove(userToDelete);
        await SaveUsersAsync(users);


        await _statisticsService.DeleteUserStatisticsAsync(userId);


        await _gamePersistenceService.DeleteAllUserGamesAsync(userId);
    }


    public async Task<bool> UserExistsAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        return user != null;
    }


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
