using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;
using VolleyGo.Views;
using VolleyGo.Views.Organizer;

namespace VolleyGo.ViewModels.Organizer;

public partial class HomeOrganizerViewModel : BaseViewModel
{
    [ObservableProperty]
    private OrganizerDashboardResponse dashboard;

    [ObservableProperty]
    private int cantActivos;

    [ObservableProperty]
    private int cantRealizados;

    [ObservableProperty]
    private int cantTotales;

    [ObservableProperty]
    private bool isDashboardLoading;

    [ObservableProperty]
    private bool showMap;

    [ObservableProperty]
    private bool showMapSearchbar = false;

    [ObservableProperty]
    private string mapTitle = "Ubicación del campeonato";

    [ObservableProperty]
    private bool scrollMapEnabled = false;

    [ObservableProperty]
    private Location location;

    [ObservableProperty]
    private ObservableCollection<ChampionshipResponse> activeChampionships;

    [ObservableProperty]
    private ObservableCollection<ChampionshipResponse> nextChampionships;

    private readonly AuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly UserService _userService;
    private readonly ChampionshipService _championshipService;

    public bool _isLoaded = false;

    public HomeOrganizerViewModel(
        AuthenticationService authenticationServices,
        INavigationService navigationService,
        UserService userService,
        ChampionshipService championshipService
        )
    {
        _authenticationService = authenticationServices;
        _navigationService = navigationService;
        _userService = userService;
        _championshipService = championshipService;

        InitializeCommand = new Command(async () =>
        {
            await SetUserRole();
            await LoadDashboard();
        });
    }

    public Command InitializeCommand { get; set; }

    private async Task SetUserRole()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;
            LoadingMessage = "Cargando info del rol...";
            await Task.Delay(500);
            await _userService.SetUserRole(role: "organizer");
        }
        catch (UnauthorizedAccessException ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        catch (Exception ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        finally
        {
            IsBusy = false;
            LoadingMessage = "";
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;

            _authenticationService.Logout();

            await _navigationService.GoToAsync($"//{nameof(LoadingPage)}?route=//{nameof(LoginPage)}&delay=1000");
        }
        catch (UnauthorizedAccessException ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        catch (Exception ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToChampionship(ChampionshipResponse championship)
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;
            LoadingMessage = "Abriendo campeonato...";
            await Task.Delay(500);
            await _navigationService.GoToAsync($"{nameof(UpdateChampionshipPage)}?id={championship.Id}");
        }
        catch (UnauthorizedAccessException ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        catch (Exception ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        finally
        {
            IsBusy = false;
            LoadingMessage = "";
        }
    }

    private async Task LoadDashboard()
    {
        try
        {
            if (IsBusy) return;
            if (_isLoaded)
            {
                IsDashboardLoading = true;
            }
            else
            {
                IsBusy = true;
            }
            LoadingMessage = "Cargando info de campeonatos...";
            await Task.Delay(500);
            Dashboard = await _championshipService.GetDashboard();
            _isLoaded = true;
        }
        catch (UnauthorizedAccessException ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        catch (Exception ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        finally
        {
            IsBusy = false;
            IsDashboardLoading = false;
            LoadingMessage = "";
        }
    }

    [RelayCommand]
    private async Task GoToCreateChampionship()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;
            LoadingMessage = "Cargando formulario...";
            await Task.Delay(500);
            await _navigationService.GoToAsync(nameof(CreateChampionshipPage));
        }
        catch (UnauthorizedAccessException ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        catch (Exception ex)
        {
            DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
        }
        finally
        {
            IsBusy = false;
            LoadingMessage = "";
        }
    }

    [RelayCommand]
    private async Task DeleteChampionship(ChampionshipResponse championship)
    {
        if (championship == null) return;

        var confirm = await Shell.Current.DisplayAlertAsync(
            "Eliminar campeonato",
            $"¿Seguro que querés eliminar \"{championship.Name}\"?",
            "Eliminar",
            "Cancelar");

        if (!confirm) return;

        try
        {
            IsBusy = true;

            await _championshipService.DeleteChampionship(championship.Id);

            RemoveChampionshipFromLists(championship);
            DisplayPopup(Texts.Success, "Campeonato eliminado correctamente", Texts.Accept);
            await Task.Delay(2000);
            IsBusy = false;
            await LoadDashboard();
        }
        catch (Exception ex)
        {
            DisplayPopup("Error", ex.Message, "Aceptar");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RemoveChampionshipFromLists(ChampionshipResponse championship)
    {
        if (championship == null)
            return;

        if (ActiveChampionships != null)
        {
            var activeItem = ActiveChampionships
                .FirstOrDefault(c => c.Id == championship.Id);

            if (activeItem != null)
                ActiveChampionships.Remove(activeItem);
        }

        if (NextChampionships != null)
        {
            var nextItem = NextChampionships
                .FirstOrDefault(c => c.Id == championship.Id);

            if (nextItem != null)
                NextChampionships.Remove(nextItem);
        }
    }

    [RelayCommand]
    private void ShowMapForChampionship(ChampionshipResponse championship)
    {
        ShowMap = true;

        // Si tiene location → cargarla en el mapa
        var parsed = ParseLocation(championship.Location);
        if (parsed != null)
            Location = parsed; // ← CustomMap lo recibe por Binding
        else
            Location = new Location(-34.9011, -56.1645); // ubicación default (ej: Montevideo)
    }

    private Location? ParseLocation(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var parts = raw.Split(':');
        if (parts.Length != 2) return null;

        if (double.TryParse(parts[0], out double lat) &&
            double.TryParse(parts[1], out double lng))
        {
            return new Location(lat, lng);
        }

        return null;
    }

    [RelayCommand]
    private async Task ToggleShowMap()
    {
        ShowMap = !ShowMap;
    }
}
