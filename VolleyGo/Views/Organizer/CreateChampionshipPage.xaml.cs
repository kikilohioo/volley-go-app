using VolleyGo.Resources.Languages;
using VolleyGo.ViewModels.Organizer;

namespace VolleyGo.Views.Organizer;

public partial class CreateChampionshipPage : ContentPage
{
	private readonly CreateChampionshipViewModel _viewModel;
	public CreateChampionshipPage(CreateChampionshipViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;

        Shell.Current.Navigating += OnShellNavigating;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.Current.Navigating -= OnShellNavigating;
    }

    private void ChampionshipMap_LocationSelected(object sender, Location e)
    {
		_viewModel.SetLocationCommand.Execute(e);
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
        _viewModel.NewChampionship = new();
        _viewModel.Location = null;
        _viewModel.LocationString = string.Empty;
        _viewModel.HeaderImage = null;

        _viewModel.HasUnsavedChanges = false;

        // Reanudar navegación manualmente
        await Shell.Current.GoToAsync("..");
    }
}