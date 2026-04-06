using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views
{
    public partial class SaveLoadDialog : Window
    {
        public SaveLoadDialog()
        {
            InitializeComponent();

            var viewModel = App.ServiceProvider.GetRequiredService<SaveLoadDialogViewModel>();
            DataContext = viewModel;
        }
    }
}
