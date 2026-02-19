using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class FriendRequestsView : ContentView
    {
        public FriendRequestsView()
        {
            InitializeComponent();
            BindingContext = new FriendRequestsViewModel();
        }

        public FriendRequestsView(FriendRequestsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void IncomingTab_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is FriendRequestsViewModel viewModel)
            {
                viewModel.SelectedTabIndex = 0;
            }
        }

        private void OutgoingTab_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is FriendRequestsViewModel viewModel)
            {
                viewModel.SelectedTabIndex = 1;
            }
        }
    }
}
