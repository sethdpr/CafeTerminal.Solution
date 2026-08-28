using CafeTerminal.Maui.Views;
using CafeTerminal.Maui.Services;

namespace CafeTerminal.Maui;

// This class configures dependency injection, fonts, and services for the MAUI app.
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
        // Android emulator uses the host machine through 10.0.2.2.
        // Use HTTP in development because the local HTTPS certificate is for localhost,
        // not for 10.0.2.2.
        var apiBase = "http://10.0.2.2:5006/";
#else
        // Windows en Mac kunnen localhost rechtstreeks gebruiken
        var apiBase = "https://localhost:7232/";
#endif

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(apiBase)
        });

        // Shared services used by multiple pages.
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<AuthService>();

        // Pages registered for navigation and dependency resolution.
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<TablesPage>();
        builder.Services.AddSingleton<ProductsPage>();
        builder.Services.AddSingleton<OrderCreatePage>();
        builder.Services.AddSingleton<PaymentPage>();

        return builder.Build();
    }
}