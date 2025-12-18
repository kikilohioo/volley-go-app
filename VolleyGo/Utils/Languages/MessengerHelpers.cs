using CommunityToolkit.Mvvm.Messaging;
using VolleyGo.Utils.Messages;

namespace VolleyGo.Utils.Languages;

public static class MessengerHelpers
{
    public static void RegisterGoBackButtonAdjust(ContentPage page)
    {
        WeakReferenceMessenger.Default.Register<AdjustGoBackButtonMessage>(page, (r, m) =>
        {
            if (page.FindByName<ImageButton>("GoBackButton") is ImageButton button)
            {
                button.Padding = 7;

                // Delay para forzar redibujado
                Task.Delay(100).ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        button.Padding = 8;
                    });
                });
            }
        });
    }

    public static void UnregisterGoBackButtonAdjust(ContentPage page)
    {
        WeakReferenceMessenger.Default.Unregister<AdjustGoBackButtonMessage>(page);
    }
}
