using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Maui.Controls;
using CafeTerminal.Maui.Services;

namespace CafeTerminal.Maui.Views;

// This page shows the table overview and opens the management dialog for one table.
public partial class TablesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly AuthService _auth_service;
    private bool _isTableDialogOpen;

    public ObservableCollection<TableItem> TableItems { get; } = new(Enumerable.Range(1, 10).Select(i => new TableItem { Number = i }));

    public TablesPage()
    {
        InitializeComponent();

        // Resolve services from MAUI DI (if available)
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _apiService = services?.GetService<ApiService>() ?? new ApiService(new HttpClient { BaseAddress = new Uri("https://localhost:7232") });
        _auth_service = services?.GetService<AuthService>() ?? new AuthService();

        BindingContext = this;
    }

    // Track last tap timestamps per table to detect double-clicks
    private readonly Dictionary<int, DateTime> _lastTapTimes = new();

    // Logs the user out and returns to the login shell.
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await _auth_service.LogoutAsync();

        // Ensure the AppShell rebuilds its items (show login/register) before navigating.
        if (Shell.Current is AppShell appShell)
        {
            await appShell.ShowLoggedOutAndNavigateToLoginAsync();
        }
        else
        {
            await Shell.Current.GoToAsync("///login");
        }
    }

    // Detects a double tap on a table tile and opens the table dialog.
    private async void OnTableTapped(object? sender, TappedEventArgs e)
    {
        if (sender is VisualElement ve && ve.BindingContext is TableItem ti)
        {
            var now = DateTime.UtcNow;
            if (_lastTapTimes.TryGetValue(ti.Number, out var last) && (now - last).TotalMilliseconds <= 500)
            {
                // Double-click detected
                _lastTapTimes.Remove(ti.Number);

                if (_isTableDialogOpen || Navigation.ModalStack.LastOrDefault() is NavigationPage)
                {
                    return;
                }

                _isTableDialogOpen = true;
                var dialog = new NavigationPage(new TableDialogPage(ti));
                await Navigation.PushModalAsync(dialog);
                return;
            }

            // Record this tap time
            _lastTapTimes[ti.Number] = now;
        }
    }

    // Reloads the tables from the API whenever the page becomes visible.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _isTableDialogOpen = false;

        if (_apiService != null)
        {
            try
            {
                var tables = await _apiService.GetTablesAsync();
                // Update TableItems collection
                TableItems.Clear();
                foreach (var t in tables)
                {
                    TableItems.Add(new TableItem { Number = t.Number, Name = t.Name });
                }
            }
            catch
            {
                // ignore for now
            }
        }
    }

    // This helper model represents one tile in the tables overview.
    public class TableItem : INotifyPropertyChanged
    {
        private string _name = string.Empty;

        public int Number { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayText)));
                }
            }
        }

        public string DisplayText => string.IsNullOrWhiteSpace(Name) ? Number.ToString() : $"{Number} - {Name}";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
