using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Services;
using VolleyGo.Utils;

namespace VolleyGo.ViewModels.Player;

public partial class ShowChampionshipViewModel : BaseViewModel
{
    [ObservableProperty]
    private ChampionshipResponse championship;

    [ObservableProperty]
    private bool showMap;

    [ObservableProperty]
    private bool showMapSearchbar = false;

    [ObservableProperty]
    private bool scrollMapEnabled = false;

    [ObservableProperty]
    private string mapTitle = "Ubicacion del campeonato";

    [ObservableProperty]
    private bool isDashboardLoading;

    [ObservableProperty]
    private Location championshipLocation;

    private readonly AuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly ApiSettings _apiSettings;
    private readonly UserService _userService;
    private readonly ChampionshipService _championshipService;

    public ShowChampionshipViewModel(
        AuthenticationService authenticationServices,
        INavigationService navigationService,
        IConfiguration config,
        UserService userService,
        ChampionshipService championshipService
        )
    {
        _authenticationService = authenticationServices;
        _navigationService = navigationService;
        _userService = userService;
        _championshipService = championshipService;

        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
    }

    [RelayCommand]
    private async Task LoadChampionship(int id)
    {
        try
        {
            if (IsBusy) return;
            IsDashboardLoading = true;

            var result = await _championshipService.GetChampionshipById(id);

            Championship = result;
            await SetLocation(result.Location);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsDashboardLoading = false;
        }
    }

    private async Task SetLocation(string locationString)
    {
        if (string.IsNullOrWhiteSpace(locationString))
            return;

        // Formato esperado: "-34.89769564394966:-56.18116311728954"
        var parts = locationString.Split(':', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            return;

        if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude))
            return;

        if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
            return;

        Location championshipLocation = new()
        {
            Latitude = latitude,
            Longitude = longitude
        };

        ChampionshipLocation = championshipLocation;
    }

    [RelayCommand]
    private void ToggleShowMap()
    {
        ShowMap = !ShowMap;
    }
}
