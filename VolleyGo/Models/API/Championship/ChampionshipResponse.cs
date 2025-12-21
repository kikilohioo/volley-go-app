using Newtonsoft.Json;
using VolleyGo.Models.API.Team;
using VolleyGo.Models.API.User;

namespace VolleyGo.Models.API.Championship;


public class ChampionshipsResponse
{
    public List<ChampionshipResponse> Championships { get; set; } = new();
}

public class ChampionshipResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    [JsonProperty("sets_to_win")]
    public int SetsToWin { get; set; }

    [JsonProperty("points_per_set")]
    public int PointsPerSet { get; set; }

    [JsonProperty("player_cost")]
    public float PlayerCost { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    [JsonIgnore]
    public bool HasLocation => !string.IsNullOrEmpty(Location);

    [JsonProperty("logo_url")]
    public string LogoUrl { get; set; } = string.Empty;
    
    [JsonProperty("can_register")]
    public bool? CanRegister { get; set; } = false;
    
    [JsonProperty("is_registered")]
    public bool? IsRegistered { get; set; } = false;
    
    [JsonProperty("my_team_id")]
    public int? MyTeamId { get; set; }

    [JsonProperty("max_teams")]
    public int MaxTeams { get; set; }

    [JsonProperty("start_date")]
    public DateTime StartDate { get; set; }

    [JsonProperty("end_date")]
    public DateTime EndDate { get; set; }

    [JsonProperty("organizer_id")]
    public int? OrganizerId { get; set; }

    public UserResponse? Organizer { get; set; }

    public string Status { get; set; } = string.Empty;

    public TeamsResponse Teams { get; set; } = new();

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
