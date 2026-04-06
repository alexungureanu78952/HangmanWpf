using HangmanWpf.Utilities;
using System;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for AboutWindow
/// Displays student information
/// </summary>
public class AboutWindowViewModel : ViewModelBase
{
    public string StudentName => "Ungureanu Alexandru-Florin";
    public string GroupNumber => "10LF244";
    public string Specialization => "Informatica";
    public string ApplicationVersion => "1.0.0";

    public ICommand CloseCommand { get; }

    public event Action? CloseRequested;

    public AboutWindowViewModel()
    {
        CloseCommand = new RelayCommand(_ => OnClose());
    }

    /// <summary>
    /// Handle close
    /// </summary>
    private void OnClose()
    {
        CloseRequested?.Invoke();
    }
}
