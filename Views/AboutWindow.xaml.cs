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


            var viewModel = App.ServiceProvider.GetRequiredService<AboutWindowViewModel>();
            DataContext = viewModel;
            DataContextChanged += OnDataContextChanged;
            Closed += OnClosed;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AboutWindowViewModel oldVm)
            {
                oldVm.CloseRequested -= HandleCloseRequested;
            }

            if (e.NewValue is AboutWindowViewModel newVm)
            {
                newVm.CloseRequested += HandleCloseRequested;
            }
        }

        private void OnClosed(object? sender, System.EventArgs e)
        {
            if (DataContext is AboutWindowViewModel vm)
            {
                vm.CloseRequested -= HandleCloseRequested;
            }
        }

        private void HandleCloseRequested()
        {
            Close();
        }
    }
}
