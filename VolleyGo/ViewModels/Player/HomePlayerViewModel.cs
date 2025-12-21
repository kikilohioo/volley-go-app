using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Models.API.Player;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;
using VolleyGo.Utils;
using VolleyGo.Views;
using VolleyGo.Views.Player;

namespace VolleyGo.ViewModels.Player;

public partial class HomePlayerViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool showBottomSheet;

    [ObservableProperty]
    private bool showMap;

    [ObservableProperty]
    private bool showMapSearchbar = false;

    [ObservableProperty]
    private string mapTitle = "Ubicación del campeonato";

    [ObservableProperty]
    private PlayerRequest playerJoinTeamRequest;

    [ObservableProperty]
    private bool scrollMapEnabled = false;

    [ObservableProperty]
    private int toRegisterToChampionshipId;

    [ObservableProperty]
    private string toRegisterToChampionshipName;

    [ObservableProperty]
    private Location location;

    [ObservableProperty]
    private ObservableCollection<ChampionshipResponse> championships = [];

    [ObservableProperty]
    private PlayerPositionOption? selectedPlayerPosition;

    public class PlayerPositionOption
    {
        public string Value { get; set; } = null!;
        public string Display { get; set; } = null!;
    }

    [ObservableProperty]
    private ObservableCollection<PlayerPositionOption> playerPositions = [
            new() { Value = "setter", Display = "Armador" },
            new() { Value = "outside_hitter", Display = "Punta" },
            new() { Value = "middle_blocker", Display = "Central" },
            new() { Value = "opposite_spiker", Display = "Opuesto" },
            new() { Value = "libero", Display = "Líbero" },
        ];

    public bool _isLoaded = false;

    private readonly AuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly ChampionshipService _championshipService;
    private readonly UserService _userService;
    private readonly PlayerService _playerService;

    public HomePlayerViewModel(
        AuthenticationService authenticationServices,
        INavigationService navigationService,
        ChampionshipService championshipService,
        UserService userService,
        PlayerService playerService
        )
    {
        _authenticationService = authenticationServices;
        _navigationService = navigationService;
        _championshipService = championshipService;
        _userService = userService;
        _playerService = playerService;

        PlayerJoinTeamRequest = new()
        {
            Name = Preferences.Get(Consts.FullNameKey, string.Empty)
        };

        InitializeCommand = new Command(async () =>
        {
            await SetUserRole();
            await LoadChampionships();
            _isLoaded = true;
        });
    }

    public Command InitializeCommand { get; }

    private async Task SetUserRole()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;
            await _userService.SetUserRole(role: "player");
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

    private async Task LoadChampionships()
    {
        try
        {
            if (IsBusy) return;
            if (_isLoaded)
            {
                IsRefreshing = true;
            }
            else
            {
                IsBusy = true;
            }
            LoadingMessage = "Cargando info de campeonatos...";
            var response = await _championshipService.GetChampionships();
            Championships = new ObservableCollection<ChampionshipResponse>(response);
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
            IsRefreshing = false;
            LoadingMessage = null;
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
    private async Task RegisterMe()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;
            ShowBottomSheet = false;
            await Task.Delay(200);

            if (PlayerJoinTeamRequest == null ||
                PlayerJoinTeamRequest.JerseyNumber == null ||
                PlayerJoinTeamRequest.JoinCode == null ||
                SelectedPlayerPosition == null)
            {
                throw new Exception("Debe completar todos los campos del jugador");
            }

            PlayerJoinTeamRequest.Position = SelectedPlayerPosition.Value;

            var result = await _playerService.JoinTeamByCode(PlayerJoinTeamRequest);
            //await _navigationService.GoToAsync($"//{nameof(LoadingPage)}?route=//{nameof(LoginPage)}&delay=1000");
            DisplayPopup(Texts.Success, $"Inscripto correctamente al campeonato id: {ToRegisterToChampionshipId}", Texts.Accept);
            IsBusy = false;
            await LoadChampionships();
            await Task.Delay(2000);
            ShowPopup = false;
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
    private async Task RegisterMyTeam()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;
            ShowBottomSheet = false;
            LoadingMessage = "Estas apunto de inscribir a tu equipo...";
            await Task.Delay(1000);
            IsBusy = false;


            await _navigationService.GoToAsync($"{nameof(CreateTeamPage)}?championship_name={ToRegisterToChampionshipName}&championship_id={ToRegisterToChampionshipId}");

            await Task.Delay(2000);
            ShowPopup = false;
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
            LoadingMessage = null;
        }
    }

    [RelayCommand]
    private async Task ShowChampionship(ChampionshipResponse championship)
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;
            LoadingMessage = "Abriendo campeonato...";
            await Task.Delay(1000);
            IsBusy = false;

            await _navigationService.GoToAsync($"{nameof(ShowChampionshipPage)}?id={championship.Id}");

            await Task.Delay(2000);
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
            LoadingMessage = null;
        }
    }

    [RelayCommand]
    private async Task RegisterToChampionship(ChampionshipResponse championship)
    {
        ShowBottomSheet = false;
        ToRegisterToChampionshipId = championship.Id;
        ToRegisterToChampionshipName = championship.Name;
        await Task.Delay(500);
        ShowBottomSheet = true;
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
