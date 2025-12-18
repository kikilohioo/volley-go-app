using Newtonsoft.Json;

namespace VolleyGo.Models.API.Auth;

public class LoginRequest
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
}
