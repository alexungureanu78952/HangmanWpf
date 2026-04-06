using HangmanWpf.Models;
using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();

        var viewModel = App.ServiceProvider.GetRequiredService<LoginWindowViewModel>();
        DataContext = viewModel;

        viewModel.UserSelected += OnUserSelected;
        viewModel.NewUserRequested += OnNewUserRequested;
    }

    private void OnUserSelected(User selectedUser)
    {
        var gameVm = App.ServiceProvider.GetRequiredService<GameWindowViewModel>();
        gameVm.Initialize(selectedUser);

        var gameWindow = new GameWindow
        {
            Owner = this
        };

        gameWindow.Closed += (_, _) =>
        {
            Show();
            Activate();
            ((LoginWindowViewModel)DataContext).LoadUsersCommand.Execute(null);
        };

        Hide();
        gameWindow.Show();
    }

    private void OnNewUserRequested()
    {
        var newUserWindow = new NewUserWindow();
        var newUserVm = (NewUserWindowViewModel)newUserWindow.DataContext;

        newUserVm.UserCreated += (user) =>
        {
            newUserWindow.Close();
            ((LoginWindowViewModel)DataContext).LoadUsersCommand.Execute(null);
        };

        newUserWindow.ShowDialog();
    }
}
