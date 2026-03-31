using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views
{
    /// <summary>
    /// Interaction logic for StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();

            // Set DataContext to StatisticsWindowViewModel from DI container
            var viewModel = App.ServiceProvider.GetRequiredService<StatisticsWindowViewModel>();
            DataContext = viewModel;
        }
    }
}
