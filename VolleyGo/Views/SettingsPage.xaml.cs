using VolleyGo.ViewModels;

namespace VolleyGo.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.GetMeCommand.Execute(this);
    }
}