using Newtonsoft.Json;

namespace VolleyGo.Models.API.User;

public enum Role
{
    organizer,
    player
}

public class UserResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("full_name")] 
    public string FullName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("role")]
    public Role UserRole { get; set; }
}
