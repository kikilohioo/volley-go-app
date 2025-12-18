using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VolleyGo.Utils.Converters;

public class FullPathConverter : IValueConverter
{

    private string BaseUrl { get; set; }

    public FullPathConverter()
    {
        var baseUrl = Preferences.Get(Consts.BaseUrlKey, string.Empty);
        var apiUrl = Preferences.Get(Consts.ApiUrlKey, string.Empty);

        BaseUrl = baseUrl + apiUrl;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || value == string.Empty)
        {
            return "default_header.png";
        }

        var path = value.ToString();

        // Si ya es absoluta, la devolvemos
        if (path.StartsWith("http") || path.Contains("default_header.png"))
            return path;

        // Si empieza con "/" concatenamos
        var url = $"{BaseUrl}{path}";

        return url;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
