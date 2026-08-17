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

    private async void OnTestApiClicked(object sender, EventArgs e)
    {
        try
        {
            var response = await _apiService.GetAsync("/api/Main/test");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                await DisplayAlert(
                    "API test",
                    result,
                    "OK");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await DisplayAlert(
                    "Niet ingelogd",
                    "De API heeft de JWT geweigerd.",
                    "OK");
            }
            else
            {
                await DisplayAlert(
                    "API fout",
                    $"Statuscode: {(int)response.StatusCode}",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Fout",
                $"Er kon geen verbinding worden gemaakt met de API: {ex.Message}",
                "OK");
        }
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