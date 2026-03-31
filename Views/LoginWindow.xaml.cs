using HangmanWpf.Models;
using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// IMPORTANT: This code-behind should contain ONLY binding initialization and window management.
/// All business logic must be in the ViewModel!
/// </summary>
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();

        // Set DataContext to LoginWindowViewModel from DI container
        var viewModel = App.ServiceProvider.GetRequiredService<LoginWindowViewModel>();
        DataContext = viewModel;

        // Subscribe to ViewModel events for window navigation
        viewModel.UserSelected += OnUserSelected;
        viewModel.NewUserRequested += OnNewUserRequested;
    }

    /// <summary>
    /// Handle user selection - open GameWindow
    /// </summary>
    private void OnUserSelected(User selectedUser)
    {
        // Get GameWindowViewModel from DI and initialize with selected user
        var gameVm = App.ServiceProvider.GetRequiredService<GameWindowViewModel>();
        gameVm.Initialize(selectedUser);

        // Create and show GameWindow
        var gameWindow = new GameWindow();
        gameWindow.Show();

        // Optionally hide login window while playing
        // this.Hide();
    }

    /// <summary>
    /// Handle new user request - open NewUserWindow modal
    /// </summary>
    private void OnNewUserRequested()
    {
        var newUserWindow = new NewUserWindow();
        var newUserVm = (NewUserWindowViewModel)newUserWindow.DataContext;

        // Subscribe to completion event
        newUserVm.UserCreated += (user) =>
        {
            newUserWindow.Close();
            // Refresh user list on return
            ((LoginWindowViewModel)DataContext).LoadUsersCommand.Execute(null);
        };

        // Show as modal
        newUserWindow.ShowDialog();
    }
}
