using CafeTerminal.Maui.Services;
using CafeTerminal.Maui.ViewModels;

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

        builder.Services.AddSingleton<HttpClient>(_ =>
            new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7232/")
            });

        builder.Services.AddSingleton<ProductService>();
        builder.Services.AddSingleton<TableService>();
        builder.Services.AddSingleton<OrderService>();

        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}