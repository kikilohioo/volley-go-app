using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyGo.Interfaces;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;
using VolleyGo.Utils;
using VolleyGo.Views;
using VolleyGo.Views.Organizer;
using VolleyGo.Views.Player;

namespace VolleyGo.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private Stream? avatarImage;
    
    [ObservableProperty]
    private ImageSource? auxAvatarImage = ImageSource.FromFile("avatar_default.png");

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string fullName;

    [ObservableProperty]
    private bool changePassword = false;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string repeatPassword;

    private readonly AuthenticationService _authenticationService;
    private readonly HttpClient _httpClient;
    private readonly UserService _userService;
    private readonly CameraService _cameraService;
    private readonly INavigationService _navigationService;

    public SettingsViewModel(
        AuthenticationService authenticationServices,
        UserService userService,
        CameraService cameraService,
        INavigationService navigationService,
        HttpClient httpClient
        )
    {
        _authenticationService = authenticationServices;
        _userService = userService;
        _cameraService = cameraService;
        _navigationService = navigationService;
        _httpClient = httpClient;

        GetMeCommand = new Command(async () => await GetMe());
    }

    public Command GetMeCommand { get; set; }

    private async Task GetMe()
    {
        var updatedUser = await _userService.GetMe();
        Email = updatedUser.Email;
        FullName = updatedUser.FullName;

        if (!string.IsNullOrEmpty(updatedUser.AvatarUrl))
        {
            try
            {
                var baseUrl = Preferences.Get(Consts.BaseUrlKey, string.Empty);
                var apiUrl = Preferences.Get(Consts.ApiUrlKey, string.Empty);
                // Construir URL completa
                var fullUrl = $"{baseUrl}{apiUrl}{updatedUser.AvatarUrl}";

                // Descargar imagen
                var bytes = await _httpClient.GetByteArrayAsync(fullUrl);

                // Convertir a MemoryStream
                var ms = new MemoryStream(bytes);
                AvatarImage = ms;

                // Para MAUI necesitamos una copia
                var msCopy = new MemoryStream(bytes);
                AuxAvatarImage = ImageSource.FromStream(() => msCopy);
            }
            catch
            {
                // Si falla la descarga, poner imagen por defecto
                AuxAvatarImage = "avatar_default.png";
            }
        }
        else
        {
            // Si no hay avatar en el usuario
            AuxAvatarImage = "avatar_default.png";
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;
            await Task.Delay(500);

            var organizerMode = Preferences.Get(Consts.OrganizerModeKey, false);
            var uri = organizerMode ? $"//{nameof(HomeOrganizerPage)}" : $"//{nameof(HomePlayerPage)}";
            
            await _navigationService.GoToAsync(uri);
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
    private async Task UpdateMe()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;

            if (Email == null || Email.Length <= 6)
                throw new Exception("Debe ingresar un email de al menos 6 caracteres");
            if (FullName == null || FullName.Length <= 3)
                throw new Exception("Debe ingresar un nombre de al menos 3 caracteres");
            if (ChangePassword && (Password == null || Password.Length <= 6))
                throw new Exception("Debe ingresar una contraseña de al menos 6 caracteres");
            if (ChangePassword && (RepeatPassword == null || RepeatPassword.Length <= 6))
                throw new Exception("Debe repetir la contraseña");
            if (ChangePassword && (Password != RepeatPassword))
                throw new Exception("Las contraseñas no coinciden");

            var registerdUser = await _userService.UpdateMe(
                email: Email,
                newPassword: Password,
                fullName: FullName,
                profilePicture: AvatarImage
                );

            DisplayPopup(Texts.Success, "Usuario actualizado con exito", Texts.Accept);
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
    private async Task EditAvatarImage()
    {
        var image = await _cameraService.SelecctOrCaptureImage();
        if (image == null) return;

        AvatarImage = image;

        // Copia del stream para MAUI
        if (AvatarImage.CanSeek)
            AvatarImage.Position = 0;

        var ms = new MemoryStream();
        AvatarImage.CopyTo(ms);
        ms.Position = 0;

        AuxAvatarImage = ImageSource.FromStream(() => ms);

        // Restaurar original
        if (AvatarImage.CanSeek)
            AvatarImage.Position = 0;
    }

}
