using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using VolleyGo.Models.API.Team;
using VolleyGo.Resources.Languages;
using VolleyGo.Utils;

namespace VolleyGo.Services;

public class TeamService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly NetworkService _networkService;

    public TeamService(
        HttpClient httpClient,
        IConfiguration config,
        NetworkService networkService)
    {
        _httpClient = httpClient;
        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>()!;
        _networkService = networkService;
    }

    public async Task<List<TeamResponse>> GetTeams(
        int championshipId,
        int limit = 20,
        int offset = 0)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.TeamsEndpoint}" +
            $"?championship_id={championshipId}&limit={limit}&offset={offset}";

        var httpResponse = await _httpClient.GetAsync(uri);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception("Error obteniendo equipo");

        return JsonConvert.DeserializeObject<List<TeamResponse>>(response) ?? [];
    }

    public async Task<TeamResponse> GetTeamById(int teamId)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.TeamsEndpoint}/{teamId}";

        var httpResponse = await _httpClient.GetAsync(uri);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception("Error obteniendo el equipo");

        return JsonConvert.DeserializeObject<TeamResponse>(response)!;
    }

    public async Task<TeamResponse> CreateTeam(TeamRequest request)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.TeamsEndpoint}/";

        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(request.Name), "name");
        form.Add(new StringContent(request.ChampionshipId.ToString()), "championship_id");

        // Logo (opcional)
        if (request.LogoBytes?.Length > 0)
        {
            var streamContent = new StreamContent(
                new MemoryStream(request.LogoBytes));

            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue("image/jpeg");

            form.Add(streamContent, "logo", "team_logo.jpg");
        }

        var httpResponse = await _httpClient.PostAsync(uri, form);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var message = !string.IsNullOrWhiteSpace(response)
                ? response
                : "Error creando el equipo";

            throw new Exception(message);
        }

        return JsonConvert.DeserializeObject<TeamResponse>(response)!;
    }

    public async Task<TeamResponse> UpdateTeam(
    int teamId,
    TeamRequest request
)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.TeamsEndpoint}/{teamId}";

        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(request.Name), "name");
        form.Add(new StringContent(request.ChampionshipId.ToString()), "championship_id");

        // Logo (opcional)
        if (request.LogoBytes?.Length > 0)
        {
            var streamContent = new StreamContent(
                new MemoryStream(request.LogoBytes));

            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue("image/jpeg");

            form.Add(streamContent, "logo", "team_logo.jpg");
        }

        var httpResponse = await _httpClient.PutAsync(uri, form);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var message = !string.IsNullOrWhiteSpace(response)
                ? response
                : "Error actualizando equipo";

            throw new Exception(message);
        }

        return JsonConvert.DeserializeObject<TeamResponse>(response)!;
    }
}
