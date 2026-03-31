using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            // Set DataContext to AboutWindowViewModel from DI container
            var viewModel = App.ServiceProvider.GetRequiredService<AboutWindowViewModel>();
            DataContext = viewModel;
        }
    }
}
