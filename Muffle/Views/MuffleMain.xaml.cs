using WebRTCme;
using WebRTCme.Connection;
using WebRTCme.Middleware;
using WebRTCme.SignallingServerProxy;

namespace Muffle
{
    public partial class MainPage : ContentPage
    {
        IWebRtc _webRtc;
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
            DynamicServerContent.Content = null;
            //Populate
            //var webrtc = new WebRtcMiddleware(_webRtc);

        }
    }

}
