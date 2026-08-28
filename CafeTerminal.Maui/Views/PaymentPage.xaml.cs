using System.Collections.ObjectModel;
using System.Globalization;
using CafeTerminal.Maui.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTerminal.Maui.Views;

// This page shows all unpaid orders for a table and completes the payment flow.
public partial class PaymentPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly int _tableNumber;
    private static readonly TimeZoneInfo BelgianTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
#if WINDOWS
        "Romance Standard Time"
#else
        "Europe/Brussels"
#endif
    );
    private static readonly CultureInfo BelgianCulture = new("nl-BE");

    public ObservableCollection<PaymentOrderViewModel> Orders { get; } = new();

    public PaymentPage(int tableNumber)
    {
        InitializeComponent();

        _tableNumber = tableNumber;
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _apiService = services?.GetService<ApiService>() ?? new ApiService(ApiService.CreateFallbackHttpClient());

        OrdersCollection.ItemsSource = Orders;
    }

    // Loads the latest payment summary whenever the page becomes visible.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSummaryAsync();
    }

    // Retrieves the payment summary from the API and updates the UI.
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
                Orders.Add(new PaymentOrderViewModel(order));
            }

            GrandTotalLabel.Text = $"Totaal: {summary.TotalPrice:F2} EUR";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kon betalingssamenvatting niet laden: {ex.Message}", "OK");
        }
    }

    // Completes the payment for the current table through the API.
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

    // Closes the payment page without completing payment.
    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // This helper view model prepares order data for the payment overview UI.
    public class PaymentOrderViewModel
    {
        public PaymentOrderViewModel(OrderDto order)
        {
            CreatedAtText = $"Bestelling van {FormatBelgianDateTime(order.CreatedAt)}";
            Items = order.Items;
            TotalPrice = order.TotalPrice;
        }

        public string CreatedAtText { get; }
        public List<OrderItemDto> Items { get; }
        public decimal TotalPrice { get; }
    }

    // Converts a UTC timestamp to Belgian local time for display in the app.
    private static string FormatBelgianDateTime(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        var belgianTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), BelgianTimeZone);
        return belgianTime.ToString("dd/MM/yyyy HH:mm", BelgianCulture);
    }
}
