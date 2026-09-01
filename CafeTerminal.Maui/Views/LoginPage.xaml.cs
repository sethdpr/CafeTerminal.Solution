using CafeTerminal.Shared.DTOs;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

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
        // Read the entered credentials from the form.
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        // Stop early when the user left required fields empty.
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert(
                "Fout",
                "Gelieve gebruikersnaam en wachtwoord in te vullen.",
                "OK");

            return; //Check if username and password are not empty. If they are, show an alert and return early
        }

        // Build the shared login request that matches the API contract.
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
            // Try to reuse the app-wide HttpClient; create a fallback client when needed.
            var services = Application.Current?.Handler?.MauiContext?.Services;
            var client = services?.GetService<HttpClient>();
            var disposeClient = false;

            if (client == null)
            {
                client = new HttpClient
                {
#if ANDROID
                    BaseAddress = new Uri("http://10.0.2.2:5006/")
#else
                    BaseAddress = new Uri("https://localhost:7232/")
#endif
                };
                disposeClient = true;
            }

            try
            {
                // Send the login request to the API.
                var response = await client.PostAsync(
                    "/api/Auth/login",
                    content); //Send a POST request to the API's login endpoint with the JSON content

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the API response.
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

                    // Persist the JWT so later API calls can authenticate automatically.
                    //store the JWT token securely on the device
                    await SecureStorage.Default.SetAsync(
                        "auth_token",
                        authResponse.Token);

                    // Switch the shell to the logged-in navigation structure.
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
                    // Show a generic login failure message for invalid credentials.
                    await DisplayAlert(
                        "Login mislukt",
                        "Ongeldige gebruikersnaam of wachtwoord.",
                        "OK");
                }
            }
            finally
            {
                // Dispose the fallback client created in this method only.
                if (disposeClient)
                {
                    client.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            // Surface connectivity or server errors to the user.
            await DisplayAlert(
                "Fout",
                $"Er kon geen verbinding worden gemaakt met de API: {ex.Message}",
                "OK");
        }
    }

    // Navigates from the login page to the registration page.
    private async void OnRegisterHereClicked(object sender, EventArgs e)
    {
        // Jump directly to the register shell route.
        await Shell.Current.GoToAsync("///register"); //This line recognises the route defined in AppShell.xaml

        /*We use "///" to navigate between Shell-elements.
                 This is called 'absolute routing'. Different from 'relative routing'.
                 Relative routing can be described as taking stairs to navigate through the app.
                 With absolute routing, we can teleport between different "rooms" in the app.
                 Like going from "/Home/Menu" to "/Account/Settings" in 1 move, for example*/
    }
}