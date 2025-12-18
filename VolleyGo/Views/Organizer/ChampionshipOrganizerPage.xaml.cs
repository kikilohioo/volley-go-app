using VolleyGo.ViewModels.Organizer;

namespace VolleyGo.Views.Organizer;

public partial class ChampionshipOrganizerPage : ContentPage, IQueryAttributable
{
    private readonly ChampionshipOrganizerViewModel _viewModel;


    public ChampionshipOrganizerPage(ChampionshipOrganizerViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var rawId) && int.TryParse(rawId.ToString(), out var id))
        {
            _viewModel.LoadChampionshipCommand.Execute(id);
        }
    }
}