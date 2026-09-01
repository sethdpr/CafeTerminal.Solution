using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CafeTerminal.Shared.DTOs;
using CafeTerminal.Maui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTerminal.Maui.Views;

// This page manages the products that can be ordered in the app.
public partial class ProductsPage : ContentPage
{
    private readonly ApiService _apiService;

    public ObservableCollection<ProductDto> Products { get; } = new();

    public ProductsPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        // Resolve ApiService from DI if available; otherwise fall back to a default instance
        _apiService = services?.GetService<ApiService>() ?? new ApiService(ApiService.CreateFallbackHttpClient());

        BindingContext = this;
    }

    // Reloads the product list whenever the page becomes visible.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadProductsAsync();
    }

    // Loads the active products from the API into the collection view.
    private async Task LoadProductsAsync()
    {
        try
        {
            // Fetch the active products and refresh the bound collection.
            var list = await _apiService.GetProductsAsync();
            Products.Clear();
            foreach (var p in list)
                Products.Add(p);
        }
        catch (Exception ex)
        {
            // Show API or connectivity failures in a simple alert.
            await DisplayAlert("Fout", $"Kon producten niet laden: {ex.Message}", "OK");
        }
    }

    // Validates the entered values and creates a new product through the API.
    private async void OnAddClicked(object sender, EventArgs e)
    {
        // Read and normalize the entered product name.
        var name = NameEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Fout", "Geef een productnaam op.", "OK");
            return;
        }

        // Validate the price field before sending it to the API.
        if (!decimal.TryParse(PriceEntry.Text, out var price))
        {
            await DisplayAlert("Fout", "Geef een geldige prijs op.", "OK");
            return;
        }

        if (price <= 0)
        {
            await DisplayAlert("Fout", "Geef een prijs groter dan 0 op.", "OK");
            return;
        }

        try
        {
            // Create the product on the backend.
            var created = await _apiService.CreateProductAsync(name, price);
            if (created != null)
            {
                // Add the created product locally and clear the input fields.
                Products.Add(created);
                NameEntry.Text = string.Empty;
                PriceEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Fout", "Kon product niet aanmaken.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Show creation failures from the API or network layer.
            await DisplayAlert("Fout", $"Fout bij aanmaken product: {ex.Message}", "OK");
        }
    }

    // Soft deletes a product after the user confirms the action.
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int id)
        {
            // Ask the user to confirm the delete action first.
            var confirm = await DisplayAlert("Bevestig", "Weet je zeker dat je dit product wilt verwijderen?", "Ja", "Nee");
            if (!confirm) return;

            try
            {
                // Delete the product through the API.
                var success = await _apiService.DeleteProductAsync(id);
                if (success)
                {
                    // Remove the deleted product from the local collection.
                    var existing = Products.FirstOrDefault(p => p.Id == id);
                    if (existing != null) Products.Remove(existing);
                }
                else
                {
                    await DisplayAlert("Fout", "Kon product niet verwijderen.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Show delete failures from the API or network layer.
                await DisplayAlert("Fout", $"Fout bij verwijderen: {ex.Message}", "OK");
            }
        }
    }
}
