using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class AddFriendView : ContentView
    {
        public AddFriendView()
        {
            InitializeComponent();
            BindingContext = new AddFriendViewModel();
        }

        public AddFriendView(AddFriendViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
