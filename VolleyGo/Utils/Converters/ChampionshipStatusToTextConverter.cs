using System.Globalization;

namespace VolleyGo.Utils.Converters;

public class ChampionshipStatusToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string status)
            return string.Empty;

        return status switch
        {
            "upcoming" => "Próximo",
            "inscriptions_open" => "Inscripciones abiertas",
            "inscriptions_closed" => "Inscripciones cerradas",
            "ongoing" => "En curso",
            "completed" => "Finalizado",
            _ => "Estado desconocido"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
