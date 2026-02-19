using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class BlockedUsersView : ContentView
    {
        public BlockedUsersView()
        {
            InitializeComponent();
            BindingContext = new BlockedUsersViewModel();
        }

        public BlockedUsersView(BlockedUsersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
