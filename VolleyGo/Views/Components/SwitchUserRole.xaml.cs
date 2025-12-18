using VolleyGo.Utils;

namespace VolleyGo.Views.Components;

public partial class SwitchUserRole : ContentView
{
	public SwitchUserRole()
	{
		InitializeComponent();

        toggleUserRole?.IsToggled = Preferences.Get(Consts.OrganizerModeKey, false);
    }

    private async void ToggleUserRole_Toggled(object sender, ToggledEventArgs e)
    {
        if (Shell.Current == null) return; 

        var actualMode = Preferences.Get(Consts.OrganizerModeKey, false);
        Preferences.Set(Consts.OrganizerModeKey, !actualMode);
        await Shell.Current.GoToAsync($"//{nameof(LoadingPage)}");
    }
}