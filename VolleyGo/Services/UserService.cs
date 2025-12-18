using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VolleyGo.Models.API;
using VolleyGo.Models.API.Auth;
using VolleyGo.Models.API.User;
using VolleyGo.Resources.Languages;
using VolleyGo.Utils;

namespace VolleyGo.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;
    private readonly NetworkService _networkService;

    public UserService(
        HttpClient httpClient, 
        IConfiguration config,
        NetworkService networkService
        )
    {
        _httpClient = httpClient;
        _apiSettings = config.GetRequiredSection(nameof(ApiSettings)).Get<ApiSettings>();
        _networkService = networkService;
    }

    public async Task SetUserRole(string role)
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.UsersEndpoint}/me/set-role";

        var body = new
        {
            role
        };

        var jsonBody = JsonConvert.SerializeObject(body);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PutAsync(uri, content);
        string jsonResult = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(jsonResult);

            throw new UnauthorizedAccessException(errorResult.Message);
        }

        var response = JsonConvert.DeserializeObject<UserResponse>(jsonResult);

        // cargamos actividades, tipos de marcas y la empresa
        await LoadData(response);
    }

    public async Task<UserResponse> GetMe()
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.UsersEndpoint}/me";

        var httpResponse = await _httpClient.GetAsync(uri);
        string jsonResult = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(jsonResult);

            throw new UnauthorizedAccessException(errorResult.Message);
        }

        var response = JsonConvert.DeserializeObject<UserResponse>(jsonResult);

        // cargamos actividades, tipos de marcas y la empresa
        await LoadData(response);

        return response;
    }

    public async Task<UserResponse> UpdateMe(
        string email, 
        string newPassword,
        string fullName,
        Stream? profilePicture
        )
    {
        if (!await _networkService.CanReachServer()) throw new HttpRequestException(Texts.NoNetworkAccess);

        var uri = $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{_apiSettings.UsersEndpoint}/me";
        using var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(email), "email");

        if(newPassword != null)
            formData.Add(new StringContent(newPassword), "new_password");
        
        formData.Add(new StringContent(fullName), "full_name");

        if (profilePicture != null)
        {
            var fileContent = new StreamContent(profilePicture);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            formData.Add(fileContent, "avatar", "foto.jpg");
        }

        var httpResponse = await _httpClient.PutAsync(uri, formData);
        string jsonResult = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(jsonResult);

            throw new UnauthorizedAccessException(errorResult.Message);
        }

        var response = JsonConvert.DeserializeObject<UserResponse>(jsonResult);

        LoadData(response);

        return response;
    }

    private async Task LoadData(UserResponse result)
    {
        Preferences.Set(Consts.UserIdKey, result.Id);
        Preferences.Set(Consts.FullNameKey, result.FullName);
        Preferences.Set(Consts.AvatarUrlKey, string.IsNullOrEmpty(result.AvatarUrl) ? "" : $"{_apiSettings.BaseUrl}{_apiSettings.ApiUrl}{result.AvatarUrl}");
        Preferences.Set(Consts.OrganizerModeKey, result.UserRole == Role.organizer);
    }
}
