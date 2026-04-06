using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for NewUserWindow
/// Manages user creation
/// </summary>
public class NewUserWindowViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private string _username;
    private string _imagePath;
    private bool _isLoading;
    private string _statusMessage;

    public string Username
    {
        get => _username;
        set
        {
            if (SetProperty(ref _username, value))
            {
                UpdateCreateCommandState();
            }
        }
    }

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            if (SetProperty(ref _imagePath, value))
            {
                UpdateCreateCommandState();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand BrowseImageCommand { get; }
    public ICommand CreateUserCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<User>? UserCreated;
    public event Action? CancellationRequested;
    public event Action? BrowseImageRequested;

    public NewUserWindowViewModel(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _username = string.Empty;
        _imagePath = string.Empty;
        _isLoading = false;
        _statusMessage = "Enter username and select an image.";

        BrowseImageCommand = new RelayCommand(_ => OnBrowseImage());
        CreateUserCommand = new AsyncRelayCommand(CreateUserAsync, () => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(ImagePath));
        CancelCommand = new RelayCommand(_ => OnCancel());

        UpdateCreateCommandState();
    }

    /// <summary>
    /// Set image path (called from View code-behind after dialog)
    /// </summary>
    public void SetImagePath(string imagePath)
    {
        ImagePath = imagePath;
        StatusMessage = "Image selected.";
        UpdateCreateCommandState();
    }

    /// <summary>
    /// Browse for image file
    /// </summary>
    private void OnBrowseImage()
    {
        BrowseImageRequested?.Invoke();
    }

    private void UpdateCreateCommandState()
    {
        if (CreateUserCommand is AsyncRelayCommand asyncCommand)
        {
            asyncCommand.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Create new user
    /// </summary>
    private async System.Threading.Tasks.Task CreateUserAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            StatusMessage = "Username cannot be empty.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ImagePath))
        {
            StatusMessage = "Image must be selected.";
            return;
        }

        IsLoading = true;
        try
        {
            var user = await _userService.CreateUserAsync(Username, ImagePath);
            StatusMessage = "User created successfully!";
            UserCreated?.Invoke(user);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating user: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Handle cancel
    /// </summary>
    private void OnCancel()
    {
        CancellationRequested?.Invoke();
    }
}
