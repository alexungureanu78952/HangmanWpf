using HangmanWpf.Services.Interfaces;
using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
            }

            // Subscribe to keyboard input
            this.PreviewKeyDown += GameWindow_PreviewKeyDown;
        }

        /// <summary>
        /// Handle keyboard input for letter guessing (A-Z keys)
        /// </summary>
        private void GameWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Handle A-Z key presses
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                char letter = ((char)('A' + (e.Key - Key.A))).ToString()[0];

                // Execute GuessLetterCommand with letter parameter
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
    }
}
