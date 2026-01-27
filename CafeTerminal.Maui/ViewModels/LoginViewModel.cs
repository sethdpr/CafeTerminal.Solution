using System.Windows.Input;
using CafeTerminal.Maui.Services;
using CafeTerminal.Maui.Views;
using CafeTerminal.Shared.DTOs;

public class LoginViewModel : BindableObject
{
    private readonly AuthService _authService;
    private readonly IServiceProvider _services;

    public LoginViewModel(AuthService authService, IServiceProvider services)
    {
        _authService = authService;
        _services = services;

        LoginCommand = new Command(async () => await LoginAsync());
    }

    private string _username;
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            var result = await _authService.LoginAsync(new LoginRequest
            {
                Username = Username,
                Password = Password
            });

            await SecureStorage.SetAsync("auth_token", result.Token);

            var mainPage = _services.GetService<MainPage>();
            Application.Current.MainPage = new NavigationPage(mainPage);
        }
        catch
        {
            ErrorMessage = "Ongeldige login gegevens";
        }
    }
}
