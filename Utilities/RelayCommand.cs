using System;
using System.Windows.Input;

namespace HangmanWpf.Utilities;

/// <summary>
/// A generic implementation of ICommand that wraps an Action and optional predicate.
/// Used by ViewModels to expose commands bindable from XAML.
/// 
/// Example usage in ViewModel:
/// public ICommand GuessLetterCommand { get; }
/// 
/// public MyViewModel()
/// {
///     GuessLetterCommand = new RelayCommand(
///         execute: param => GuessLetter(param.ToString()),
///         canExecute: param => !gameEnded
///     );
/// }
/// 
/// In XAML:
/// &lt;Button Command="{Binding GuessLetterCommand}" CommandParameter="A" Text="A" /&gt;
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    /// <summary>
    /// Raised when CanExecute result changes (button enabled/disabled)
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Creates a new command
    /// </summary>
    /// <param name="execute">Action to execute when command runs</param>
    /// <param name="canExecute">Optional predicate to determine if command can execute</param>
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        if (execute == null)
            throw new ArgumentNullException(nameof(execute));

        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Determines if command can execute (used to enable/disable button)
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    /// Executes the command action
    /// </summary>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Manually trigger CanExecuteChanged event
    /// Call this when the can-execute condition changes
    /// 
    /// Example:
    /// gameEnded = false;
    /// guessCommand.RaiseCanExecuteChanged(); // Button becomes enabled
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
