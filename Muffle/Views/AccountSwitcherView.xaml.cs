using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class AccountSwitcherView : ContentView
    {
        public AccountSwitcherView()
        {
            InitializeComponent();
            BindingContext = new AccountSwitcherViewModel();
        }

        public AccountSwitcherView(AccountSwitcherViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
