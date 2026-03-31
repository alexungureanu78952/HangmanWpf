using HangmanWpf.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

/// <summary>
/// Service for managing user persistence (CRUD operations on users.json)
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Create a new user and persist to JSON
    /// </summary>
    Task<User> CreateUserAsync(string username, string imagePath);

    /// <summary>
    /// Get all users from persistence
    /// </summary>
    Task<List<User>> GetAllUsersAsync();

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<User?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Delete a user (cascade delete associated stats and saves)
    /// </summary>
    Task DeleteUserAsync(Guid userId);

    /// <summary>
    /// Check if user exists
    /// </summary>
    Task<bool> UserExistsAsync(Guid userId);
}
