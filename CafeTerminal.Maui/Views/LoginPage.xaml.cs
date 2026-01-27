using CafeTerminal.Maui.ViewModels;

namespace CafeTerminal.Maui.Views;

public partial class LoginPage : ContentPage
{
    private readonly IServiceProvider _services;

    public LoginPage(LoginViewModel viewModel, IServiceProvider services)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _services = services;   
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var registerPage = _services.GetService<RegisterPage>();
        if (registerPage != null)
        {
            await Navigation.PushAsync(registerPage);
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "RegisterPage niet gevonden", "OK");
        }
    }
}