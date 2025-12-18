using Microsoft.Extensions.Configuration;

namespace VolleyGo.Utils;

public class Settings
{
    //public const string Environment = "Production";
    public const string Environment = "Development";
    //public const string Environment = "Testing";
    public static List<string> ForcePreferenceValueFromAppSettings =
    [
        Consts.BaseUrlKey
    ];
}

public class ApiSettings
{
    public string BaseUrl { get; set; }
    public string ApiUrl { get; set; }
    public string LoginEndpoint { get; set; }
    public string RegisterEndpoint { get; set; }
    public string CheckSessionEndpoint { get; set; }
    public string ChampionshipsEndpoint { get; set; }
    public string OrganizerDashboardEndpoint { get; set; }
    public string UsersEndpoint { get; set; }
    public string TeamsEndpoint { get; set; }
    public string PlayersEndpoint { get; set; }
}

public class AppSettings
{
    public int Language { get; set; }
}

public class Consts
{
    #region numeric constants
    #endregion

    #region date formats
    public const string YYYYMMDD_HHIISS = "yyyy-MM-dd HH:mm:ss";
    public const string H_YYYYMMDD_HHIISS = "dd/MM/yyyy HH:mm:ss";
    public const string YYYY_MM_DD_HH_II_SS = "yyyy-MM-dd_HH-mm-ss";
    public const string YYYYMMDD_HHIISSFFFF = "yyyy-MM-dd HH:mm:ss.fff";
    public const string DDD_DDMMMMYYYY = "ddd, dd MMMM yyyy";
    public const string HHMMSS = "HH:mm:ss";
    #endregion

    #region colors
    public const string ErrorLightRed = "#ffa8a8";
    public const string SuccesLightGreen = "#99f7b8";
    public const string ErrorRed = "#d63c3c";
    public const string SuccesGreen = "#3cd66f";
    #endregion

    #region settings keys for match appsettings.json y las clases AppSettings y ApiSettings
    public const string BaseUrlKey = "BaseUrl";
    public const string ApiUrlKey = "ApiUrl";
    public const string AccessTokenKey = "AccessToken";
    public const string HeaderAccessTokenKey = "Authorization";

    public const string OrganizerModeKey = "OrganizerMode";

    public const string AuthEndpointKey = "AuthEndpoint";
    public const string RegisterEndpointKey = "RegisterEndpoint";
    public const string CheckSessionEndpointKey = "CheckSessionEndpoint";

    public const string LanguageKey = "Language";

    public const string FullNameKey = "FullName";
    public const string AvatarUrlKey = "AvatarUrl";
    public const string UserIdKey = "UserId";
    #endregion
}