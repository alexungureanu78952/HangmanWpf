using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Windows;

namespace HangmanWpf.Views
{
    public partial class NewUserWindow : Window
    {
        private readonly NewUserWindowViewModel _viewModel;

        public NewUserWindow()
        {
            InitializeComponent();

            _viewModel = App.ServiceProvider.GetRequiredService<NewUserWindowViewModel>();
            DataContext = _viewModel;

            _viewModel.BrowseImageRequested += OnBrowseImageRequested;
            Closed += OnClosed;
        }

        private void OnBrowseImageRequested()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select Avatar Image",
                    Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dialog.ShowDialog(this) == true)
                {
                    _viewModel.SetImagePath(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage = $"Error browsing: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"BrowseImage Error: {ex}");
            }
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            _viewModel.BrowseImageRequested -= OnBrowseImageRequested;
            Closed -= OnClosed;
        }
    }
}
