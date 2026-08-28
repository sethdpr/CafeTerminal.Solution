using Microsoft.Maui.Controls;
using CafeTerminal.Maui.Views;

namespace CafeTerminal.Maui
{
    // This is the root MAUI application class.
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Start the app inside the Shell so page navigation is centralized.
            MainPage = new AppShell();
        }
    }
}