using Newtonsoft.Json;

namespace VolleyGo.Models.API.User;

public class UpdateUserRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("full_name")]
    public string FullName { get; set; }

    [JsonProperty("new_password")]
    public string NewPassword { get; set; }

    [JsonProperty("avatar")]
    public string Avatar { get; set; }
}
