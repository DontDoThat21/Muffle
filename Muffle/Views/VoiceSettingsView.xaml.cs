using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class VoiceSettingsView : ContentView
    {
        public VoiceSettingsView()
        {
            InitializeComponent();
            BindingContext = new VoiceSettingsViewModel();
        }

        public VoiceSettingsView(VoiceSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
