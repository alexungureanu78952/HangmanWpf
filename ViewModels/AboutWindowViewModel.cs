using HangmanWpf.Utilities;
using System;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;


public class AboutWindowViewModel : ViewModelBase
{
    public string StudentName => "Ungureanu Alexandru-Florin";
    public string GroupNumber => "10LF244";
    public string Specialization => "Informatica";

    public ICommand CloseCommand { get; }

    public event Action? CloseRequested;

    public AboutWindowViewModel()
    {
        CloseCommand = new RelayCommand(_ => OnClose());
    }
    private void OnClose()
    {
        CloseRequested?.Invoke();
    }
}
