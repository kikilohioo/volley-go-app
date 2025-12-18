
using System.Collections;
using VolleyGo.ViewModels;

namespace VolleyGo.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
