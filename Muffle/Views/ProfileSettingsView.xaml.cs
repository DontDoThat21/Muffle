using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class ProfileSettingsView : ContentView
    {
        public ProfileSettingsView()
        {
            InitializeComponent();
            BindingContext = new ProfileSettingsViewModel();
        }

        public ProfileSettingsView(ProfileSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
