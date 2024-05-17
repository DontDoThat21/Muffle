using Muffle.Data.Models;
using Muffle.Views;
using WebRTCme;
using WebRTCme.Connection;
using WebRTCme.Middleware;
using WebRTCme.SignallingServerProxy;

namespace Muffle
{
    public partial class MainPage : ContentPage
    {
        IWebRtc _webRtc;
        private object selectedObject;

        public MainPage() // IWebRtc webRtc
        {
            InitializeComponent();
            BindingContext = new ViewModels.MainPageViewModel();
            _webRtc = CrossWebRtc.Current;
            WebRtcMiddleware webrtc = new WebRtcMiddleware(_webRtc);
            //var config = new RTCConfiguration();
            //var peerConnection = webrtc.New

            //_webRtc = webRtc;
        }

        private void ServerButton_OnClicked(object? sender, EventArgs e)
        {
            // Clear existing content
            MainContentFrame.Content = null;
            // Get the selected item from the command parameter
            var selectedItem = (sender as Button)?.BindingContext;
            if (selectedItem != null)
            {
                // Set the selected item
                selectedObject = selectedItem;

                // Update MainContentFrame with details of the selected item
                UpdateMainContentFrame();
            }
            //Populate
            //var webrtc = new WebRtcMiddleware(_webRtc);

        }

        private void FriendButton_OnClicked(object? sender, EventArgs e)
        {
            // Clear existing content
            MainContentFrame.Content = null;
            // Get the selected item from the command parameter
            var selectedItem = (sender as Button)?.BindingContext;
            if (selectedItem != null)
            {
                // Set the selected item
                selectedObject = selectedItem;

                // Update MainContentFrame with details of the selected item
                UpdateMainContentFrame();
            }

        }

        private void UpdateMainContentFrame()
        {
            // Dynamically switch content based on the type of selected item
            if (selectedObject is Friend friend)
            {
                MainContentFrame.Content = new FriendDetailsContentView(friend);
            }
            else if (selectedObject is Server server)
            {
                MainContentFrame.Content = new ServerDetailsContentView(server);
            }
        }

    }

}
