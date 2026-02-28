using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class PrivacySettingsView : ContentView
    {
        public PrivacySettingsView()
        {
            InitializeComponent();
            BindingContext = new PrivacySettingsViewModel();
        }

        public PrivacySettingsView(PrivacySettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
