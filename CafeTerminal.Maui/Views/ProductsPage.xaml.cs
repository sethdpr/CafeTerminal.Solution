using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CafeTerminal.Shared.DTOs;
using CafeTerminal.Maui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CafeTerminal.Maui.Views;

public partial class ProductsPage : ContentPage
{
    private readonly ApiService _apiService;

    public ObservableCollection<ProductDto> Products { get; } = new();

    public ProductsPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        // Resolve ApiService from DI if available; otherwise fall back to a default instance
        _apiService = services?.GetService<ApiService>() ?? new ApiService(new HttpClient { BaseAddress = new Uri("https://localhost:7232") });

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var list = await _apiService.GetProductsAsync();
            Products.Clear();
            foreach (var p in list)
                Products.Add(p);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Kon producten niet laden: {ex.Message}", "OK");
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Fout", "Geef een productnaam op.", "OK");
            return;
        }

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
            var created = await _apiService.CreateProductAsync(name, price);
            if (created != null)
            {
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
            await DisplayAlert("Fout", $"Fout bij aanmaken product: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int id)
        {
            var confirm = await DisplayAlert("Bevestig", "Weet je zeker dat je dit product wilt verwijderen?", "Ja", "Nee");
            if (!confirm) return;

            try
            {
                var success = await _apiService.DeleteProductAsync(id);
                if (success)
                {
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
                await DisplayAlert("Fout", $"Fout bij verwijderen: {ex.Message}", "OK");
            }
        }
    }
}
