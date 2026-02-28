using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class ServerBrowserView : ContentPage
    {
        public ServerBrowserView()
        {
            InitializeComponent();
            BindingContext = new ServerBrowserViewModel();
        }
    }
}
