using CafeTerminal.Maui.Services;

using Microsoft.Maui.ApplicationModel;
using CafeTerminal.Maui.Views;

namespace CafeTerminal.Maui
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;

        public AppShell()
        {
            InitializeComponent();

            // Register routes so navigation works even if Shell items are modified at runtime
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("tables", typeof(TablesPage));
            Routing.RegisterRoute("products", typeof(ProductsPage));

            _authService = new AuthService();

            Navigating += OnShellNavigating;

            // Initialize menu items based on login state
            _ = UpdateShellItemsAsync();
        }

        private async void OnShellNavigating( //The app checks if the user is logged in when navigating to the main page. If not, it cancels the navigation and redirects to the login page.
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

        // Removed automatic refresh on every navigation to avoid resetting the current page.
        // Shell items are refreshed explicitly after login/logout via the helper methods.

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

        // Public helper to refresh shell items and navigate to the login page.
        public async Task ShowLoggedOutAndNavigateToLoginAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered login route.
            await GoToAsync("///login");
        }

        // Public helper to refresh shell items and navigate to the main page after login
        public async Task ShowLoggedInAndNavigateToTablesAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered tables route.
            await GoToAsync("///tables");
        }

        // Public helper to refresh shell items and navigate to the products page after login
        public async Task ShowLoggedInAndNavigateToProductsAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered products route.
            await GoToAsync("///products");
        }
    }
}
