using HangmanWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf.Views
{
    /// <summary>
    /// Interaction logic for SaveLoadDialog.xaml
    /// </summary>
    public partial class SaveLoadDialog : Window
    {
        public SaveLoadDialog()
        {
            InitializeComponent();

            // Set DataContext to SaveLoadDialogViewModel from DI container
            var viewModel = App.ServiceProvider.GetRequiredService<SaveLoadDialogViewModel>();
            DataContext = viewModel;
        }
    }
}
