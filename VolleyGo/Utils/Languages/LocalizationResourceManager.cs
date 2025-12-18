using VolleyGo.Resources.Languages;
using System.ComponentModel;
using System.Globalization;

namespace VolleyGo.Utils.Languages;

public class LocalizationResourceManager : INotifyPropertyChanged
{
    private LocalizationResourceManager()
    {
        var currentSystemCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        var currentLanguage = Preferences.Get(Consts.LanguageKey, currentSystemCulture);
        if(string.IsNullOrEmpty(currentLanguage))
        {
            currentLanguage = currentSystemCulture;
            Preferences.Set(Consts.LanguageKey, currentLanguage);
        }
        var culture = new CultureInfo(currentLanguage);
        Texts.Culture = culture;
    }

    public static LocalizationResourceManager Instance { get; } = new();

    public object this[string resourceKey]
        => Texts.ResourceManager.GetObject(resourceKey, Texts.Culture) ?? Array.Empty<byte>();

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        Texts.Culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
