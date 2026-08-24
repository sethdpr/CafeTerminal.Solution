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
    private bool _isOrderPageOpen;

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _isOrderPageOpen = false;
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
                await Navigation.PopModalAsync();
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
        if (_isOrderPageOpen || Navigation.NavigationStack.LastOrDefault() is OrderCreatePage)
        {
            return;
        }

        _isOrderPageOpen = true;

        // Open order creation page on the dialog's own navigation stack
        var orderPage = new OrderCreatePage(_table.Number);
        await Navigation.PushAsync(orderPage);

        // Note: After the modal is popped, this page appears again.
        // The guard is reset in OnAppearing.
    }

    private async void OnPaymentClicked(object sender, EventArgs e)
    {
        // Placeholder - will call API or navigate to payment page later
        await DisplayAlert("Info", "Betaling - nog niet geïmplementeerd", "OK");
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
