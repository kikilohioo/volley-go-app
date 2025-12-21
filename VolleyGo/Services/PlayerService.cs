using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VolleyGo.Models.API;
using VolleyGo.Models.API.Player;
using VolleyGo.Resources.Languages;
using VolleyGo.Utils;

namespace VolleyGo.Services;

public class PlayerService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly NetworkService _networkService;

    public PlayerService(
        HttpClient httpClient,
        IConfiguration config,
        NetworkService networkService)
    {
        _httpClient = httpClient;
        _apiSettings = config
            .GetRequiredSection(nameof(ApiSettings))
            .Get<ApiSettings>()!;
        _networkService = networkService;
    }

    // ---------------------------
    // GET /players
    // ---------------------------
    public async Task<List<PlayerResponse>> GetPlayers(
        int limit = 20,
        int offset = 0)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.PlayersEndpoint}" +
            $"?limit={limit}&offset={offset}";

        var httpResponse = await _httpClient.GetAsync(uri);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception("Error obteniendo jugadores");

        return JsonConvert.DeserializeObject<List<PlayerResponse>>(response) ?? [];
    }

    // ---------------------------
    // GET /players/{id}
    // ---------------------------
    public async Task<PlayerResponse> GetPlayerById(int playerId)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.PlayersEndpoint}/{playerId}";

        var httpResponse = await _httpClient.GetAsync(uri);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception("Error obteniendo jugador");

        return JsonConvert.DeserializeObject<PlayerResponse>(response)!;
    }

    // ---------------------------
    // POST /players/join-team
    // ---------------------------
    public async Task<PlayerResponse> JoinTeamByCode(PlayerRequest request)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.PlayersEndpoint}/join-team";

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(uri, content);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var message = !string.IsNullOrWhiteSpace(response)
                ? response
                : "Error uniéndose al equipo";

            var result = JsonConvert.DeserializeObject<ErrorResponse>(response)!;

            throw new Exception(result.Message);
        }

        return JsonConvert.DeserializeObject<PlayerResponse>(response)!;
    }

    // ---------------------------
    // DELETE /players/{id}
    // ---------------------------
    public async Task LeaveTeam(int playerId)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.PlayersEndpoint}/{playerId}";

        var httpResponse = await _httpClient.DeleteAsync(uri);

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception("Error saliendo del equipo");
    }
}
