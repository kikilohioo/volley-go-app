using VolleyGo.ViewModels.Player;

namespace VolleyGo.Views.Player;

public partial class HomePlayerPage : ContentPage, IQueryAttributable
{
	public readonly HomePlayerViewModel _viewModel;
    public HomePlayerPage(HomePlayerViewModel viewModel)
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

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!_viewModel._isLoaded) _viewModel.InitializeCommand.Execute(this);
    }
}