using System;
using HangmanWpf.Services;
using HangmanWpf.Services.Interfaces;
using HangmanWpf.ViewModels;
using HangmanWpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HangmanWpf;

/// <summary>
/// Interaction logic for App
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        InitializeComponent();
    }

    /// <summary>
    /// Configure dependency injection container
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // Register services in dependency order
        // StatisticsService and GamePersistenceService have no dependencies
        services.AddSingleton<IStatisticsService, StatisticsService>();
        services.AddSingleton<IGamePersistenceService, GamePersistenceService>();

        // UserService depends on StatisticsService and GamePersistenceService
        services.AddSingleton<IUserService, UserService>();

        // WordService and ThemeService are independent
        services.AddSingleton<IWordService, WordService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IGameDialogService, GameDialogService>();
        services.AddSingleton<IUiDispatcher, UiDispatcher>();

        // GameService depends on WordService
        services.AddSingleton<IGameService, GameService>();

        // Register ViewModels
        services.AddSingleton<LoginWindowViewModel>();
        services.AddSingleton<GameWindowViewModel>();
        services.AddSingleton<NewUserWindowViewModel>();
        services.AddSingleton<StatisticsWindowViewModel>();
        services.AddSingleton<SaveLoadDialogViewModel>();
        services.AddSingleton<AboutWindowViewModel>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Apply default theme on startup
        try
        {
            var themeService = ServiceProvider.GetRequiredService<IThemeService>();
            await themeService.ApplyThemeAsync("DarkPurple");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying theme on startup: {ex.Message}");
        }
    }
}
