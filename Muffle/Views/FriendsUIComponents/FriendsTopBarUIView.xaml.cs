namespace Muffle.Views;

public partial class FriendsTopBarUIView : ContentView
{
    public event EventHandler FriendAddButtonClicked;
    public event EventHandler PendingButtonClicked;

    public FriendsTopBarUIView()
	{
		InitializeComponent();
	}

    private void FriendAddButton_OnClicked(object sender, EventArgs e)
    {
        // Raise the event
        FriendAddButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    private void PendingButton_OnClicked(object sender, EventArgs e)
    {
        // Raise the event
        PendingButtonClicked?.Invoke(this, EventArgs.Empty);
    }
}