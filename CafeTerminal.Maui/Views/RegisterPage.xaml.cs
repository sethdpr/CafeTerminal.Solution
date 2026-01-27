using CafeTerminal.Maui.ViewModels;
using Microsoft.Maui.Controls;

namespace CafeTerminal.Maui.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}