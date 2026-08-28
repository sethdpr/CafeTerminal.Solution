using CafeTerminal.Maui.Services;

using Microsoft.Maui.ApplicationModel;
using CafeTerminal.Maui.Views;

namespace CafeTerminal.Maui
{
    // This Shell controls the main navigation structure of the MAUI app.
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;

        public AppShell()
        {
            InitializeComponent();

            // Register routes so navigation works even when the visible shell items change.
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("tables", typeof(TablesPage));
            Routing.RegisterRoute("products", typeof(ProductsPage));

            _authService = new AuthService();

            Navigating += OnShellNavigating;

            // Show the correct menu items for the current login state.
            _ = UpdateShellItemsAsync();
        }

        // Blocks navigation to logged-in pages when no token is available.
        private async void OnShellNavigating(
            object? sender,
            ShellNavigatingEventArgs args)
        {
            if (args.Target.Location.OriginalString.Contains("tables") || args.Target.Location.OriginalString.Contains("products"))
            {
                var isLoggedIn = await _authService.IsLoggedInAsync();

                if (!isLoggedIn)
                {
                    args.Cancel();

                    await GoToAsync("///login");
                }
            }
        }

        // Rebuilds the shell menu so only the pages for the current login state are visible.
        private async Task UpdateShellItemsAsync()
        {
            var isLoggedIn = await _authService.IsLoggedInAsync();

            // Ensure UI updates run on the main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items.Clear();

                if (isLoggedIn)
                {
                    Items.Add(TablesShellContent);
                    Items.Add(ProductsShellContent);
                }
                else
                {
                    Items.Add(LoginShellContent);
                    Items.Add(RegisterShellContent);
                }
            });
        }

        // Refreshes the shell after logout and redirects to the login page.
        public async Task ShowLoggedOutAndNavigateToLoginAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered login route.
            await GoToAsync("///login");
        }

        // Refreshes the shell after login and redirects to the tables page.
        public async Task ShowLoggedInAndNavigateToTablesAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered tables route.
            await GoToAsync("///tables");
        }

        // Refreshes the shell after login and redirects to the products page.
        public async Task ShowLoggedInAndNavigateToProductsAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered products route.
            await GoToAsync("///products");
        }
    }
}
