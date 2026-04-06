using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();

            var viewModel = App.ServiceProvider.GetRequiredService<StatisticsWindowViewModel>();
            DataContext = viewModel;
            viewModel.CloseRequested += () => Close();
        }
    }
}
