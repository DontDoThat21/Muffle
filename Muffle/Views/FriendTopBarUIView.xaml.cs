using Muffle.Data.Models;

namespace Muffle.Views;

public partial class FriendTopBarUIView : ContentView
{
    public FriendTopBarUIView(Friend friend)
    {
        InitializeComponent();
        BindingContext = friend;
    }
}