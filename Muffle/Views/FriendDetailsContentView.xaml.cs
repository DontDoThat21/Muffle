using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muffle.Data.Models;

namespace Muffle.Views;

public partial class FriendDetailsContentView : ContentView
{
    public FriendDetailsContentView(Friend friend)
    {
        InitializeComponent();
        BindingContext = friend;
    }

    private void FriendButton_OnClicked(object sender, EventArgs e)
    {

    }
}