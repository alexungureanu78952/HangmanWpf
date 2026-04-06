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

    Task<User> CreateUserAsync(string username, string imagePath);


    Task<List<User>> GetAllUsersAsync();


    Task<User?> GetUserByIdAsync(Guid userId);


    Task DeleteUserAsync(Guid userId);


    Task<bool> UserExistsAsync(Guid userId);
}
