using Muffle.Data.Models;

namespace Muffle.Views;

public partial class FriendDetailTopBarUIView : ContentView
{
    public event EventHandler VoiceCallRequested;
    public event EventHandler VideoCallRequested;

    public FriendDetailTopBarUIView(Friend friend)
    {
        InitializeComponent();
        BindingContext = friend;
    }

    private void TopBar_VoiceCallRequested(object sender, EventArgs e)
    {
        VoiceCallRequested?.Invoke(this, EventArgs.Empty);
    }

    private void TopBar_VideoCallRequested(object sender, EventArgs e)
    {
        VideoCallRequested?.Invoke(this, EventArgs.Empty);
    }
}