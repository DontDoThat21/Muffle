using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.ViewModels;

namespace Muffle.Views;

public partial class MobileMuffleMainPage : ContentPage
{
    private enum ActiveTab { Servers, Friends, Chat }

    private ActiveTab _activeTab = ActiveTab.Servers;
    private object _selectedObject = "friendcategory";
    private FriendDetailsContentViewModel _currentFriendViewModel;

    public MobileMuffleMainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }

    // ── Tab Switching ─────────────────────────────────────────────────────────

    private void SwitchToTab(ActiveTab tab)
    {
        _activeTab = tab;

        ServersPanel.IsVisible           = tab == ActiveTab.Servers;
        FriendsPanel.IsVisible           = tab == ActiveTab.Friends;
        MobileMainContentFrame.IsVisible = tab == ActiveTab.Chat;

        ServersTabButton.BackgroundColor = tab == ActiveTab.Servers ? Color.FromArgb("#7289DA") : Color.FromArgb("#1a1a1a");
        FriendsTabButton.BackgroundColor = tab == ActiveTab.Friends ? Color.FromArgb("#7289DA") : Color.FromArgb("#1a1a1a");
        ChatTabButton.BackgroundColor    = tab == ActiveTab.Chat    ? Color.FromArgb("#7289DA") : Color.FromArgb("#1a1a1a");

        ServersTabButton.TextColor = tab == ActiveTab.Servers ? Colors.White : Color.FromArgb("#aaaaaa");
        FriendsTabButton.TextColor = tab == ActiveTab.Friends ? Colors.White : Color.FromArgb("#aaaaaa");
        ChatTabButton.TextColor    = tab == ActiveTab.Chat    ? Colors.White : Color.FromArgb("#aaaaaa");
    }

    private void ServersTab_OnClicked(object sender, EventArgs e) => SwitchToTab(ActiveTab.Servers);
    private void FriendsTab_OnClicked(object sender, EventArgs e) => SwitchToTab(ActiveTab.Friends);
    private void ChatTab_OnClicked(object sender, EventArgs e)    => SwitchToTab(ActiveTab.Chat);

    // ── Menu Overlay ──────────────────────────────────────────────────────────

    private void MenuButton_OnClicked(object sender, EventArgs e)
    {
        SettingsOverlay.IsVisible = true;
    }

    private void CloseMenu_OnClicked(object sender, EventArgs e)
    {
        SettingsOverlay.IsVisible = false;
    }

    // ── Server & Friend Selection ─────────────────────────────────────────────

    private void ServerButton_OnClicked(object? sender, EventArgs e)
    {
        var selectedItem = (sender as Button)?.BindingContext;
        if (selectedItem != null)
        {
            _selectedObject = selectedItem;
            UpdateTopBarTitle();
            UpdateMainContentFrame();
            SwitchToTab(ActiveTab.Chat);
        }
    }

    private void FriendIndividualButton_OnClicked(object? sender, EventArgs e)
    {
        var selectedItem = (sender as Button)?.BindingContext;
        if (selectedItem != null)
        {
            _selectedObject = selectedItem;
            UpdateTopBarTitle();
            UpdateMainContentFrame();
            SwitchToTab(ActiveTab.Chat);
        }
    }

    private void FriendButton_OnClicked(object? sender, EventArgs e)
    {
        _selectedObject = "friendcategory";
        MobileMainContentFrame.Content = null;
        TopBarTitle.Text = "Friends";
        SwitchToTab(ActiveTab.Chat);
    }

    private void PendingButton_OnClicked(object sender, EventArgs e)
    {
        ShowInContentFrame(new FriendRequestsView(new FriendRequestsViewModel()), "Pending Requests");
    }

    private void BlockedButton_OnClicked(object sender, EventArgs e)
    {
        ShowInContentFrame(new BlockedUsersView(new BlockedUsersViewModel()), "Blocked Users");
    }

    private void AddFriendButton_OnClicked(object sender, EventArgs e)
    {
        ShowInContentFrame(new AddFriendView(new AddFriendViewModel()), "Add Friend");
    }

    private void GroupsButton_OnClicked(object sender, EventArgs e)
    {
        var friendGroupsViewModel = new FriendGroupsViewModel();
        var friendGroupsView = new FriendGroupsView(friendGroupsViewModel);
        friendGroupsView.GroupCallRequested += (s, group) =>
        {
            DisplayAlert("Group Call", $"Starting group call for \"{group.Name}\"\nRoom: {group.RoomKey}", "OK");
        };
        ShowInContentFrame(friendGroupsView, "Groups");
    }

    // ── Content Frame Helpers ─────────────────────────────────────────────────

    private void UpdateMainContentFrame()
    {
        if (_selectedObject is Friend friend)
        {
            _currentFriendViewModel = new FriendDetailsContentViewModel();
            _currentFriendViewModel._friendSelected = friend;
            MobileMainContentFrame.Content = new FriendDetailsContentView(_currentFriendViewModel);
        }
        else if (_selectedObject is Server server)
        {
            _currentFriendViewModel = null;
            MobileMainContentFrame.Content = new ServerDetailsContentView(server);
        }
    }

    private void UpdateTopBarTitle()
    {
        if (_selectedObject is Friend friend)
            TopBarTitle.Text = friend.Name;
        else if (_selectedObject is Server server)
            TopBarTitle.Text = server.Name;
        else
            TopBarTitle.Text = "Friends";
    }

    private void ShowInContentFrame(View view, string title)
    {
        MobileMainContentFrame.BackgroundColor = Colors.Transparent;
        MobileMainContentFrame.Content = view;
        TopBarTitle.Text = title;
        SettingsOverlay.IsVisible = false;
        SwitchToTab(ActiveTab.Chat);
    }

    // ── Server Creation ───────────────────────────────────────────────────────

    private async void CreateServerButton_OnClicked(object sender, EventArgs e)
    {
        string serverName = await DisplayPromptAsync("Create Server", "Enter server name:");
        if (!string.IsNullOrWhiteSpace(serverName))
        {
            string description = await DisplayPromptAsync("Create Server", "Enter server description (optional):", placeholder: "Optional description");

            int userId = 1;
            var viewModel = BindingContext as MainPageViewModel;
            if (viewModel?.User != null)
                userId = viewModel.User.UserId;

            var createdServer = UsersService.CreateServer(
                name: serverName,
                description: description ?? "",
                ipAddress: "127.0.0.1",
                port: 8080,
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

    // ── Settings Handlers ─────────────────────────────────────────────────────

    private void ProfileButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new ProfileSettingsView(new ProfileSettingsViewModel()), "Profile");

    private void ThemesButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new ThemeSettingsView(new ThemeSettingsViewModel()), "Themes");

    private void VoiceSettingsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new VoiceSettingsView(new VoiceSettingsViewModel()), "Voice");

    private void VideoSettingsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new VideoSettingsView(new VideoSettingsViewModel()), "Video");

    private void AccessibilitySettingsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new AccessibilitySettingsView(new AccessibilitySettingsViewModel()), "Accessibility");

    private void DeveloperSettingsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new DeveloperSettingsView(new DeveloperSettingsViewModel()), "Developer");

    private void PrivacySettingsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new PrivacySettingsView(new PrivacySettingsViewModel()), "Privacy");

    private void ConnectedDevicesButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new ConnectedDevicesView(new ConnectedDevicesViewModel()), "Devices");

    private void PatchNotesButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new PatchNotesView(new PatchNotesViewModel()), "Patch Notes");

    private void LibraryAcknowledgementsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new LibraryAcknowledgementsView(new LibraryAcknowledgementsViewModel()), "Credits");

    private void TwoFactorAuthButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new TwoFactorAuthView(new TwoFactorAuthViewModel()), "2FA");

    private void PasswordChangeButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new PasswordChangeView(new PasswordChangeViewModel()), "Password");

    private void PremiumButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new SubscriptionView(new SubscriptionViewModel()), "Premium");

    private void GiftButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new GiftSubscriptionView(new GiftSubscriptionViewModel()), "Gift");

    private void SettingsButton_OnClicked(object sender, EventArgs e) =>
        ShowInContentFrame(new AccountSettingsView(new AccountSettingsViewModel()), "Account Settings");
}
