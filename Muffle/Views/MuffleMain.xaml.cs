using Muffle.Data.Models;
using Muffle.ViewModels;
using Muffle.Views;
using WebRTCme;

namespace Muffle
{
    public partial class MainPage : ContentPage
    {
        //IWebRtc _webRtc;
        private object selectedObject = "friendcategory";
        private FriendDetailsContentViewModel _currentFriendViewModel;

        public MainPage() // IWebRtc webRtc
        {
            InitializeComponent();

            //_contextSqlServer = sqlServerContext;
            //_contextSqlLite = sqlLiteContext;

            BindingContext = new ViewModels.MainPageViewModel();
            //_webRtc = CrossWebRtc.Current;

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

        private void UpdateSharedUIFrame()
        {
            if (selectedObject is Friend friend)
            {
                var friendDetailTopBarUIView = new FriendDetailTopBarUIView(friend);
                friendDetailTopBarUIView.VoiceCallRequested += FriendDetailTopBarUIView_VoiceCallRequested;
                friendDetailTopBarUIView.VideoCallRequested += FriendDetailTopBarUIView_VideoCallRequested;
                SharedTopBarUI.Content = friendDetailTopBarUIView;                
            }
            else if (selectedObject is Server server)
            {
                SharedTopBarUI.Content = null;//  new ServerTopBarUIView(server);
                SharedTopBarUI.BackgroundColor = Color.FromHex("#303030");
                //SharedTopBarUI.HeightRequest = 0;
            }
            else if(selectedObject.ToString() == "friendcategory")
            {
                var friendsTopBarUIView = new FriendsTopBarUIView();
                friendsTopBarUIView.FriendAddButtonClicked += FriendsTopBarUIView_FriendAddButtonClicked;
                SharedTopBarUI.Content = friendsTopBarUIView;

                //SharedLeftPanelUI.Content = new FriendDetailLeftPanelUIView();
            }
        }

        private void UpdateMainContentFrame()
        {
            // Dynamically switch content based on the type of selected item
            if (selectedObject is Friend friend)
            {
                _currentFriendViewModel = new FriendDetailsContentViewModel();
                _currentFriendViewModel._friendSelected = friend;
                MainContentFrame.Content = new FriendDetailsContentView(_currentFriendViewModel);
            }
            else if (selectedObject is Server server)
            {
                _currentFriendViewModel = null; // Clear friend view model when switching to server
                MainContentFrame.Content = new ServerDetailsContentView(server);
            }
        }

        private void FriendsTopBarUIView_FriendAddButtonClicked(object sender, EventArgs e)
        {
            // Update the MainContentFrame here
            MainContentFrame.BackgroundColor = Colors.Red; // Example action
            MainContentFrame.Content = new Label { Text = "Friend Added", TextColor = Colors.White };
        }

        private async void FriendDetailTopBarUIView_VoiceCallRequested(object sender, EventArgs e)
        {
            if (_currentFriendViewModel != null)
            {
                await _currentFriendViewModel.StartVoiceCallAsync();
            }
        }

        private async void FriendDetailTopBarUIView_VideoCallRequested(object sender, EventArgs e)
        {
            if (_currentFriendViewModel != null)
            {
                await _currentFriendViewModel.StartVideoCallAsync();
            }
        }

    }

}
