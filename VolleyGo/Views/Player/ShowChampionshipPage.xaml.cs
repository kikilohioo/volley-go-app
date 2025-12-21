using VolleyGo.ViewModels.Player;

namespace VolleyGo.Views.Player;

public partial class ShowChampionshipPage : ContentPage, IQueryAttributable
{
	private readonly ShowChampionshipViewModel _viewModel;
	public ShowChampionshipPage(ShowChampionshipViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;

        //Shell.Current.Navigating += OnShellNavigating;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var rawId) && int.TryParse(rawId.ToString(), out var id))
        {
            _viewModel.LoadChampionshipCommand.Execute(id);
        }
    }
}