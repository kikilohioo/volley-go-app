using Newtonsoft.Json;

namespace VolleyGo.Models.API;

public class ErrorResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }
}
