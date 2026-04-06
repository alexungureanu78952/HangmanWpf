using HangmanWpf.Models;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HangmanWpf.ViewModels;

/// <summary>
/// ViewModel for StatisticsWindow
/// Displays game statistics for all users
/// </summary>
public class StatisticsWindowViewModel : ViewModelBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly IUserService _userService;
    private readonly IWordService _wordService;
    private ObservableCollection<StatisticsRow> _statisticsRows;
    private bool _isLoading;

    public ObservableCollection<StatisticsRow> StatisticsRows
    {
        get => _statisticsRows;
        set => SetProperty(ref _statisticsRows, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadStatisticsCommand { get; }
    public ICommand CloseCommand { get; }

    public event Action? CloseRequested;

    public StatisticsWindowViewModel(
        IStatisticsService statisticsService,
        IUserService userService,
        IWordService wordService)
    {
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _wordService = wordService ?? throw new ArgumentNullException(nameof(wordService));
        _statisticsRows = new ObservableCollection<StatisticsRow>();
        _isLoading = false;

        LoadStatisticsCommand = new AsyncRelayCommand(LoadStatisticsAsync);
        CloseCommand = new RelayCommand(_ => OnClose());

        _statisticsService.StatisticsChanged += OnStatisticsChanged;

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
            var users = await _userService.GetAllUsersAsync();
            var allStats = await _statisticsService.GetAllStatisticsAsync();
            var categories = await _wordService.GetAllCategoriesAsync();

            var rows = users
                .SelectMany(user =>
                {
                    var userStats = allStats.FirstOrDefault(s => s.UserId == user.UserId);

                    return categories.Select(category =>
                    {
                        var categoryStats = userStats?.CategoryStats.FirstOrDefault(c => c.Category == category);
                        int played = categoryStats?.GamesPlayed ?? 0;
                        int won = categoryStats?.GamesWon ?? 0;

                        return new StatisticsRow
                        {
                            Username = user.Username,
                            Category = category,
                            GamesPlayed = played,
                            GamesWon = won
                        };
                    });
                })
                .Where(row => row.GamesPlayed > 0 || row.GamesWon > 0)
                .OrderBy(r => r.Username)
                .ThenBy(r => r.Category)
                .ToList();

            StatisticsRows = new ObservableCollection<StatisticsRow>(rows);
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
        _statisticsService.StatisticsChanged -= OnStatisticsChanged;
        CloseRequested?.Invoke();
    }

    private void OnStatisticsChanged()
    {
        if (Application.Current?.Dispatcher == null)
        {
            _ = LoadStatisticsAsync();
            return;
        }

        if (Application.Current.Dispatcher.CheckAccess())
        {
            _ = LoadStatisticsAsync();
        }
        else
        {
            Application.Current.Dispatcher.InvokeAsync(() => LoadStatisticsAsync());
        }
    }
}

/// <summary>
/// One display row in statistics grid: User + Category totals.
/// </summary>
public class StatisticsRow
{
    public string Username { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
}
