using System.Text.Json;
using System.Text;
using CafeTerminal.Maui.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;


namespace CafeTerminal.Maui.Views;

public partial class TableDialogPage : ContentPage
{
    private readonly TablesPage.TableItem _table;
    private readonly ApiService? _apiService;

    public TableDialogPage(TablesPage.TableItem table)
    {
        InitializeComponent();

        _table = table;
        // Resolve ApiService from MAUI DI
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _apiService = services?.GetService<ApiService>();

        TitleLabel.Text = $"Table {_table.Number}";

        if (!string.IsNullOrWhiteSpace(_table.Name))
        {
            NameEntry.Text = _table.Name;
            NameEntry.IsVisible = false;
            SaveButton.IsVisible = false;
            AddOrderButton.IsVisible = true;
            PaymentButton.IsVisible = true;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Fout", "Geef een naam op voor de tafel.", "OK");
            return;
        }

        try
        {
            if (_apiService == null)
            {
                await DisplayAlert("Fout", "API service niet beschikbaar.", "OK");
                return;
            }

            var success = await _apiService.SetTableNameAsync(_table.Number, name);
            if (success)
            {
                _table.Name = name;
                await DisplayAlert("Succes", "Tabelnaam opgeslagen", "OK");
                await Shell.Current.Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Fout", "Kon tabelnaam niet opslaan.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er kon geen verbinding worden gemaakt met de API: {ex.Message}", "OK");
        }
    }

    private async void OnAddOrderClicked(object sender, EventArgs e)
    {
        // Placeholder - will call API or navigate to order page later
        await DisplayAlert("Info", "Voeg bestelling - nog niet geïmplementeerd", "OK");
    }

    private async void OnPaymentClicked(object sender, EventArgs e)
    {
        // Placeholder - will call API or navigate to payment page later
        await DisplayAlert("Info", "Betaling - nog niet geïmplementeerd", "OK");
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
