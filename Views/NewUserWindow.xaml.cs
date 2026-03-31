using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Windows;

namespace HangmanWpf.Views
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();

            // Set DataContext to NewUserWindowViewModel from DI container
            var viewModel = App.ServiceProvider.GetRequiredService<NewUserWindowViewModel>();
            DataContext = viewModel;
        }
    }
}
