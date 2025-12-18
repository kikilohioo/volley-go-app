using Newtonsoft.Json;
using VolleyGo.Models.API.User;

namespace VolleyGo.Models.API.Team;


public class TeamsResponse
{
    public List<TeamResponse> Teams { get; set; } = new();
}

public class TeamResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonProperty("logo_url")]
    public string? LogoUrl { get; set; }

    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Points { get; set; }

    [JsonProperty("sets_won")]
    public int SetsWon { get; set; }

    [JsonProperty("sets_lost")]
    public int SetsLost { get; set; }

    public UserResponse User { get; set; } = default!;

    [JsonProperty("championship_id")]
    public int ChampionshipId { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // ---------- UI HELPERS ----------

    [JsonIgnore]
    public int MatchesPlayed => Wins + Losses;

    [JsonIgnore]
    public bool HasLogo => !string.IsNullOrEmpty(LogoUrl);
}
