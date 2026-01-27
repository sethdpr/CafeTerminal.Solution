using System.Windows.Input;
using CafeTerminal.Maui.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.Maui.Controls;

public class RegisterViewModel : BindableObject
{
    private readonly AuthService _authService;

    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    private string _message;
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public ICommand RegisterCommand { get; }

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;

        RegisterCommand = new Command(async () => await RegisterAsync());
    }

    private async Task RegisterAsync()
    {
        //await Application.Current.MainPage.DisplayAlert("Debug", "Command wordt uitgevoerd", "OK");

        try
        {
            await _authService.RegisterAsync(new RegisterRequest
            {
                Email = Email,
                Username = Username,
                Password = Password
            });

            Message = "Registratie geslaagd. Je kunt nu inloggen.";

            await Application.Current.MainPage.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            Message = "Registratie mislukt.";
        }
    }
}
