using VolleyGo.Services;
using VolleyGo.Resources.Languages;
using VolleyGo.Utils;
using VolleyGo.ViewModels;
using VolleyGo.Views.Organizer;
using VolleyGo.Views.Player;

namespace VolleyGo.Views;

public partial class LoadingPage : ContentPage, IQueryAttributable
{
    private readonly AuthenticationService _authenticationServices;
    private readonly LoadingViewModel _viewModel;

    public string RedirectRoutePath { get; set; } = string.Empty;

    public LoadingPage(
        LoadingViewModel loadingViewModel,
        AuthenticationService authenticationServices
        )
    {
        InitializeComponent();
        _authenticationServices = authenticationServices;
        BindingContext = loadingViewModel;

        var organizerMode = Preferences.Get(Consts.OrganizerModeKey, false);

        RedirectRoutePath = organizerMode ? $"//{nameof(HomeOrganizerPage)}" : $"//{nameof(HomePlayerPage)}";

        (Shell.Current as AppShell).ApplyRole(organizerMode ? "organizer" : "player");

        loadingViewModel.Delay = 1000;
        
        _viewModel = loadingViewModel;

        // para una app sin diferentes modos de usuario descomentar esto y reemplazar por lo actual
        // RedirectRoutePath = $"//{nameof(HomePage)}";
        // _viewModel = loadingViewModel;
        //
        // tambien se recomienda simplificar la estructura de directorios
        // ya que en este caso no necesitaremos diferentes carpetas para
        // organizar mejor el codigo

    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("route", out var routeObj) && routeObj is string route && !string.IsNullOrWhiteSpace(route))
        {
            RedirectRoutePath = route;
        }

        if (query.TryGetValue("delay", out var delayObj))
        {
            if (delayObj is int delayInt)
            {
                _viewModel.Delay = delayInt;
            }
            else if (delayObj is string delayStr && int.TryParse(delayStr, out var parsedDelay))
            {
                _viewModel.Delay = parsedDelay;
            }
        }
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        try
        {
            base.OnNavigatedTo(args);

            var result = await _authenticationServices.CheckSession();

            if (result) { _viewModel.RedirectRoutePath = RedirectRoutePath; }
            else { _viewModel.RedirectRoutePath = $"//{nameof(LoginPage)}"; }
        }
        catch (UnauthorizedAccessException ex)
        {
            _viewModel.DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
            _viewModel.RedirectRoutePath = $"//{nameof(LoginPage)}";
        }
        catch (Exception ex)
        {
            _viewModel.DisplayPopup(Texts.Error, ex.Message, Texts.Accept);
            _viewModel.RedirectRoutePath = $"//{nameof(LoginPage)}";
        }
        finally
        {
            _viewModel.IsBusy = false;
        }
    }
}