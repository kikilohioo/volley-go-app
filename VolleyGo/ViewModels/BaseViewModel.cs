using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VolleyGo.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private string loadingMessage;

    [ObservableProperty]
    private string popupTitle;

    [ObservableProperty]
    private string popupMessage;

    [ObservableProperty]
    private string? popupConfirmOption;

    [ObservableProperty]
    private string? popupDeclineOption;

    [ObservableProperty]
    private bool showPopup;

    public void DisplayPopup(
        string title, 
        string message, 
        string confirm = null,
        string decline = null
        )
    {
        PopupTitle = title;
        PopupMessage = message;
        PopupConfirmOption = confirm;
        PopupDeclineOption = decline;
        ShowPopup = true;
    }

    [RelayCommand]
    private void ClosePopup()
    {
        ShowPopup = false;
    }
}