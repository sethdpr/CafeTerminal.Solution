using CafeTerminal.Maui.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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

        // Store the table number so the created order can be linked correctly.
        _tableNumber = tableNumber;

        var services = Application.Current?.Handler?.MauiContext?.Services;

        _apiService = services?.GetService<ApiService>()
            ?? new ApiService(ApiService.CreateFallbackHttpClient());

        // Bind the product rows to the collection view.
        ProductsList.ItemsSource = Rows;
    }

    // Loads all active products from the API when the page becomes visible.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Load the active product catalog for quantity selection.
            var products = await _apiService.GetProductsAsync();

            Rows.Clear();

            // Create one editable UI row per product.
            foreach (var p in products)
            {
                Rows.Add(new ProductOrderRow
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = 0
                });
            }

            if (Rows.Count == 0)
            {
                // Explain why ordering is not possible when no products exist.
                await DisplayAlert(
                    "Info",
                    "Er zijn geen actieve producten beschikbaar. Voeg eerst producten toe op de pagina Producten.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            // Show product-loading failures.
            await DisplayAlert(
                "Fout",
                $"Kon producten niet laden: {ex.Message}",
                "OK");
        }
    }

    // Decreases the quantity of the selected product.
    private void OnDecreaseQuantityClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is ProductOrderRow row)
        {
            // Prevent the quantity from dropping below zero.
            if (row.Quantity > 0)
            {
                row.Quantity--;
            }
        }
    }

    // Increases the quantity of the selected product.
    private void OnIncreaseQuantityClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is ProductOrderRow row)
        {
            // Keep quantities within a practical upper limit.
            if (row.Quantity < 20)
            {
                row.Quantity++;
            }
        }
    }

    // Sends the selected products and quantities to the API to create an order.
    private async void OnSaveOrderClicked(object sender, EventArgs e)
    {
        // Collect only the rows where the user selected a quantity.
        var items = Rows
            .Where(r => r.Quantity > 0)
            .Select(r => new CreateOrderItemRequest
            {
                ProductId = r.ProductId,
                Quantity = r.Quantity
            })
            .ToList();

        if (!items.Any())
        {
            await DisplayAlert(
                "Fout",
                "Selecteer minstens één product met hoeveelheid.",
                "OK");

            return;
        }

        // Build the full order payload for the selected table.
        var req = new CreateOrderRequest
        {
            TableNumber = _tableNumber,
            Items = items
        };

        try
        {
            // Submit the new order to the backend.
            var created = await _apiService.CreateOrderAsync(req);

            if (created != null)
            {
                // Confirm the order and return to the previous page.
                await DisplayAlert(
                    "Succes",
                    $"Bestelling aangemaakt. Totaal: {created.TotalPrice:F2} EUR",
                    "OK");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert(
                    "Fout",
                    "Kon bestelling niet aanmaken.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            // Show API or network errors that prevented order creation.
            await DisplayAlert(
                "Fout",
                $"Fout bij aanmaken bestelling: {ex.Message}",
                "OK");
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
                // Ignore duplicate values to avoid unnecessary UI notifications.
                if (_quantity == value)
                    return;

                _quantity = value;

                // Notify the bound UI that the quantity changed.
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Quantity)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}