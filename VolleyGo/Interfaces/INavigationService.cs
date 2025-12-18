namespace VolleyGo.Interfaces;

public interface INavigationService
{
    Task GoToAsync(string uri, bool isModal = false);
    Task GoToAsync(string uri, IDictionary<string, object> parameters);
    Task PushAsync(Page page);
    Task PushModalAsync(Page page);
    Task PopModalAsync();
}
