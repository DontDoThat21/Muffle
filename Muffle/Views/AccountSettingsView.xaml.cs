using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class AccountSettingsView : ContentView
    {
        public AccountSettingsView()
        {
            InitializeComponent();
            BindingContext = new AccountSettingsViewModel();
        }

        public AccountSettingsView(AccountSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
