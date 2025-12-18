using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Models.API.Player;
using VolleyGo.Models.API.Team;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;

namespace VolleyGo.ViewModels.Player;

public partial class CreateTeamViewModel : BaseViewModel
{
    public bool IsEditing { get; set; } = false;

    // -----------------------------
    // MODELO
    // -----------------------------
    [ObservableProperty]
    private TeamRequest team = new();

    [ObservableProperty]
    private PlayerRequest player = new();

    // -----------------------------
    // UI STATE
    // -----------------------------
    [ObservableProperty]
    private ImageSource teamLogoPreview = ImageSource.FromFile("default_team.png");

    [ObservableProperty]
    private bool hasUnsavedChanges;

    [ObservableProperty]
    private string formTitle = "Crear equipo";

    [ObservableProperty]
    private string submitButtonText = "Crear equipo";

    [ObservableProperty]
    private string toRegisterChambionshipName;

    [ObservableProperty]
    private int toRegisterChambionshipId;
    // -----------------------------
    // DATA
    // -----------------------------
    [ObservableProperty]
    private List<ChampionshipResponse> championships = [];

    [ObservableProperty]
    private ChampionshipResponse? selectedChampionship;

    partial void OnSelectedChampionshipChanged(ChampionshipResponse? value)
    {
        if (value == null) return;

        Team.ChampionshipId = value.Id;
        HasUnsavedChanges = true;
    }

    // -----------------------------
    // SERVICES
    // -----------------------------
    private readonly TeamService _teamService;
    private readonly ChampionshipService _championshipService;
    private readonly INavigationService _navigationService;
    private readonly CameraService _cameraService;

    public CreateTeamViewModel(
        TeamService teamService,
        ChampionshipService championshipService,
        INavigationService navigationService,
        CameraService cameraService
    )
    {
        _teamService = teamService;
        _championshipService = championshipService;
        _navigationService = navigationService;
        _cameraService = cameraService;
    }

    [RelayCommand]
    private async Task LoadChampionshipId(int championshipId)
    {
        ToRegisterChambionshipId = championshipId;
        Team.ChampionshipId = championshipId;
    }

    [RelayCommand]
    private async Task LoadChampionshipName(string championshipName)
    {
        ToRegisterChambionshipName = championshipName;
    }

    // -----------------------------
    // COMMANDS
    // -----------------------------
    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;

            // VALIDACIONES
            if (string.IsNullOrWhiteSpace(Team.Name) || Team.Name.Length < 3)
                throw new Exception("Debe ingresar un nombre válido (mínimo 3 caracteres).");

            if (Team.ChampionshipId <= 0)
                throw new Exception("Debe seleccionar un campeonato.");

            TeamResponse result;

            if (IsEditing)
            {
                result = await _teamService.UpdateTeam(teamId: 1, request: Team);
            }
            else
            {
                result = await _teamService.CreateTeam(Team);
            }

            DisplayPopup(
                Texts.Success,
                IsEditing ? "Equipo actualizado correctamente" : "Equipo creado correctamente",
                Texts.Accept
            );

            HasUnsavedChanges = false;
            await Task.Delay(2000);
            ShowPopup = false;
            await _navigationService.GoToAsync("..");
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
    private async Task PickLogo()
    {
        var image = await _cameraService.SelecctOrCaptureImage();
        if (image == null) return;

        HasUnsavedChanges = true;

        using var stream = image;
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        // 🔥 persistente, reusable
        Team.LogoBytes = ms.ToArray();

        // UI usa copia independiente
        TeamLogoPreview = ImageSource.FromStream(() =>
            new MemoryStream(Team.LogoBytes));
    }
}
