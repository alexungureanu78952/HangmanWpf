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
    public string StudentName => "Student Name";
    public string GroupNumber => "Group Number";
    public string Specialization => "Specialization";
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
