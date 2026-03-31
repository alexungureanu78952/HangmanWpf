namespace HangmanWpf.Utilities;

using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Base class for all ViewModels.
/// Implements INotifyPropertyChanged for data binding support.
/// Provides helper method SetProperty for convenient property updates.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Raised when a property value changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises PropertyChanged event
    /// </summary>
    /// <param name="propertyName">Name of property that changed</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets a property value and raises PropertyChanged only if value actually changed.
    /// This is a helper method that reduces boilerplate in property setters.
    /// 
    /// Usage:
    /// private string _username = string.Empty;
    /// public string Username
    /// {
    ///     get => _username;
    ///     set => SetProperty(ref _username, value);
    /// }
    /// </summary>
    /// <typeparam name="T">Type of property</typeparam>
    /// <param name="field">Private backing field (passed by ref)</param>
    /// <param name="value">New value to set</param>
    /// <param name="propertyName">Property name (auto-captured via CallerMemberName)</param>
    /// <returns>True if value changed; false if same as before</returns>
    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        // If values are already equal, don't fire event
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        // Set new value and fire PropertyChanged
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
