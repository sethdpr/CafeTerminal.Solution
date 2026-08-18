using CafeTerminal.Maui.Services;

namespace CafeTerminal.Maui.Views;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly AuthService _authService;

    public MainPage()
    {
        InitializeComponent();

        _apiService = new ApiService();
        _authService = new AuthService();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await _authService.LogoutAsync();

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
}