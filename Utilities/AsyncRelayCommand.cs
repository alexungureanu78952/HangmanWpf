using System;
using System.Windows.Input;
using System.Threading.Tasks;

namespace HangmanWpf.Utilities;

/// <summary>
/// An async-capable version of ICommand that wraps a Task-returning Action.
/// Used for ViewModels commands that need to perform asynchronous operations
/// like file I/O without blocking the UI thread.
/// 
/// Example usage in ViewModel:
/// public ICommand SaveGameCommand { get; }
/// 
/// public GameWindowViewModel(IGamePersistenceService persistenceService)
/// {
///     SaveGameCommand = new AsyncRelayCommand(
///         execute: async () => await persistenceService.SaveGameAsync(currentGame),
///         canExecute: () => gameInProgress
///     );
/// }
/// 
/// In XAML:
/// &lt;Button Command="{Binding SaveGameCommand}" Content="Save Game" /&gt;
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    /// <summary>
    /// Raised when CanExecute result changes
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Creates a new async command
    /// </summary>
    /// <param name="execute">Async action to execute</param>
    /// <param name="canExecute">Optional predicate to determine if can execute</param>
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Determines if command can execute
    /// Returns false while command is already executing (prevents double clicks)
    /// </summary>
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    /// <summary>
    /// Executes the async command
    /// Sets _isExecuting flag to prevent concurrent executions
    /// </summary>
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            await _execute();
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Manually trigger CanExecuteChanged event
    /// Call this when the can-execute condition changes
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// Generic async command that accepts a parameter.
/// Used when command needs to receive runtime parameter from XAML.
/// </summary>
public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;

    public AsyncRelayCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke((T?)parameter) ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            await _execute((T?)parameter);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
