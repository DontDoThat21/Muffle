using Muffle.Data.Models;
using Muffle.ViewModels;

namespace Muffle.Views;

public partial class FriendGroupsView : ContentView
{
    private readonly FriendGroupsViewModel _viewModel;

    public event EventHandler<FriendGroup> GroupCallRequested;

    public FriendGroupsView(FriendGroupsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.GroupCallRequested += (s, group) =>
            GroupCallRequested?.Invoke(this, group);
    }

    private void GroupCallButton_OnClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is FriendGroup group)
            _viewModel.RequestGroupCall(group);
    }

    private void RemoveFriendButton_OnClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is not Friend friend) return;

        // Walk up the visual tree until we find an element whose BindingContext is a FriendGroup
        var current = (sender as Element)?.Parent;
        while (current != null)
        {
            if (current.BindingContext is FriendGroup group)
            {
                _viewModel.RemoveFriendFromGroup(group, friend);
                return;
            }
            current = current.Parent;
        }
    }
}
