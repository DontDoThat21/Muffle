using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class VideoSettingsView : ContentView
    {
        public VideoSettingsView()
        {
            InitializeComponent();
            BindingContext = new VideoSettingsViewModel();
        }

        public VideoSettingsView(VideoSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
