using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.ViewModels;

namespace Muffle.Views;

public partial class FriendDetailsContentView : ContentView
{
    private WebSocketService _webSocketService;

    public FriendDetailsContentView(FriendDetailsContentViewModel friendDetailsModel)
    {
        InitializeComponent();
        BindingContext = friendDetailsModel;
    }

    private void FriendButton_OnClicked(object sender, EventArgs e)
    {

    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        if (sender is Entry entry && BindingContext is FriendDetailsContentViewModel viewModel)
        {
            viewModel.SendMessage(entry.Text);
        }
    }
}