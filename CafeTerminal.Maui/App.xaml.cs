using Microsoft.Maui.Controls;
using CafeTerminal.Maui.Views;

namespace CafeTerminal.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell(); //set the main page to AppShell wich automatically shows the first ShellContent: LoginPage
        }
    }
}