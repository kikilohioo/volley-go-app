using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Reflection;
using VolleyGo.Interfaces;
using VolleyGo.Services;
using VolleyGo.Utils;
using VolleyGo.ViewModels;
using VolleyGo.ViewModels.Organizer;
using VolleyGo.ViewModels.Player;
using VolleyGo.Views;
using VolleyGo.Views.Organizer;
using VolleyGo.Views.Player;

namespace VolleyGo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // asi cargamos las configuraciones a nuestra clase Settings
            var assemblyInstance = Assembly.GetExecutingAssembly();

            // nombre de la aplicación
            var appName = Assembly.GetExecutingAssembly().GetName().Name;

            // Cargar el archivo base (appsettings.json)
            using var baseStream = assemblyInstance.GetManifestResourceStream($"{appName}.appsettings.json");
            var configBuilder = new ConfigurationBuilder().AddJsonStream(baseStream);

            // Cargar la configuración específica del entorno (appsettings.Development.json, Testing, etc.)
            var environment = Settings.Environment;
            var envFilePath = $"{appName}.appsettings.{environment}.json";
            using var envStream = assemblyInstance.GetManifestResourceStream(envFilePath);

            if (envStream != null)
            {
                configBuilder.AddJsonStream(envStream);
            }

            var config = configBuilder.Build();

            LoadPreferences(config);

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiMaps();

            builder.Configuration.AddConfiguration(config);

            /* ##### SERVICES ##### */
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton(Connectivity.Current);
            // esta parte es para compensar que el certificado no se valida correctamente
            HttpClientHandler customHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            builder.Services.AddSingleton(new HttpClient(customHandler));
            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<NetworkService>();
            builder.Services.AddSingleton<CameraService>();
            builder.Services.AddSingleton<ChampionshipService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<TeamService>();
            builder.Services.AddSingleton<PlayerService>();

            /* ##### VIEWS AND VIEW MODELS ##### */
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();

            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddTransient<LoadingPage>();

            builder.Services.AddTransient<HomePlayerViewModel>();
            builder.Services.AddTransient<HomePlayerPage>();

            builder.Services.AddTransient<HomeOrganizerViewModel>();
            builder.Services.AddTransient<HomeOrganizerPage>();

            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();

            builder.Services.AddTransient<ChampionshipOrganizerViewModel>();
            builder.Services.AddTransient<ChampionshipOrganizerPage>();

            builder.Services.AddTransient<ShowChampionshipViewModel>();
            builder.Services.AddTransient<ShowChampionshipPage>();

            builder.Services.AddTransient<CreateChampionshipViewModel>();
            builder.Services.AddTransient<CreateChampionshipPage>();

            builder.Services.AddTransient<UpdateChampionshipViewModel>();
            builder.Services.AddTransient<UpdateChampionshipPage>();

            builder.Services.AddTransient<CreateTeamViewModel>();
            builder.Services.AddTransient<CreateTeamPage>();

            RouteLogger.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            RouteLogger.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            RouteLogger.RegisterRoute(nameof(ChampionshipOrganizerPage), typeof(ChampionshipOrganizerPage));
            RouteLogger.RegisterRoute(nameof(CreateChampionshipPage), typeof(CreateChampionshipPage));
            RouteLogger.RegisterRoute(nameof(UpdateChampionshipPage), typeof(UpdateChampionshipPage));
            RouteLogger.RegisterRoute(nameof(CreateTeamPage), typeof(CreateTeamPage));
            RouteLogger.RegisterRoute(nameof(ShowChampionshipPage), typeof(ShowChampionshipPage));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static void LoadPreferences(IConfigurationRoot config)
        {
            var apiSettingsJson = config.GetSection("ApiSettings").GetChildren()
                                        .ToDictionary(x => x.Key, x => x.Value);

            if (apiSettingsJson != null)
            {
                foreach (var setting in apiSettingsJson)
                {
                    if (setting.Key == "ApiUrl" || setting.Key == "BaseUrl")
                    {
                        var toSetValue = Settings.ForcePreferenceValueFromAppSettings.Contains(setting.Key)
                                ? setting.Value // Forzar el valor desde appsettings.json
                                : Preferences.Get(setting.Key, setting.Value); // Obtener el valor actual si no se fuerza
                        Preferences.Set(setting.Key, toSetValue);
                    }
                }
            }

            var appSettingsJson = config.GetSection("AppSettings").GetChildren()
                                        .ToDictionary(x => x.Key, x => x.Value);
            if (appSettingsJson != null)
            {
                foreach (var setting in appSettingsJson)
                {
                    if (bool.TryParse(setting.Value, out bool boolValue))
                    {
                        var toSetValue = Settings.ForcePreferenceValueFromAppSettings.Contains(setting.Key)
                            ? boolValue // Forzar el valor desde appsettings.json
                            : Preferences.Get(setting.Key, boolValue); // Obtener el valor actual si no se fuerza
                        Preferences.Set(setting.Key, toSetValue);
                    }
                    else if (int.TryParse(setting.Value, out int intValue))
                    {
                        var toSetValue = Settings.ForcePreferenceValueFromAppSettings.Contains(setting.Key)
                            ? intValue // Forzar el valor desde appsettings.json
                            : Preferences.Get(setting.Key, intValue); // Obtener el valor actual si no se fuerza
                        Preferences.Set(setting.Key, toSetValue);
                    }
                    else if (double.TryParse(setting.Value, out double doubleValue))
                    {
                        var toSetValue = Settings.ForcePreferenceValueFromAppSettings.Contains(setting.Key)
                            ? doubleValue // Forzar el valor desde appsettings.json
                            : Preferences.Get(setting.Key, doubleValue); // Obtener el valor actual si no se fuerza
                        Preferences.Set(setting.Key, toSetValue);
                    }
                    else
                    {
                        var toSetValue = Settings.ForcePreferenceValueFromAppSettings.Contains(setting.Key)
                            ? setting.Value // Forzar el valor desde appsettings.json
                            : Preferences.Get(setting.Key, setting.Value); // Obtener el valor actual si no se fuerza
                        Preferences.Set(setting.Key, toSetValue); // Guardar como string si no es un número o booleano
                    }
                }
            }
        }
    }

    public static class RouteLogger
    {
        private static readonly List<string> _registeredRoutes = new();

        public static void RegisterRoute(string route, Type pageType)
        {
            Routing.RegisterRoute(route, pageType);
            _registeredRoutes.Add(route);
            System.Diagnostics.Debug.WriteLine($"[ROUTE REGISTERED] {route} -> {pageType.Name}");
        }

        public static void LogAllRoutes()
        {
            foreach (var route in _registeredRoutes)
            {
                System.Diagnostics.Debug.WriteLine($"[ROUTE] {route}");
            }
        }
    }
}
