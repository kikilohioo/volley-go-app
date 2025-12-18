using Newtonsoft.Json;
using VolleyGo.Models.API.User;

namespace VolleyGo.Models.API.Auth;

public class LoginResponse
{
    [JsonProperty("access_token")]
    public string Token { get; set; }

    [JsonProperty("user")]
    public UserResponse User { get; set; }
}
