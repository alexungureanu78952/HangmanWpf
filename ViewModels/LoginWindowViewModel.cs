using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for LoginWindow
/// Manages user selection, creation, and deletion
/// </summary>
public class LoginWindowViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private ObservableCollection<User> _users;
    private User? _selectedUser;
    private bool _isLoading;

    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            SetProperty(ref _selectedUser, value);
            ((RelayCommand)SelectUserCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteUserCommand).RaiseCanExecuteChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadUsersCommand { get; }
    public ICommand SelectUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand CreateUserCommand { get; }

    /// <summary>
    /// Event fired when user selects a user and clicks Play
    /// </summary>
    public event Action<User>? UserSelected;

    /// <summary>
    /// Event fired when user wants to create a new user
    /// </summary>
    public event Action? NewUserRequested;

    public LoginWindowViewModel(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _users = new ObservableCollection<User>();
        _selectedUser = null;
        _isLoading = false;

        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        SelectUserCommand = new RelayCommand(_ => OnUserSelected(), _ => SelectedUser != null);
        DeleteUserCommand = new RelayCommand(_ => OnDeleteUser(), _ => SelectedUser != null);
        CreateUserCommand = new RelayCommand(_ => OnCreateUser());

        // Load users on ViewModel creation
        _ = LoadUsersAsync();
    }

    /// <summary>
    /// Load all users from persistence
    /// </summary>
    private async System.Threading.Tasks.Task LoadUsersAsync()
    {
        IsLoading = true;
        try
        {
            var users = await _userService.GetAllUsersAsync();
            Users = new ObservableCollection<User>(users);
            SelectedUser = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Handle user selection and Play button click
    /// </summary>
    private void OnUserSelected()
    {
        if (SelectedUser != null)
        {
            UserSelected?.Invoke(SelectedUser);
        }
    }

    /// <summary>
    /// Handle delete user request
    /// </summary>
    private async void OnDeleteUser()
    {
        if (SelectedUser == null)
            return;

        var userToDelete = SelectedUser;
        try
        {
            await _userService.DeleteUserAsync(userToDelete.UserId);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting user: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle create new user request
    /// </summary>
    private void OnCreateUser()
    {
        NewUserRequested?.Invoke();
    }
}
