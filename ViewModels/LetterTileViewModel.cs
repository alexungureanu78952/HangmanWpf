using System;
using HangmanWpf.Utilities;

namespace HangmanWpf.ViewModels;

public enum LetterGuessState
{
    Available,
    Correct,
    Wrong
}

public class LetterTileViewModel : ViewModelBase
{
    private char _letter;
    private LetterGuessState _state;

    public char Letter
    {
        get => _letter;
        set => SetProperty(ref _letter, value);
    }

    public LetterGuessState State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }

    public LetterTileViewModel(char letter)
    {
        _letter = letter;
        _state = LetterGuessState.Available;
    }
}
