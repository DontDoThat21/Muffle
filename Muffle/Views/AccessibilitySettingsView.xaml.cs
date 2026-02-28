using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class AccessibilitySettingsView : ContentView
    {
        public AccessibilitySettingsView()
        {
            InitializeComponent();
            BindingContext = new AccessibilitySettingsViewModel();
        }

        public AccessibilitySettingsView(AccessibilitySettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
