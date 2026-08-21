using CafeTerminal.Maui.Views;

using CafeTerminal.Maui.Views;
using CafeTerminal.Maui.Services;

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
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<OrderCreatePage>();

        // Pages
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<TablesPage>();
        builder.Services.AddSingleton<ProductsPage>();
        builder.Services.AddSingleton<OrderCreatePage>();

        return builder.Build();
    }
}