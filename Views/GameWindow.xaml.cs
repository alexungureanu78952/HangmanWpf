using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace HangmanWpf.Views
{

    public partial class GameWindow : Window
    {
        private GameWindowViewModel? _gameVm;

        public GameWindow()
        {
            InitializeComponent();


            _gameVm = App.ServiceProvider.GetRequiredService<GameWindowViewModel>();
            DataContext = _gameVm;


            if (_gameVm != null)
            {
                _gameVm.GameClosed += OnGameClosed;
            }

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
                if (vm.ShowAboutCommand.CanExecute(null))
                {
                    vm.ShowAboutCommand.Execute(null);
                    e.Handled = true;
                }

                return;
            }

            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                char letter = (char)('A' + (e.Key - Key.A));
                if (_gameVm?.GuessLetterCommand.CanExecute(letter) == true)
                {
                    _gameVm.GuessLetterCommand.Execute(letter);
                    e.Handled = true;
                }
            }
        }

        private void OnGameClosed()
        {
            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            if (_gameVm != null)
            {
                _gameVm.GameClosed -= OnGameClosed;
            }

            this.PreviewKeyDown -= GameWindow_PreviewKeyDown;
        }
    }
}
