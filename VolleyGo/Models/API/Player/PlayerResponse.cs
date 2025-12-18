using Newtonsoft.Json;
using VolleyGo.Models.API.User;

namespace VolleyGo.Models.API.Player;


public class PlayersResponse
{
    public List<PlayerResponse> Players { get; set; } = new();
}

public class PlayerResponse
{
    public int Id { get; set; }

    [JsonProperty("user_id")]
    public int UserId { get; set; }

    [JsonProperty("team_id")]
    public int TeamId { get; set; }

    public string? Position { get; set; }

    [JsonProperty("jersey_number")]
    public int? JerseyNumber { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // --- Relaciones (si tu API las expone) ---
    public UserResponse? User { get; set; }
}
