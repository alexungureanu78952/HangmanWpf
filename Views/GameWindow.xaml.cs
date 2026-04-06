using HangmanWpf.Services.Interfaces;
using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Input;

namespace HangmanWpf.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private GameWindowViewModel? _gameVm;

        public GameWindow()
        {
            InitializeComponent();

            // Set DataContext to GameWindowViewModel from DI container
            _gameVm = App.ServiceProvider.GetRequiredService<GameWindowViewModel>();
            DataContext = _gameVm;

            // Subscribe to close events
            if (_gameVm != null)
            {
                _gameVm.GameClosed += OnGameClosed;
                _gameVm.OpenGameRequested += OnOpenGameRequested;
            }

            // Subscribe to keyboard input
            this.PreviewKeyDown += GameWindow_PreviewKeyDown;
        }

        /// <summary>
        /// Handle keyboard input for letter guessing (A-Z keys)
        /// </summary>
        private void GameWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not GameWindowViewModel vm)
            {
                return;
            }

            // Support menu actions from keyboard while keeping MVVM command execution.
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.N)
                {
                    if (vm.StartNewGameCommand.CanExecute(null))
                    {
                        vm.StartNewGameCommand.Execute(null);
                        e.Handled = true;
                    }

                    return;
                }

                if (e.Key == Key.O)
                {
                    if (vm.LoadGameCommand.CanExecute(null))
                    {
                        vm.LoadGameCommand.Execute(null);
                        e.Handled = true;
                    }

                    return;
                }

                if (e.Key == Key.S)
                {
                    if (vm.SaveGameCommand.CanExecute(null))
                    {
                        vm.SaveGameCommand.Execute(null);
                        e.Handled = true;
                    }

                    return;
                }

                if (e.Key == Key.Q)
                {
                    if (vm.CancelCommand.CanExecute(null))
                    {
                        vm.CancelCommand.Execute(null);
                        e.Handled = true;
                    }

                    return;
                }
            }

            if (e.Key == Key.F1)
            {
                OnAboutMenuClick(this, new RoutedEventArgs());
                e.Handled = true;

                return;
            }

            // Handle A-Z key presses.
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                char letter = (char)('A' + (e.Key - Key.A));
                if (_gameVm?.GuessLetterCommand.CanExecute(letter) == true)
                {
                    _gameVm.GuessLetterCommand.Execute(letter);
                    e.Handled = true; // Mark event as handled
                }
            }
        }

        /// <summary>
        /// Handle game window closing
        /// </summary>
        private void OnGameClosed()
        {
            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            // Unsubscribe from events to prevent memory leaks
            if (_gameVm != null)
            {
                _gameVm.GameClosed -= OnGameClosed;
                _gameVm.OpenGameRequested -= OnOpenGameRequested;
            }

            this.PreviewKeyDown -= GameWindow_PreviewKeyDown;
        }

        /// <summary>
        /// Handle About menu click
        /// </summary>
        private void OnAboutMenuClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        /// <summary>
        /// Handle Dark Purple theme
        /// </summary>
        private async void OnThemePurpleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_gameVm != null)
            {
                var themeService = App.ServiceProvider.GetRequiredService<IThemeService>();
                await themeService.ApplyThemeAsync("DarkPurple");
            }
        }

        /// <summary>
        /// Handle Dark Red theme
        /// </summary>
        private async void OnThemeRedClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_gameVm != null)
            {
                var themeService = App.ServiceProvider.GetRequiredService<IThemeService>();
                await themeService.ApplyThemeAsync("DarkRed");
            }
        }

        /// <summary>
        /// Handle Statistics menu click
        /// </summary>
        private void OnStatisticsMenuClick(object sender, RoutedEventArgs e)
        {
            var statisticsWindow = new StatisticsWindow
            {
                Owner = this
            };
            statisticsWindow.ShowDialog();
        }

        private async void OnOpenGameRequested()
        {
            if (_gameVm == null || _gameVm.CurrentUser.UserId == Guid.Empty)
            {
                return;
            }

            var dialog = new SaveLoadDialog
            {
                Owner = this
            };

            var dialogVm = (SaveLoadDialogViewModel)dialog.DataContext;
            dialogVm.Initialize(_gameVm.CurrentUser.UserId);

            dialogVm.GameLoaded += async savedGame =>
            {
                await _gameVm.RestoreSavedGameAsync(savedGame);
                dialog.Close();
            };

            dialogVm.CancellationRequested += () => dialog.Close();

            dialog.ShowDialog();
        }
    }
}
