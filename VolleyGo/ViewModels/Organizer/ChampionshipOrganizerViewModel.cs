using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;
using VolleyGo.Views;
using VolleyGo.Views.Organizer;

namespace VolleyGo.ViewModels.Organizer;

public partial class ChampionshipOrganizerViewModel : BaseViewModel
{
    [ObservableProperty]
    private ChampionshipResponse championship;

    private readonly AuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly UserService _userService;
    private readonly ChampionshipService _championshipService;

    public ChampionshipOrganizerViewModel(
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
    }

    [RelayCommand]
    private async Task LoadChampionship(int id)
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;

            var result = await _championshipService.GetChampionshipById(id);

            Championship = result;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
