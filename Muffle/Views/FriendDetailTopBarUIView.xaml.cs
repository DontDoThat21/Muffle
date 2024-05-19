using Muffle.Data.Models;

namespace Muffle.Views;

public partial class FriendDetailTopBarUIView : ContentView
{
    public FriendDetailTopBarUIView(Friend friend)
    {
        InitializeComponent();
        BindingContext = friend;
    }
}