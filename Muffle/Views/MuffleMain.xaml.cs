using Microsoft.EntityFrameworkCore;
using Muffle.Data.Data;
using Muffle.Data.Models;
using Muffle.Views;
using System.Drawing;
using WebRTCme;
using WebRTCme.Connection;
using WebRTCme.Middleware;
using WebRTCme.SignallingServerProxy;

namespace Muffle
{
    public partial class MainPage : ContentPage
    {
        IWebRtc _webRtc;
        private object selectedObject = "friendcategory";

        private readonly SqlServerDbContext _contextSqlServer;
        private readonly SqlLiteDbContext _contextSqlLite;

        public MainPage() // IWebRtc webRtc
        {
            InitializeComponent();

            //_contextSqlServer = sqlServerContext;
            //_contextSqlLite = sqlLiteContext;

            BindingContext = new ViewModels.MainPageViewModel();
            _webRtc = CrossWebRtc.Current;
            WebRtcMiddleware webrtc = new WebRtcMiddleware(_webRtc);

            UpdateSharedUIFrame();

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
                UpdateSharedUIFrame();
            }
            //Populate
            //var webrtc = new WebRtcMiddleware(_webRtc);

        }

        private void FriendButton_OnClicked(object? sender, EventArgs e)
        {
            // Clear existing content
            MainContentFrame.Content = null;
            selectedObject = "friendcategory";
            // Get the selected item from the command parameter
            //var selectedItem = (sender as Button)?.BindingContext;
            //if (selectedItem != null)
            //{
            // Set the selected item
            //selectedObject = selectedItem;

            // Update MainContentFrame with details of the selected item
            UpdateMainContentFrame();
            UpdateSharedUIFrame();
            //}

        }

        private void FriendIndividualButton_OnClicked(object? sender, EventArgs e)
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
                UpdateSharedUIFrame();
            }

        }

        private void UpdateSharedUIFrame()
        {
            if (selectedObject is Friend friend)
            {
                SharedTopBarUI.Content = new FriendDetailTopBarUIView(friend);
            }
            else if (selectedObject is Server server)
            {
                SharedTopBarUI.Content = null;//  new ServerTopBarUIView(server);
                SharedTopBarUI.BackgroundColor = Microsoft.Maui.Graphics.Color.FromHex("#303030");
                //SharedTopBarUI.HeightRequest = 0;
            }
            else if(selectedObject.ToString() == "friendcategory")
            {
                var friendsTopBarUIView = new FriendsTopBarUIView();
                friendsTopBarUIView.FriendAddButtonClicked += FriendsTopBarUIView_FriendAddButtonClicked;
                SharedTopBarUI.Content = friendsTopBarUIView;
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

        private void FriendsTopBarUIView_FriendAddButtonClicked(object sender, EventArgs e)
        {
            // Update the MainContentFrame here
            MainContentFrame.BackgroundColor = Colors.Red; // Example action
            MainContentFrame.Content = new Label { Text = "Friend Added", TextColor = Colors.White };
        }

    }

}
