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
            Routing.RegisterRoute("main", typeof(MainPage));

            _authService = new AuthService();

            Navigating += OnShellNavigating;
            Navigated += OnShellNavigated;

            // Initialize menu items based on login state
            _ = UpdateShellItemsAsync();
        }

        private async void OnShellNavigating( //The app checks if the user is logged in when navigating to the main page. If not, it cancels the navigation and redirects to the login page.
            object? sender,
            ShellNavigatingEventArgs args)
        {
            if (args.Target.Location.OriginalString.Contains("main"))
            {
                var isLoggedIn = await _authService.IsLoggedInAsync();

                if (!isLoggedIn)
                {
                    args.Cancel();

                    await GoToAsync("///login");
                }
            }
        }

        private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            // Refresh the visible shell items after navigation to reflect login state
            _ = UpdateShellItemsAsync();
        }

        private async Task UpdateShellItemsAsync()
        {
            var isLoggedIn = await _authService.IsLoggedInAsync();

            // Ensure UI updates run on the main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items.Clear();

                if (isLoggedIn)
                {
                    Items.Add(MainShellContent);
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
        public async Task ShowLoggedInAndNavigateToMainAsync()
        {
            await UpdateShellItemsAsync();

            // After items are updated, navigate to the registered main route.
            await GoToAsync("///main");
        }
    }
}
