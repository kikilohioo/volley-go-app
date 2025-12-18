using CommunityToolkit.Mvvm.ComponentModel;
using VolleyGo.Interfaces;

namespace VolleyGo.ViewModels;

public partial class LoadingViewModel : BaseViewModel
{
    [ObservableProperty]
    private string loadingText;

    [ObservableProperty]
    private string redirectRoutePath;

    [ObservableProperty]
    private int delay = 0;

    private readonly INavigationService _navigationService;

    public LoadingViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        IsBusy = true;
        LoadingTextEffect();
        PropertyChanged += LoadingViewModel_PropertyChanged;
    }

    private async void LoadingViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RedirectRoutePath))
        {
            await Task.Delay(Delay);
            await _navigationService.GoToAsync(RedirectRoutePath);
            IsBusy = false;
        }
    }

    public async void LoadingTextEffect()
    {
        while (IsBusy)
        {
            LoadingText = ". ";
            await Task.Delay(500);
            LoadingText += ". ";
            await Task.Delay(500);
            LoadingText += ". ";
            await Task.Delay(500);
            LoadingText = "";
            await Task.Delay(500);
        }
    }
}
