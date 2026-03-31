using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for StatisticsWindow
/// Displays game statistics for all users
/// </summary>
public class StatisticsWindowViewModel : ViewModelBase
{
    private readonly IStatisticsService _statisticsService;
    private ObservableCollection<Statistics> _allStatistics;
    private bool _isLoading;

    public ObservableCollection<Statistics> AllStatistics
    {
        get => _allStatistics;
        set => SetProperty(ref _allStatistics, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadStatisticsCommand { get; }
    public ICommand CloseCommand { get; }

    public event Action? CloseRequested;

    public StatisticsWindowViewModel(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _allStatistics = new ObservableCollection<Statistics>();
        _isLoading = false;

        LoadStatisticsCommand = new AsyncRelayCommand(LoadStatisticsAsync);
        CloseCommand = new RelayCommand(_ => OnClose());

        _ = LoadStatisticsAsync();
    }

    /// <summary>
    /// Load all statistics from persistence
    /// </summary>
    private async System.Threading.Tasks.Task LoadStatisticsAsync()
    {
        IsLoading = true;
        try
        {
            var stats = await _statisticsService.GetAllStatisticsAsync();
            AllStatistics = new ObservableCollection<Statistics>(stats);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Handle close
    /// </summary>
    private void OnClose()
    {
        CloseRequested?.Invoke();
    }
}
