using CafeTerminal.Shared.DTOs;
using System.Text.Json;

namespace CafeTerminal.Maui.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            //read fields from UI
            var email = EmailEntry.Text;
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            //front-end troubleshoot
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Fout", "Gelieve alle velden in te vullen", "OK");
                return;
            }

            //create request object
            var request = new RegisterRequest
            {
                Email = email,
                Username = username,
                Password = password
            };

            //convert to JSON. API expects JSON in POST-body
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(
                json,
                System.Text.Encoding.UTF8,
                "application/json");

            //send POST to API
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7232");

            var response = await client.PostAsync(
                "/api/Auth/register",
                content);

            if (response.IsSuccessStatusCode)
            {
                //read the response from the API
                var responseJson = await response.Content.ReadAsStringAsync();

                //convert the JSON response into an AuthResponse object
                var authResponse =
                    JsonSerializer.Deserialize<AuthResponse>(
                        responseJson,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (authResponse == null || string.IsNullOrWhiteSpace(authResponse.Token))
                {
                    await DisplayAlert(
                        "Fout",
                        "De registratie is gelukt, maar er werd geen JWT-token ontvangen.",
                        "OK");

                    return;
                }

                //store the JWT token securely on the device
                await SecureStorage.Default.SetAsync(
                    "auth_token",
                    authResponse.Token);

                await DisplayAlert(
                    "Succes",
                    "Gebruiker succesvol geregistreerd",
                    "OK");

                // Rebuild the shell items for the logged-in state before navigating
                if (Shell.Current is AppShell appShell)
                {
                    await appShell.ShowLoggedInAndNavigateToMainAsync();
                }
                else
                {
                    await Shell.Current.GoToAsync("///main");
                }

                /*We use "///" to navigate between Shell-elements.
                 This is called 'absolute routing'. Different from 'relative routing'.
                 Relative routing can be described as taking stairs to navigate through the app.
                 With absolute routing, we can teleport between different "rooms" in the app.
                 Like going from "/Home/Menu" to "/Account/Settings" in 1 move, for example*/
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert(
                    "Fout",
                    $"Registratie mislukt: {error}",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Fout",
                $"Registratie mislukt: {ex.Message}",
                "OK");
        }
    }
}