using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class ConnectedDevicesView : ContentView
    {
        public ConnectedDevicesView()
        {
            InitializeComponent();
            BindingContext = new ConnectedDevicesViewModel();
        }

        public ConnectedDevicesView(ConnectedDevicesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
