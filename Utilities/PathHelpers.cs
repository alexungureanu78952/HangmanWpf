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

}