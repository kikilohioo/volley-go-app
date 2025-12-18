
using System.Collections;
using VolleyGo.ViewModels;

namespace VolleyGo.Views;

public partial class LoginPage : ContentPage
{
    private int _tickCount = 0;
    private bool _keepTimerRunning = false;
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartAutoSlide();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // esto hará que el callback devuelva false y el timer termine
        _keepTimerRunning = false;
    }

    private void StartAutoSlide()
    {
        // si ya está corriendo, no arrancar otro
        if (_keepTimerRunning)
            return;

        _keepTimerRunning = true;
        _tickCount = 0;

        // intervalo de 1 segundo por tick; retornamos true para continuar,
        // o false para detener el timer cuando _keepTimerRunning sea false.
        Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            // si la página ya no quiere el timer, lo detenemos
            if (!_keepTimerRunning)
                return false;

            _tickCount++;

            if (_tickCount >= 6)
            {
                _tickCount = 0;

                if (carousel.ItemsSource is IList items && items.Count > 0)
                {
                    // avanzamos al índice siguiente usando módulo
                    var nextIndex = (carousel.Position + 1) % items.Count;
                    // Asignar SelectedIndex desde el hilo UI (ya estamos en Dispatcher)
                    carousel.Position = nextIndex;
                }
            }

            return true; // continuar el timer
        });
    }
}
