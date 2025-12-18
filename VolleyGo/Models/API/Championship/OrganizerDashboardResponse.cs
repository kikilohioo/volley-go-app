using Newtonsoft.Json;
using VolleyGo.Models.API.Team;

namespace VolleyGo.Models.API.Championship;

public class OrganizerDashboardResponse
{
    public DashboardStats Stats { get; set; } = new();

    [JsonProperty("active_championships")]
    public List<ChampionshipResponse> ActiveChampionships { get; set; } = new();

    [JsonIgnore]
    public bool HasActiveChampionships => ActiveChampionships != null && ActiveChampionships.Count > 0;

    [JsonIgnore]
    public bool HasNotActiveChampionships => !HasActiveChampionships;

    [JsonProperty("next_championships")]
    public List<ChampionshipResponse> NextChampionships { get; set; } = new();

    [JsonIgnore]
    public bool HasNextChampionships => NextChampionships != null && NextChampionships.Count > 0;

    [JsonIgnore]
    public bool HasNotNextChampionships => !HasNextChampionships;
}

public class DashboardStats
{
    public int Active { get; set; }
    public int Finished { get; set; }
    public int Total { get; set; }
}
