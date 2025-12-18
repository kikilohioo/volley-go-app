using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyGo.Interfaces;
using VolleyGo.Models.Internal;
using VolleyGo.Resources.Languages;
using VolleyGo.Services;
using VolleyGo.Views;

namespace VolleyGo.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string fullName;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string repeatPassword;

    private readonly AuthenticationService _authenticationService;
    private readonly CameraService _cameraService;
    private readonly INavigationService _navigationService;

    public RegisterViewModel(
        AuthenticationService authenticationServices,
        CameraService cameraService,
        INavigationService navigationService
        )
    {
        _authenticationService = authenticationServices;
        _cameraService = cameraService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;
            await Task.Delay(500);
            await _navigationService.GoToAsync($"//{nameof(LoginPage)}");
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
    private async Task Register()
    {
        try
        {
            if (IsBusy) return;
            
            IsBusy = true;

            if (Email == null || Email.Length <= 6) throw new Exception("Debe ingresar un email de al menos 6 caracteres");
            if (FullName == null || FullName.Length <= 3) throw new Exception("Debe ingresar un nombre de al menos 3 caracteres");
            if (Password == null || Password.Length <= 6) throw new Exception("Debe ingresar una contraseña de al menos 6 caracteres");
            if (RepeatPassword == null || RepeatPassword.Length <= 6) throw new Exception("Debe repetir la contraseña");
            if (Password != RepeatPassword) throw new Exception("Las contraseñas no coinciden");

            var image = await _cameraService.SelecctOrCaptureImage();

            var registerdUser = await _authenticationService.Register(
                email: Email, 
                password: Password,
                fullName: FullName,
                profilePicture: image
                );

            if (registerdUser)
            {
                await _navigationService.GoToAsync($"//{nameof(LoadingPage)}");
            }
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
}
