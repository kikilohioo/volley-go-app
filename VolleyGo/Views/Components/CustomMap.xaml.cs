using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Windows.Input;

namespace VolleyGo.Views.Components;

public partial class CustomMap : ContentView
{
    public static readonly BindableProperty SetLocationProperty =
    BindableProperty.Create(
        nameof(SetLocation),
        typeof(Location),
        typeof(CustomMap),
        default(Location),
        propertyChanged: OnSetLocationChanged);

    public Location? SetLocation
    {
        get => (Location?)GetValue(SetLocationProperty);
        set => SetValue(SetLocationProperty, value);
    }

    public event EventHandler<Location>? LocationSelected;

    public CustomMap()
	{
		InitializeComponent();
        SearchLocationBar.SearchCommand = SearchLocationCommand;
    }

    // 🔍 Buscar dirección
    public ICommand SearchLocationCommand => new Command(async () =>
    {
        var query = SearchLocationBar.Text;
        if (string.IsNullOrWhiteSpace(query))
            return;

        var locations = await Geocoding.GetLocationsAsync(query);
        var location = locations?.FirstOrDefault();
        if (location == null)
            return;

        // mover mapa al resultado
        MoveMap(location);
    });

    private static void OnSetLocationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is not Location location)
            return;

        var control = (CustomMap)bindable;
        control.MoveMap(location);
    }

    // 📌 Usuario toca el mapa → mover pin a esa posición
    private void Map_MapClicked(object sender, MapClickedEventArgs e)
    {
        MoveMap(e.Location);
    }

    // 📍 Centro visual + enviar ubicación al padre
    private void MoveMap(Location location)
    {
        MainMap.MoveToRegion(
            MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.5))
        );

        MainMap.Pins.Clear();

        Pin pin = new Pin
        {
            Label = "Ubicación seleccionada",
            Type = PinType.Place,
            Location = location
        };

        MainMap.Pins.Add(pin);

        LocationSelected?.Invoke(this, location);
    }

    // 📦 Método para que el padre reciba ubicación final al guardar
    public Location GetSelectedLocation()
    {
        var center = MainMap.VisibleRegion.Center;
        return new Location(center.Latitude, center.Longitude);
    }
}