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
    private bool _isClickable;

    public char Letter
    {
        get => _letter;
        set => SetProperty(ref _letter, value);
    }

    public LetterGuessState State
    {
        get => _state;
        set
        {
            if (SetProperty(ref _state, value))
            {
                IsClickable = value == LetterGuessState.Available;
            }
        }
    }

    public bool IsClickable
    {
        get => _isClickable;
        set => SetProperty(ref _isClickable, value);
    }

    public LetterTileViewModel(char letter)
    {
        _letter = letter;
        _state = LetterGuessState.Available;
        _isClickable = true;
    }
}
