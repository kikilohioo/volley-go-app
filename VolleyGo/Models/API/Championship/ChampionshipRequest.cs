using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolleyGo.Models.API.Championship;

public partial class ChampionshipRequest : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
    public int SetsToWin { get; set; } = 1;
    public float PointsPerSet { get; set; } = 0;
    public float PlayerCost { get; set; } = 25;
    public DateTime StartDateTime { get; set; } = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
    public DateTime EndDateTime { get; set; } = DateTime.UtcNow.Date.AddDays(1).AddHours(20);
    public string Description { get; set; } = string.Empty;
    public int MaxTeams { get; set; } = 24;

    // ⚠️ NO se serializa: va por multipart/form-data
    [JsonIgnore]
    [ObservableProperty]
    private byte[]? logoBytes;
}
