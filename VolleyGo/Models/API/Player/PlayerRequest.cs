using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolleyGo.Models.API.Player;

public partial class PlayerRequest : ObservableObject
{
    [JsonProperty("join_code")]
    public string JoinCode { get; set; } = string.Empty;

    [JsonProperty("position")]
    public string? Position { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("jersey_number")]
    public int? JerseyNumber { get; set; }
}
