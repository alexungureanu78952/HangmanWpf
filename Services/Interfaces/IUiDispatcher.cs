using System;
using System.Threading.Tasks;

namespace HangmanWpf.Services.Interfaces;

public interface IUiDispatcher
{
    bool CheckAccess();
    void Invoke(Action action);
    Task InvokeAsync(Action action);
}
