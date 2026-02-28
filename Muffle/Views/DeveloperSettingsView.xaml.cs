using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class DeveloperSettingsView : ContentView
    {
        public DeveloperSettingsView()
        {
            InitializeComponent();
            BindingContext = new DeveloperSettingsViewModel();
        }

        public DeveloperSettingsView(DeveloperSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
