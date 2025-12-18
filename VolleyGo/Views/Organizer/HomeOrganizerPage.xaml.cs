using VolleyGo.ViewModels.Organizer;

namespace VolleyGo.Views.Organizer;

public partial class HomeOrganizerPage : ContentPage, IQueryAttributable
{
	private readonly HomeOrganizerViewModel _viewModel;
	public HomeOrganizerPage(HomeOrganizerViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("refresh", out var refreshObj)
        && bool.TryParse(refreshObj?.ToString(), out var refresh)
        && refresh)
        {
            _viewModel.InitializeCommand.Execute(null);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if(!_viewModel._isLoaded) _viewModel.InitializeCommand.Execute(this);
    }
}