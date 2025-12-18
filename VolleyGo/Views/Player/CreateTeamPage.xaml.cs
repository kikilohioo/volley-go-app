using VolleyGo.ViewModels.Player;

namespace VolleyGo.Views.Player;

public partial class CreateTeamPage : ContentPage, IQueryAttributable
{
	private readonly CreateTeamViewModel _viewModel;
	public CreateTeamPage(CreateTeamViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;

        Shell.Current.Navigating += OnShellNavigating;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var rawId) && int.TryParse(rawId.ToString(), out var id))
        {
            //_viewModel.LoadTeamCommand.Execute(id);
        }
        if (query.TryGetValue("championship_id", out var champRawId) &&
            int.TryParse(champRawId.ToString(), out var championshipId) &&
            query.TryGetValue("championship_name", out var champName))
        {
            _viewModel.LoadChampionshipIdCommand.Execute(championshipId);
            _viewModel.LoadChampionshipNameCommand.Execute(champName);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.Current.Navigating -= OnShellNavigating;
    }

    private async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
    {
        // Detectar si el usuario está intentando salir de esta página
        if (e.Source != ShellNavigationSource.PopToRoot) return;

        if (!_viewModel.HasUnsavedChanges)
            return;

        // Cancelar navegación
        e.Cancel();

        bool discard = await DisplayAlertAsync(
            "Confirmar",
            "Tiene cambios sin guardar, ¿Seguro que desea descartarlos?",
            "Descartar",
            "Cancelar"
        );

        if (!discard)
            return;

        // Limpiar cambios
        _viewModel.Team = new();
        _viewModel.TeamLogoPreview = null;

        _viewModel.HasUnsavedChanges = false;

        // Reanudar navegación manualmente
        await Shell.Current.GoToAsync("..");
    }
}