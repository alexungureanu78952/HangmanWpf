using HangmanWpf.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace HangmanWpf.Services;

public class UiDispatcher : IUiDispatcher
{
    public bool CheckAccess()
    {
        return Application.Current?.Dispatcher?.CheckAccess() ?? true;
    }

    public void Invoke(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (Application.Current?.Dispatcher == null || CheckAccess())
        {
            action();
            return;
        }

        Application.Current.Dispatcher.Invoke(action);
    }

    public Task InvokeAsync(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (Application.Current?.Dispatcher == null || CheckAccess())
        {
            action();
            return Task.CompletedTask;
        }

        return Application.Current.Dispatcher.InvokeAsync(action).Task;
    }
}
