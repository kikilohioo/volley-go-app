using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using VolleyGo.Interfaces;
using VolleyGo.Models.API.Championship;
using VolleyGo.Models.API.User;
using VolleyGo.Services;
using VolleyGo.Utils;

namespace VolleyGo.ViewModels.Organizer;

public partial class UpdateChampionshipViewModel : BaseViewModel
{
    [ObservableProperty]
    private string headerImage = "default_header.png";

    [ObservableProperty]
    private ImageSource headerImageStream = ImageSource.FromFile("default_header.png");

    // =====================================================
    //  PROPIEDADES PARA EL RESUMEN / DASHBOARD
    // =====================================================

    [ObservableProperty] private int id;
    [ObservableProperty] private string name;
    [ObservableProperty] private string location;
    [ObservableProperty] private string type;
    [ObservableProperty] private string status;
    [ObservableProperty] private int maxTeams;
    [ObservableProperty] private string description;
    [ObservableProperty] private string logoUrl;

    // Fechas completas
    [ObservableProperty] private DateTime startDate;
    [ObservableProperty] private DateTime endDate;

    // Organizador
    [ObservableProperty] private int championshipId;
    [ObservableProperty] private int organizerId;
    [ObservableProperty] private string organizerFullName;
    [ObservableProperty] private string organizerAvatarUrl;

    [ObservableProperty]
    private bool isDashboardLoading;

    [ObservableProperty] private ChampionshipRequest newChampionship = new();

    [ObservableProperty] private bool hasUnsavedChanges;
    
    [ObservableProperty] private bool championshipEdited;

    [ObservableProperty] private bool showMap;

    [ObservableProperty] private bool showMapSearchbar = true;

    [ObservableProperty] private string mapTitle = "Seleccionar ubicación";

    [ObservableProperty] private string toRemoveTeamCode;

    [ObservableProperty] private bool scrollMapEnabled = true;

    [ObservableProperty] private Location selectedLocation;

    [ObservableProperty] private UserResponse organizer;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationSelected))]
    [NotifyPropertyChangedFor(nameof(LocationNotSelected))]
    private string locationString;

    public bool LocationSelected => !string.IsNullOrWhiteSpace(LocationString);
    public bool LocationNotSelected => string.IsNullOrWhiteSpace(LocationString);

    // Form fechas (para el editor)
    [ObservableProperty] private DateTime formStartDate;
    [ObservableProperty] private TimeSpan formStartTime;

    [ObservableProperty] private DateTime formEndDate;
    [ObservableProperty] private TimeSpan formEndTime;

    // Tipo de campeonato seleccionado
    [ObservableProperty] private ChampionshipTypeOption selectedChampionshipTypeObject;
    [ObservableProperty] private ChampionshipStatusOption selectedChampionshipStatusObject;

    public List<ChampionshipTypeOption> ChampionshipTypes { get; } = new()
    {
        new() { Value = "round_robin", Display = "Todos contra todos" },
        new() { Value = "groups_then_semis", Display = "Grupos y luego semifinales" },
        new() { Value = "groups_then_quarters", Display = "Grupos, cuartos y luego semis" },
        new() { Value = "knockout", Display = "Eliminación directa" },
    };

    public List<ChampionshipStatusOption> ChampionshipStatus { get; } = new()
    {
        new() { Value = "upcoming",            Display = "Próximamente" },
        new() { Value = "inscriptions_open",   Display = "Inscripciones abiertas" },
        new() { Value = "inscriptions_closed", Display = "Inscripciones cerradas" },
        new() { Value = "ongoing",             Display = "En curso" },
        new() { Value = "completed",           Display = "Finalizado" },
    };

    partial void OnSelectedChampionshipTypeObjectChanged(ChampionshipTypeOption value)
    {
        if (value != null)
        {
            HasUnsavedChanges = true;
            NewChampionship.Type = value.Value;
        }
    }

    partial void OnSelectedChampionshipStatusObjectChanged(ChampionshipStatusOption value)
    {
        if (value != null)
        {
            // Guarda solo el valor real que necesita la API
            HasUnsavedChanges = true;
            NewChampionship.Status = value.Value;
        }
    }

    // Fechas del editor
    partial void OnFormStartDateChanged(DateTime value)
    {
        HasUnsavedChanges = true;
        NewChampionship.StartDateTime = value.Date + FormStartTime;
    }

    partial void OnFormStartTimeChanged(TimeSpan value)
    {
        HasUnsavedChanges = true;
        NewChampionship.StartDateTime = FormStartDate.Date + value;
    }

    partial void OnFormEndDateChanged(DateTime value)
    {
        HasUnsavedChanges = true;
        NewChampionship.EndDateTime = value.Date + FormEndTime;
    }

    partial void OnFormEndTimeChanged(TimeSpan value)
    {
        HasUnsavedChanges = true;
        NewChampionship.EndDateTime = FormEndDate.Date + value;
    }

    [ObservableProperty]
    private string formTitle = "Edita tu campeonato";

    [ObservableProperty]
    private string submitButtonText = "Editar campeonato";

    // =====================================================
    //  SERVICIOS
    // =====================================================

    private readonly ChampionshipService _championshipService;
    private readonly CameraService _cameraService;
    private readonly ApiSettings _apiSettings;
    private readonly INavigationService _navigationService;

    // =====================================================
    //  CONSTRUCTOR
    // =====================================================

    public UpdateChampionshipViewModel(
        ChampionshipService championshipService,
        CameraService cameraService,
        IConfiguration config,
        INavigationService navigationService
    )
    {
        _championshipService = championshipService;
        _cameraService = cameraService;
        _navigationService = navigationService;

        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
    }

    [RelayCommand]
    private async Task LoadChampionship(int id)
    {
        try
        {
            if (IsBusy || IsDashboardLoading) return;
            IsDashboardLoading = true;

            var result = await _championshipService.GetChampionshipById(id);

            LoadFromResponse(result);
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsDashboardLoading = false;
            HasUnsavedChanges = false;
        }
    }

    public void SetHeaderImage(ChampionshipResponse result)
    {
        if (!string.IsNullOrWhiteSpace(result.LogoUrl))
        {
            var fullUrl = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{result.LogoUrl}";
            HeaderImageStream = ImageSource.FromUri(new Uri(fullUrl));
        }
        else
        {
            HeaderImageStream = ImageSource.FromFile("default_header.png");
        }
    }

    [RelayCommand]
    public void LoadFromResponse(ChampionshipResponse data)
    {
        // Resumen
        NewChampionship = new ChampionshipRequest
        {
            Name = data.Name,
            Description = data.Description,
            Location = data.Location,
            MaxTeams = data.MaxTeams,
            PointsPerSet = data.PointsPerSet,
            Type = data.Type,
            PlayerCost = data.PlayerCost,
            StartDateTime = data.StartDate,
            EndDateTime = data.EndDate
        };

        Id = data.Id;
        HeaderImage = data.LogoUrl ?? "default_header.png";
        SetHeaderImage(data);
        ChampionshipId = data.Id;
        Organizer = data.Organizer;
        EndDate = data.EndDate;
        StartDate = data.StartDate;

        // Replace this line in LoadChampionship method:
        // SelectedChampionshipTypeObject = result.Type;

        // With the following code to fix CS0029:
        SelectedChampionshipTypeObject = ChampionshipTypes.FirstOrDefault(x => x.Value == data.Type);
        SelectedChampionshipStatusObject = ChampionshipStatus.FirstOrDefault(x => x.Value == data.Status);

        // Para controles de fecha/hora del form
        FormStartDate = data.StartDate.Date;
        FormStartTime = data.StartDate.TimeOfDay;

        FormEndDate = data.EndDate.Date;
        FormEndTime = data.EndDate.TimeOfDay;

        LocationString = data.Location;
    }

    [RelayCommand]
    private void ToggleShowMap()
    {
        ShowMap = !ShowMap;
    }

    [RelayCommand]
    private async Task SetLocation(Location location)
    {
        HasUnsavedChanges = true;
        SelectedLocation = location;

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
        HeaderImageStream = ImageSource.FromStream(() =>
            new MemoryStream(NewChampionship.LogoBytes));
    }

    [RelayCommand]
    private async Task RemoveTeam()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;

            //await _championshipService.RemoveTeam(teamCode: ToRemoveTeamCode);

            DisplayPopup("Éxito", "Equipo elimnado correctamente.", "OK");
        }
        catch (Exception ex)
        {
            DisplayPopup("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateTeam()
    {
        try
        {
            if (IsBusy) return;
            IsBusy = true;

            //await _championshipService.RemoveTeam(Id, NewChampionship);

            DisplayPopup("Éxito", "Equipo elimnado correctamente.", "OK");
        }
        catch (Exception ex)
        {
            DisplayPopup("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
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

            await _championshipService.UpdateChampionship(Id, NewChampionship);

            HasUnsavedChanges = false;
            ChampionshipEdited = true;

            DisplayPopup("Éxito", "Campeonato actualizado correctamente.", "OK");
        }
        catch (Exception ex)
        {
            DisplayPopup("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
