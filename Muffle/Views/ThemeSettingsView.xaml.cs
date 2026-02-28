using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class ThemeSettingsView : ContentView
    {
        public ThemeSettingsView()
        {
            InitializeComponent();
            BindingContext = new ThemeSettingsViewModel();
        }

        public ThemeSettingsView(ThemeSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
