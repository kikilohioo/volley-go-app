using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using VolleyGo.Models.API.Championship;
using VolleyGo.Resources.Languages;
using VolleyGo.Utils;

namespace VolleyGo.Services;

public class ChampionshipService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly NetworkService _networkService;

    public ChampionshipService(
        HttpClient httpClient,
        IConfiguration config,
        NetworkService networkService
        )
    {
        _httpClient = httpClient;
        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
        _networkService = networkService;
    }

    public async Task<List<ChampionshipResponse>> GetChampionships(int limit = 10, int offset = 0)
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var queryParameter = $"?limit={limit}&offset={offset}";

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.ChampionshipsEndpoint}/me{queryParameter}";

        var httpResponse = await _httpClient.GetAsync(uri);

        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);
        }
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(Texts.ErrorFetchingChampionships);
        }

        var championships = JsonConvert.DeserializeObject<List<ChampionshipResponse>>(response);

        return championships ?? [];
    }

    public async Task<ChampionshipResponse> GetChampionshipById(int championshipId)
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.ChampionshipsEndpoint}/{championshipId}";
        var httpResponse = await _httpClient.GetAsync(uri);

        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);
        }
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(Texts.ErrorFetchingChampionships);
        }

        var championship = JsonConvert.DeserializeObject<ChampionshipResponse>(response);

        return championship;
    }

    public async Task<ChampionshipResponse> CreateChampionship(ChampionshipRequest request)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.ChampionshipsEndpoint}/";

        using var form = new MultipartFormDataContent();

        // Campos de texto
        form.Add(new StringContent(request.Name), "name");
        form.Add(new StringContent(request.Location), "location");
        form.Add(new StringContent(request.Type), "type");
        form.Add(new StringContent(request.SetsToWin.ToString()), "sets_to_win");
        form.Add(new StringContent(request.PointsPerSet.ToString()), "points_per_set");
        form.Add(new StringContent(request.PlayerCost.ToString()), "player_cost");
        form.Add(new StringContent(request.StartDateTime.ToString("o")), "start_date"); // formato ISO
        form.Add(new StringContent(request.EndDateTime.ToString("o")), "end_date");
        form.Add(new StringContent(request.Description), "description");
        form.Add(new StringContent(request.MaxTeams.ToString()), "max_teams");

        // Archivo (opcional)
        if (request.LogoBytes?.Length > 0)
        {
            var streamContent = new StreamContent(
                new MemoryStream(request.LogoBytes));

            streamContent.Headers.ContentType =
                new MediaTypeHeaderValue("image/jpeg");

            form.Add(streamContent, "logo", "team_logo.jpg");
        }

        var httpResponse = await _httpClient.PostAsync(uri, form);
        var responseString = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var message = !string.IsNullOrWhiteSpace(responseString)
                ? responseString
                : "Error creando campeonato, intentelo nuevamente más tarde.";
            throw new Exception(message);
        }

        return JsonConvert.DeserializeObject<ChampionshipResponse>(responseString)!;
    }

    public async Task<ChampionshipResponse> UpdateChampionship(
        int championshipId,
        ChampionshipRequest request
    )
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.ChampionshipsEndpoint}/{championshipId}";

        using var form = new MultipartFormDataContent();

        // Campos de texto
        form.Add(new StringContent(request.Name), "name");
        form.Add(new StringContent(request.Location), "location");
        form.Add(new StringContent(request.Type), "type");
        form.Add(new StringContent(request.SetsToWin.ToString()), "sets_to_win");
        form.Add(new StringContent(request.PointsPerSet.ToString()), "points_per_set");
        form.Add(new StringContent(request.PlayerCost.ToString()), "player_cost");
        form.Add(new StringContent(request.StartDateTime.ToString("o")), "start_date");
        form.Add(new StringContent(request.EndDateTime.ToString("o")), "end_date");
        form.Add(new StringContent(request.Description), "description");
        form.Add(new StringContent(request?.Status), "status");
        form.Add(new StringContent(request.MaxTeams.ToString()), "max_teams");

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
        var responseString = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var message = !string.IsNullOrWhiteSpace(responseString)
                ? responseString
                : "Error actualizando campeonato, inténtelo nuevamente más tarde.";

            throw new Exception(message);
        }

        return JsonConvert.DeserializeObject<ChampionshipResponse>(responseString)!;
    }

    public async Task<OrganizerDashboardResponse> GetDashboard(int limit = 10, int offset = 0)
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var queryParameter = $"?limit={limit}&offset={offset}";

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.OrganizerDashboardEndpoint}";
        var httpResponse = await _httpClient.GetAsync(uri);

        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);
        }
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(Texts.ErrorFetchingChampionships);
        }

        var dashboard = JsonConvert.DeserializeObject<OrganizerDashboardResponse>(response);

        return dashboard;
    }

    public async Task DeleteChampionship(int championshipId)
    {
        if (!await _networkService.CanReachServer())
            throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri =
            $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}" +
            $"{_apiSettings.ChampionshipsEndpoint}/{championshipId}";

        var httpResponse = await _httpClient.DeleteAsync(uri);

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);

        // 204 = OK (sin contenido)
        if (!httpResponse.IsSuccessStatusCode)
        {
            var response = await httpResponse.Content.ReadAsStringAsync();

            var message = !string.IsNullOrWhiteSpace(response)
                ? response
                : "Error eliminando el campeonato, inténtelo nuevamente más tarde.";

            throw new Exception(message);
        }
    }
}
