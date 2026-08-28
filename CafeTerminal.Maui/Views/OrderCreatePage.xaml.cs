using CafeTerminal.Shared.DTOs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CafeTerminal.Shared.DTOs;
using CafeTerminal.Maui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTerminal.Maui.Views;

// This page creates one new order for a specific table.
public partial class OrderCreatePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly int _tableNumber;

    public ObservableCollection<ProductOrderRow> Rows { get; } = new();

    public OrderCreatePage(int tableNumber)
    {
        InitializeComponent();

        _tableNumber = tableNumber;

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _apiService = services?.GetService<ApiService>() ?? new ApiService(ApiService.CreateFallbackHttpClient());

        ProductsList.ItemsSource = Rows;
    }

    // Loads all active products from the API when the page becomes visible.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var products = await _apiService.GetProductsAsync();
            Rows.Clear();
            foreach (var p in products)
            {
                Rows.Add(new ProductOrderRow { ProductId = p.Id, Name = p.Name, Price = p.Price, Quantity = 0 });
            }

            if (Rows.Count == 0)
            {
                await DisplayAlert("Info", "Er zijn geen actieve producten beschikbaar. Voeg eerst producten toe op de pagina Producten.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kon producten niet laden: {ex.Message}", "OK");
        }
    }

    // Sends the selected products and quantities to the API to create an order.
    private async void OnSaveOrderClicked(object sender, EventArgs e)
    {
        var items = Rows.Where(r => r.Quantity > 0)
            .Select(r => new CreateOrderItemRequest { ProductId = r.ProductId, Quantity = r.Quantity })
            .ToList();

        if (!items.Any())
        {
            await DisplayAlert("Fout", "Selecteer minstens één product met hoeveelheid.", "OK");
            return;
        }

        var req = new CreateOrderRequest { TableNumber = _tableNumber, Items = items };
        try
        {
            var created = await _apiService.CreateOrderAsync(req);
            if (created != null)
            {
                await DisplayAlert("Succes", $"Bestelling aangemaakt. Totaal: {created.TotalPrice:F2} EUR", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Fout", "Kon bestelling niet aanmaken.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Fout bij aanmaken bestelling: {ex.Message}", "OK");
        }
    }

    // Closes the page without creating an order.
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // This helper view model represents one product row in the order creation UI.
    public class ProductOrderRow : INotifyPropertyChanged
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
