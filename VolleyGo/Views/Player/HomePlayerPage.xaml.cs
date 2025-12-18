using VolleyGo.ViewModels.Player;

namespace VolleyGo.Views.Player;

public partial class HomePlayerPage : ContentPage
{
	public readonly HomePlayerViewModel _viewModel;
    public HomePlayerPage(HomePlayerViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _viewModel.InitializeCommand.Execute(this);
    }
}