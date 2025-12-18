using Microsoft.Extensions.Configuration;
using VolleyGo.Utils;

namespace VolleyGo.Services;

public class NetworkService
{
    private readonly IConnectivity _connectivity;
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    public NetworkService(
        IConnectivity connectivity,
        HttpClient httpClient,
        IConfiguration config
        )
    {
        _connectivity = connectivity;
        _httpClient = httpClient;
        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
    }

    private bool HasInternet()
    {
        return _connectivity.NetworkAccess == NetworkAccess.Internet;
    }

    public async Task<bool> CanReachServer(int timeoutSeconds = 3)
    {
        if (!HasInternet())
            return false;

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}";
            var response = await _httpClient.GetAsync(uri, cts.Token);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
