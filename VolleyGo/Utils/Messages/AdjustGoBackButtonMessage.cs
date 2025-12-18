using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VolleyGo.Utils.Messages;

public class AdjustGoBackButtonMessage : ValueChangedMessage<bool>
{
    public AdjustGoBackButtonMessage(bool value) : base(value) { }
}
