using System;
using System.IO;

namespace HangmanWpf.Utilities;

/// <summary>
/// Helper class for working with relative and absolute file paths.
/// Critical for ensuring application portability across different machines.
/// 
/// PRINCIPLE: All file paths used in this application should be RELATIVE paths,
/// not absolute paths (like C:\Users\Student\...), so the application works
/// on any computer without modification.
/// 
/// This class ensures all paths are resolved relative to the application's
/// base directory (where the .exe is located in the output folder).
/// </summary>
public static class PathHelpers
{
    /// <summary>
    /// Gets the absolute path by combining AppDomain base directory with relative path.
    /// 
    /// Example:
    /// GetRelativePath("Resources/Data/users.json")
    /// → "C:\Project\HangmanWpf\bin\Debug\net8.0-windows\Resources\Data\users.json"
    /// 
    /// The returned path is still "relative" logically (relative to the .exe location),
    /// but is resolved to an absolute filesystem path that Windows can read.
    /// </summary>
    /// <param name="relativePath">Path relative to application base (e.g., "Resources/Data/users.json")</param>
    /// <returns>Absolute filesystem path</returns>
    public static string GetRelativePath(string relativePath)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDirectory, relativePath);
    }

    /// <summary>
    /// Converts an absolute filesystem path to a path relative to application base.
    /// Useful for storing in JSON (we want portable relative paths, not absolute).
    /// 
    /// Example:
    /// ConvertToRelativePath(@"C:\Project\HangmanWpf\bin\Debug\net8.0-windows\Resources\Images\avatar.jpg")
    /// → "Resources/Images/avatar.jpg"
    /// </summary>
    /// <param name="absolutePath">Full filesystem path</param>
    /// <returns>Path relative to application base</returns>
    public static string ConvertToRelativePath(string absolutePath)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        if (absolutePath.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = absolutePath.Substring(baseDirectory.Length).TrimStart('\\', '/');
            return Path.Combine("Resources", relativePath);
        }

        return absolutePath;
    }

    /// <summary>
    /// Ensures directory exists, creating it if necessary.
    /// Used for creating SavedGames/{UserId}/ etc.
    /// </summary>
    /// <param name="relativePath">Directory path relative to app base</param>
    public static void EnsureDirectory(string relativePath)
    {
        var fullPath = GetRelativePath(relativePath);
        Directory.CreateDirectory(fullPath);
    }

    /// <summary>
    /// Gets the user's SavedGames folder path
    /// Example: "Resources/Data/SavedGames/{userId}/"
    /// </summary>
    public static string GetUserSavesPath(Guid userId)
    {
        return $"Resources/Data/SavedGames/{userId}";
    }

    /// <summary>
    /// Validates that a given relative path exists
    /// </summary>
    public static bool FileExists(string relativePath)
    {
        return File.Exists(GetRelativePath(relativePath));
    }

    /// <summary>
    /// Validates that a directory exists
    /// </summary>
    public static bool DirectoryExists(string relativePath)
    {
        return Directory.Exists(GetRelativePath(relativePath));
    }

    /// <summary>
    /// Gets the full path to a resource file (theme, words, etc.)
    /// </summary>
    public static string GetResourcePath(string resourceType, string fileName)
    {
        return $"Resources/{resourceType}/{fileName}";
    }
}
