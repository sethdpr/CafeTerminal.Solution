using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CafeTerminal.Shared.Models;
using CafeTerminal.Maui.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CafeTerminal.Maui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public readonly ProductService _productService;
    public readonly TableService _tableService;
    public readonly OrderService _orderService;

    public MainViewModel(ProductService productService, TableService tableService, OrderService orderService)
    {
        _productService = productService;
        _tableService = tableService;
        _orderService = orderService;

        Products = new ObservableCollection<Product>();
        Tables = new ObservableCollection<Table>();
        Orders = new ObservableCollection<Order>();
    }

    // --- Product properties ---
    [ObservableProperty]
    private string newProductName = string.Empty;

    [ObservableProperty]
    private decimal newProductPrice;

    public ObservableCollection<Product> Products { get; }

    // --- Table properties ---
    [ObservableProperty]
    private string newTableName = string.Empty;

    [ObservableProperty]
    private int newTableNumber;

    [ObservableProperty]
    private Table selectedTable;

    public ObservableCollection<Table> Tables { get; }

    // --- Order properties ---
    [ObservableProperty]
    private ObservableCollection<OrderLine> newOrderLines = new ObservableCollection<OrderLine>();

    public ObservableCollection<Order> Orders { get; }

    // --- Commands ---
    [RelayCommand]
    public async Task LoadAllAsync()
    {
        Products.Clear();
        var products = await _productService.GetProductsAsync();
        foreach (var p in products) Products.Add(p);

        Tables.Clear();
        var tables = await _tableService.GetTablesAsync();
        foreach (var t in tables) Tables.Add(t);

        Orders.Clear();
        var orders = await _orderService.GetOrdersAsync();
        foreach (var o in orders) Orders.Add(o);
    }

    [RelayCommand]
    private async Task AddProductAsync()
    {
        var product = new Product { Name = NewProductName, Price = NewProductPrice };
        var created = await _productService.CreateProductAsync(product);
        if (created != null) Products.Add(created);

        NewProductName = string.Empty;
        NewProductPrice = 0;
    }

    [RelayCommand]
    private async Task DeleteProductAsync(Product product)
    {
        if (product == null) return;
        var success = await _productService.DeleteProductAsync(product.Id);
        if (success) Products.Remove(product);
    }

    [RelayCommand]
    private async Task AddTableAsync()
    {
        var table = new Table { Name = NewTableName, Number = NewTableNumber };
        var created = await _tableService.CreateTableAsync(table);
        if (created != null) Tables.Add(created);

        NewTableName = string.Empty;
        NewTableNumber = 0;
    }

    [RelayCommand]
    private async Task DeleteTableAsync(Table table)
    {
        if (table == null) return;
        var success = await _tableService.DeleteTableAsync(table.Id);
        if (success) Tables.Remove(table);
    }

    [RelayCommand]
    private async Task AddOrderAsync()
    {
        if (SelectedTable == null) return;

        var order = new Order
        {
            TableId = SelectedTable.Id,
            OrderLines = new List<OrderLine>(NewOrderLines),
            CreatedAt = DateTime.UtcNow,
            IsClosed = false
        };

        var created = await _orderService.CreateOrderAsync(order);
        if (created != null) Orders.Add(created);

        NewOrderLines.Clear();
    }

    [RelayCommand]
    private async Task DeleteOrderAsync(Order order)
    {
        if (order == null) return;
        var success = await _orderService.DeleteOrderAsync(order.Id);
        if (success) Orders.Remove(order);
    }
}
