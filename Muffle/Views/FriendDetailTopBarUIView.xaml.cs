using Muffle.Data.Models;

namespace Muffle.Views;

public partial class FriendDetailTopBarUIView : ContentView
{
    public event EventHandler VoiceCallRequested;
    public event EventHandler VideoCallRequested;
    public event EventHandler ScreenShareRequested;

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

    private void TopBar_ScreenShareRequested(object sender, EventArgs e)
    {
        ScreenShareRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Updates the screenshare button colour to reflect the current sharing state.</summary>
    public void SetScreenSharingActive(bool active)
    {
        ScreenShareButton.TextColor = active ? Colors.LimeGreen : Colors.Gray;
        ScreenShareButton.BackgroundColor = active ? Color.FromArgb("#1a3a1a") : Color.FromArgb("#303030");
    }
}