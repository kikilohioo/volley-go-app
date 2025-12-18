using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolleyGo.Models.API.Team;

public partial class TeamRequest : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    [JsonProperty("championship_id")]
    public int ChampionshipId { get; set; }

    // ⚠️ NO se serializa: va por multipart/form-data
    [JsonIgnore]
    [ObservableProperty]
    private byte[]? logoBytes;
}
