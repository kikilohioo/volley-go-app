using System.Globalization;

namespace VolleyGo.Utils.Converters;

public class ChampionshipTypeToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string type)
            return string.Empty;

        return type switch
        {
            "round_robin" => "Todos contra todos",
            "groups_then_semis" => "Grupos y semifinales",
            "groups_then_quarters" => "Grupos, cuartos y semifinales",
            "knockout" => "Eliminación directa",
            _ => "Tipo de campeonato desconocido"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
