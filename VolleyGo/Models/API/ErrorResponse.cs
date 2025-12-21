using Newtonsoft.Json;

namespace VolleyGo.Models.API;

public class ErrorResponse
{
    [JsonProperty("detail")]
    public string Message { get; set; }
}
