using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyGo.Services;
using System.Collections.ObjectModel;
using VolleyGo.Models.Internal;
using VolleyGo.Resources.Languages;
using VolleyGo.Interfaces;
using VolleyGo.Views;

namespace VolleyGo.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<CarouselModel> imageCollection;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    private readonly AuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;

    public LoginViewModel(
        AuthenticationService authenticationServices,
        INavigationService navigationService
        )
    {
        _authenticationService = authenticationServices;
        _navigationService = navigationService;

        var imageList = new ObservableCollection<CarouselModel>
        {
            new("iaimage1.png"),
            new("iaimage2.png"),
            new("iaimage3.png"),
            new("iaimage4.png"),
            new("iaimage5.png")
        };

        ImageCollection = imageList;
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        try
        {
            if (IsBusy) return;

            IsBusy = true;

            await Task.Delay(1000);
            await _navigationService.GoToAsync(nameof(RegisterPage));
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
    private async Task Login()
    {
        try
        {
            if (IsBusy) return;
            
            IsBusy = true;

            if (Email.Length <= 6 && Password.Length <= 6) throw new Exception("Debe completar ambos campos con al menos 6 caracteres");

            var loggedIn = await _authenticationService.Login(email: Email, password: Password);

            if (loggedIn)
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
