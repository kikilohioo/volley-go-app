using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using VolleyGo.Models.API;
using VolleyGo.Models.API.Auth;
using VolleyGo.Models.API.User;
using VolleyGo.Resources.Languages;
using VolleyGo.Utils;

namespace VolleyGo.Services;

public class AuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly NetworkService _networkService;

    public AuthenticationService(
        HttpClient httpClient,
        IConfiguration config,
        NetworkService networkService
        )
    {
        _httpClient = httpClient;
        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
        _networkService = networkService;
    }

    public async Task<bool> CheckSession()
    {
        var accessToken = Preferences.Get(Consts.AccessTokenKey, string.Empty);

        if (string.IsNullOrEmpty(accessToken))
        {
            Logout();
            return false;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // esto es para que si hay token, como no podemos validar si es valido, mantenemos la sesión ok hasta que puedamos validar en linea
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.CheckSessionEndpoint}";
        var httpResponse = await _httpClient.GetAsync(uri);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Logout();
            throw new UnauthorizedAccessException(Texts.UnathorizedAccesReLogin);
        }

        // si llegó a esta parte todo ok asi que se retorna true directo
        return true;
    }

    public async Task<bool> Login(string email, string password)
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.LoginEndpoint}";
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", password)
        });

        var httpResponse = await _httpClient.PostAsync(uri, formData);
        string jsonResult = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(jsonResult);

            throw new UnauthorizedAccessException(errorResult.Message);
        }

        var response = JsonConvert.DeserializeObject<LoginResponse>(jsonResult);

        // cargamos actividades, tipos de marcas y la empresa
        await LoadData(response);

        return true;
    }

    public async Task<bool> Register(
        string email,
        string password,
        string fullName,
        Stream? profilePicture
        )
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.RegisterEndpoint}";
        using var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(email), "email");
        formData.Add(new StringContent(password), "password");
        formData.Add(new StringContent(fullName), "full_name");

        if (profilePicture != null)
        {
            var fileContent = new StreamContent(profilePicture);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            formData.Add(fileContent, "avatar", "foto.jpg");
        }

        var httpResponse = await _httpClient.PostAsync(uri, formData);
        string jsonResult = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(jsonResult);

            throw new UnauthorizedAccessException(errorResult.Message);
        }

        var response = JsonConvert.DeserializeObject<RegisterResponse>(jsonResult);

        return await Login(email: email, password: password);
    }

    public void Logout()
    {
        Preferences.Remove(Consts.AccessTokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;

        Preferences.Remove(Consts.UserIdKey);
        Preferences.Remove(Consts.FullNameKey);

        Preferences.Set(Consts.OrganizerModeKey, false);
    }

    private async Task LoadData(LoginResponse result)
    {
        // seteamos las preferences del usuario
        Preferences.Set(Consts.AccessTokenKey, result.Token);
        _httpClient.DefaultRequestHeaders.Remove(Consts.HeaderAccessTokenKey);
        _httpClient.DefaultRequestHeaders.Add(Consts.HeaderAccessTokenKey, $"Bearer {result.Token}");

        var user = result.User;

        Preferences.Set(Consts.UserIdKey, user.Id);
        Preferences.Set(Consts.FullNameKey, user.FullName);
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            Preferences.Set(Consts.AvatarUrlKey, $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{user.AvatarUrl}");
        }
        else
        {
            Preferences.Remove(Consts.AvatarUrlKey);
        }
        Preferences.Set(Consts.OrganizerModeKey, user.UserRole == Role.organizer);
    }
}
