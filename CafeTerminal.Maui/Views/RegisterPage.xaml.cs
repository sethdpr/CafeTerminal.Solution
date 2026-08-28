using CafeTerminal.Shared.DTOs;
using CafeTerminal.Shared.DTOs;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace CafeTerminal.Maui.Views;

// This page handles the registration flow for new users.
public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    // Validates the entered data, calls the registration API,
    // stores the returned JWT, and redirects to the logged-in shell.
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

            if (!new EmailAddressAttribute().IsValid(email))
            {
                await DisplayAlert("Fout", "Vul een geldig e-mailadres in.", "OK");
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
                    await appShell.ShowLoggedInAndNavigateToTablesAsync();
                }
                else
                {
                    await Shell.Current.GoToAsync("///tables");
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
                var errorMessage = ExtractRegisterErrorMessage(error);
                await DisplayAlert(
                    "Fout",
                    $"Registratie mislukt: {errorMessage}",
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

    // Converts the raw Identity error response into readable text for the UI.
    private static string ExtractRegisterErrorMessage(string error)
    {
        try
        {
            var identityErrors = JsonSerializer.Deserialize<List<IdentityErrorResponse>>(error, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (identityErrors != null && identityErrors.Count > 0)
            {
                return string.Join(Environment.NewLine, identityErrors.Select(e => e.Description));
            }
        }
        catch
        {
        }

        return error;
    }

    // Helper type used to deserialize Identity errors returned by the API.
    private class IdentityErrorResponse
    {
        public string Description { get; set; } = string.Empty;
    }
}