using VolleyGo.Interfaces;

namespace VolleyGo.Services;

public class NavigationService : INavigationService
{
    public Task GoToAsync(string uri, bool isModal = false)
    {
        return Shell.Current.GoToAsync(uri, isModal);
    }

    public Task GoToAsync(string uri, IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(uri, parameters);
    }

    public Task PopModalAsync()
    {
        return Shell.Current.Navigation.PopModalAsync();
    }

    public Task PushAsync(Page page)
    {
        return Shell.Current.Navigation.PushAsync(page);
    }

    public Task PushModalAsync(Page page)
    {
        return Shell.Current.Navigation.PushModalAsync(page);
    }
}
