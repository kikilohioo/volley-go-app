using Microsoft.Extensions.DependencyInjection;
using VolleyGo.Utils;

namespace VolleyGo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var organizerMode = Preferences.Get(Consts.OrganizerModeKey, false);

            var shell = new AppShell();
            shell.ApplyRole(organizerMode ? "organizer" : "player");

            return new Window(shell);
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            // Limpia cualquier focus restante
            Application.Current?.MainPage?.Unfocus();
        }
    }
}