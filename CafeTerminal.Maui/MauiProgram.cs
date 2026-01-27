using CafeTerminal.Maui.Services;
using CafeTerminal.Maui.ViewModels;
using CafeTerminal.Maui.Views;

namespace CafeTerminal.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

#if ANDROID
        // Android emulator kan localhost niet direct bereiken
        var apiBase = "https://10.0.2.2:7232/";
#else
        // Windows en Mac kunnen localhost rechtstreeks gebruiken
        var apiBase = "https://localhost:7232/";
#endif

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(apiBase)
        });

        // Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ProductService>();
        builder.Services.AddSingleton<TableService>();
        builder.Services.AddSingleton<OrderService>();

        // ViewModels
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddSingleton<MainViewModel>();

        // Pages
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}