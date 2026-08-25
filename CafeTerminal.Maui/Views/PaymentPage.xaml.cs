using CafeTerminal.Maui.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace CafeTerminal.Maui.Views;

public partial class PaymentPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly int _tableNumber;

    public ObservableCollection<OrderDto> Orders { get; } = new();

    public PaymentPage(int tableNumber)
    {
        InitializeComponent();

        _tableNumber = tableNumber;
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _apiService = services?.GetService<ApiService>() ?? new ApiService(new HttpClient { BaseAddress = new Uri("https://localhost:7232") });

        OrdersCollection.ItemsSource = Orders;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSummaryAsync();
    }

    private async Task LoadSummaryAsync()
    {
        try
        {
            var summary = await _apiService.GetPaymentSummaryAsync(_tableNumber);
            if (summary == null)
            {
                await DisplayAlert("Fout", "Kon betalingssamenvatting niet laden.", "OK");
                return;
            }

            TitleLabel.Text = string.IsNullOrWhiteSpace(summary.TableName)
                ? $"Tafel {summary.TableNumber}"
                : $"Tafel {summary.TableNumber} - {summary.TableName}";

            Orders.Clear();
            foreach (var order in summary.Orders)
            {
                Orders.Add(order);
            }

            GrandTotalLabel.Text = $"Totaal: {summary.TotalPrice:F2} EUR";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kon betalingssamenvatting niet laden: {ex.Message}", "OK");
        }
    }

    private async void OnPaymentCompleteClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Bevestig", "Betaling afronden voor deze tafel?", "Ja", "Nee");
        if (!confirm)
        {
            return;
        }

        try
        {
            var success = await _apiService.CompletePaymentAsync(_tableNumber);
            if (!success)
            {
                await DisplayAlert("Fout", "Kon betaling niet afronden.", "OK");
                return;
            }

            await DisplayAlert("Succes", "Betaling afgerond.", "OK");
            await Navigation.PopAsync();
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Fout bij afronden betaling: {ex.Message}", "OK");
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
