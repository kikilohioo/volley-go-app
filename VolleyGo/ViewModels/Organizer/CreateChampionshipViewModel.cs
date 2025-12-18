using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;
using VolleyGo.Views;
using VolleyGo.Views.Organizer;

namespace VolleyGo.ViewModels.Organizer;

public class ChampionshipTypeOption
{
    public string Value { get; set; }
    public string Display { get; set; }
}

public class ChampionshipStatusOption
{
    public string Value { get; set; }
    public string Display { get; set; }
}

public partial class CreateChampionshipViewModel : BaseViewModel
{
    public bool IsEditing { get; set; } = false;

    [ObservableProperty]
    private ChampionshipRequest newChampionship = new();

    [ObservableProperty]
    private ImageSource headerImage = ImageSource.FromFile("default_header.png");

    [ObservableProperty]
    private bool hasUnsavedChanges;

    [ObservableProperty]
    private bool showMap;

    [ObservableProperty]
    private bool showMapSearchbar = true;

    [ObservableProperty]
    private string mapTitle = "Seleccionar ubiación";

    [ObservableProperty]
    private bool scrollMapEnabled = true;

    [ObservableProperty]
    private Location location;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationSelected))]
    [NotifyPropertyChangedFor(nameof(LocationNotSelected))]
    private string locationString;

    public bool LocationSelected => !string.IsNullOrWhiteSpace(LocationString);
    public bool LocationNotSelected => string.IsNullOrWhiteSpace(LocationString);

    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private TimeSpan startTime = TimeSpan.FromHours(10); // hora por defecto

    [ObservableProperty]
    private DateTime endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private TimeSpan endTime = TimeSpan.FromHours(12);

    [ObservableProperty]
    private string formTitle = "Crea un campeonato";

    [ObservableProperty]
    private string submitButtonText = "Crear campeonato";

    [ObservableProperty]
    private ChampionshipTypeOption selectedChampionshipTypeObject;

    public List<ChampionshipTypeOption> ChampionshipTypes { get; } = new()
    {
        new() { Value = "round_robin",            Display = "Todos contra todos" },
        new() { Value = "groups_then_semis",      Display = "Grupos y luego semifinales" },
        new() { Value = "groups_then_quarters",   Display = "Grupos, cuartos y luego semis" },
        new() { Value = "knockout",               Display = "Eliminación directa" },
    };

    partial void OnSelectedChampionshipTypeObjectChanged(ChampionshipTypeOption value)
    {
        if (value != null)
        {
            // Guarda solo el valor real que necesita la API
            HasUnsavedChanges = true;
            NewChampionship.Type = value.Value;
        }
    }

    partial void OnStartDateChanged(DateTime value)
    {
        HasUnsavedChanges = true;
        NewChampionship.StartDateTime = value.Date + StartTime;
    }

    partial void OnStartTimeChanged(TimeSpan value)
    {
        HasUnsavedChanges = true;
        NewChampionship.StartDateTime = StartDate.Date + value;
    }

    partial void OnEndDateChanged(DateTime value)
    {
        HasUnsavedChanges = true;
        NewChampionship.EndDateTime = value.Date + EndTime;
    }

    partial void OnEndTimeChanged(TimeSpan value)
    {
        HasUnsavedChanges = true;
        NewChampionship.EndDateTime = EndDate.Date + value;
    }

    private readonly AuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly UserService _userService;
    private readonly ChampionshipService _championshipService;
    private readonly CameraService _cameraService;

    public CreateChampionshipViewModel(
        AuthenticationService authenticationServices,
        INavigationService navigationService,
        UserService userService,
        ChampionshipService championshipService,
        CameraService cameraService
        )
    {
        _authenticationService = authenticationServices;
        _navigationService = navigationService;
        _userService = userService;
        _championshipService = championshipService;
        _cameraService = cameraService;
    }

    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;

            // VALIDACIONES BÁSICAS
            if (string.IsNullOrWhiteSpace(NewChampionship.Name) || NewChampionship.Name.Length < 3)
                throw new Exception("Debe ingresar un nombre válido (mínimo 3 caracteres).");

            if (string.IsNullOrWhiteSpace(NewChampionship.Type))
                throw new Exception("Debe seleccionar un tipo de campeonato.");

            if (string.IsNullOrWhiteSpace(NewChampionship.Location))
                throw new Exception("Debe seleccionar una ubicación.");

            if (NewChampionship.MaxTeams <= 0)
                throw new Exception("Debe ingresar un número válido de equipos.");

            if (NewChampionship.StartDateTime == default)
                throw new Exception("Debe seleccionar una fecha de inicio válida.");

            if (NewChampionship.EndDateTime == default)
                throw new Exception("Debe seleccionar una fecha de finalización válida.");

            // La fecha de inicio debe ser mayor al momento actual
            if (NewChampionship.StartDateTime <= DateTime.UtcNow)
                throw new Exception("La fecha de inicio debe ser mayor a la fecha y hora actual.");

            // La fecha de inicio debe ser anterior a la fecha final
            if (NewChampionship.EndDateTime <= NewChampionship.StartDateTime)
                throw new Exception("La fecha de finalización debe ser posterior a la fecha de inicio.");

            // Llamar API
            var createdChampionship = await _championshipService.CreateChampionship(NewChampionship);

            // si llegó acá, se creó exitosamente
            DisplayPopup(Texts.Success, "Campeonato creado exitosamente", Texts.Accept);
            HasUnsavedChanges = false;
            DisplayPopup(Texts.Success, "Campeonato creado correctamente", Texts.Accept);
            await Task.Delay(2000);
            await _navigationService.GoToAsync("..?refresh=true");
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
    private async Task SetLocation(Location location)
    {
        HasUnsavedChanges = true;
        Location = location;
        LocationString = $"{location.Latitude}:{location.Longitude}";
        NewChampionship.Location = LocationString;
    }

    [RelayCommand]
    private async Task EditHeaderImage()
    {
        var image = await _cameraService.SelecctOrCaptureImage();
        if (image == null) return;

        HasUnsavedChanges = true;
        using var stream = image;
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        // 🔥 persistente, reusable
        NewChampionship.LogoBytes = ms.ToArray();

        // UI usa copia independiente
        HeaderImage = ImageSource.FromStream(() =>
            new MemoryStream(NewChampionship.LogoBytes));
    }

    [RelayCommand]
    private async Task ToggleShowMap()
    {
        ShowMap = !ShowMap;
    }
}
