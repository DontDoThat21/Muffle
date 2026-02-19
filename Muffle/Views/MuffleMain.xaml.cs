using Muffle.Data.Models;
using Muffle.Data.Services;
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
                friendsTopBarUIView.PendingButtonClicked += FriendsTopBarUIView_PendingButtonClicked;
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
            // Show the Add Friend view
            var addFriendViewModel = new AddFriendViewModel();
            var addFriendView = new AddFriendView(addFriendViewModel);
            
            // Update the main content frame
            MainContentFrame.BackgroundColor = Colors.Transparent;
            MainContentFrame.Content = addFriendView;
        }

        private void FriendsTopBarUIView_PendingButtonClicked(object sender, EventArgs e)
        {
            // Show the Friend Requests view
            var friendRequestsViewModel = new FriendRequestsViewModel();
            var friendRequestsView = new FriendRequestsView(friendRequestsViewModel);
            
            // Update the main content frame
            MainContentFrame.BackgroundColor = Colors.Transparent;
            MainContentFrame.Content = friendRequestsView;
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

        private async void CreateServerButton_OnClicked(object sender, EventArgs e)
        {
            // Show a simple server creation dialog
            string serverName = await DisplayPromptAsync("Create Server", "Enter server name:");
            if (!string.IsNullOrWhiteSpace(serverName))
            {
                string description = await DisplayPromptAsync("Create Server", "Enter server description (optional):", placeholder: "Optional description");

                // Retrieve the current user's ID
                int userId = 1;
                var viewModel = BindingContext as MainPageViewModel;
                if (viewModel?.User != null)
                {
                    userId = viewModel.User.Id;
                }

                var createdServer = UsersService.CreateServer(
                    name: serverName,
                    description: description ?? "",
                    ipAddress: "127.0.0.1", // Default local address
                    port: 8080, // Default port
                    userId: userId
                );

                if (createdServer != null)
                {
                    viewModel?.RefreshServers();
                    await DisplayAlert("Success", $"Server '{serverName}' created successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create server. Please try again.", "OK");
                }
            }
        }

    }

}
