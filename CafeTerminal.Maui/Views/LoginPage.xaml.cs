using CafeTerminal.Shared.DTOs;
using System.Text.Json;
using System.Text;

namespace CafeTerminal.Maui.Views;

// This page handles the login flow for existing users.
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    // Validates the entered credentials, calls the login API, stores the JWT,
    // and redirects the user to the logged-in shell.
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert(
                "Fout",
                "Gelieve gebruikersnaam en wachtwoord in te vullen.",
                "OK");

            return; //Check if username and password are not empty. If they are, show an alert and return early
        }

        var request = new LoginRequest
        {
            Username = username,
            Password = password //Create LoginRequest
        };

        var json = JsonSerializer.Serialize(request); //Serialize the request object to JSON

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"); //Create StringContent with the JSON, encoding and content type

        try
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7232") //Set the base address of the HttpClient to the API's URL
            };

            var response = await client.PostAsync(
                "/api/Auth/login",
                content); //Send a POST request to the API's login endpoint with the JSON content

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
                        "De login is gelukt, maar er werd geen JWT-token ontvangen.",
                        "OK");

                    return;
                }

                //store the JWT token securely on the device
                await SecureStorage.Default.SetAsync(
                    "auth_token",
                    authResponse.Token);

                // Rebuild the shell items for the logged-in state before navigating
                if (Shell.Current is AppShell appShell)
                {
                    await appShell.ShowLoggedInAndNavigateToTablesAsync();
                }
                else
                {
                    await Shell.Current.GoToAsync("///tables");
                }
            }
            else
            {
                await DisplayAlert(
                    "Login mislukt",
                    "Ongeldige gebruikersnaam of wachtwoord.",
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

    // Navigates from the login page to the registration page.
    private async void OnRegisterHereClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///register"); //This line recognises the route defined in AppShell.xaml

        /*We use "///" to navigate between Shell-elements.
                 This is called 'absolute routing'. Different from 'relative routing'.
                 Relative routing can be described as taking stairs to navigate through the app.
                 With absolute routing, we can teleport between different "rooms" in the app.
                 Like going from "/Home/Menu" to "/Account/Settings" in 1 move, for example*/
    }
}