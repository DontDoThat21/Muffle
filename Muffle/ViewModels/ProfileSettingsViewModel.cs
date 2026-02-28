using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for profile settings — edit profile, status, connections, and active status
    /// </summary>
    public class ProfileSettingsViewModel : BindableObject
    {
        private User? _currentUser;
        private string _avatarUrl = string.Empty;
        private string _bannerUrl = string.Empty;
        private string _aboutMe = string.Empty;
        private string _pronouns = string.Empty;
        private UserStatus _selectedStatus = UserStatus.Online;
        private string _customStatusText = string.Empty;
        private string _customStatusEmoji = string.Empty;
        private bool _showOnlineStatus = true;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;
        private string _selectedTab = "Profile";

        // Connection fields
        private string _newConnectionService = string.Empty;
        private string _newConnectionUsername = string.Empty;
        private string _newConnectionUrl = string.Empty;

        public User? CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public string AvatarUrl
        {
            get => _avatarUrl;
            set { _avatarUrl = value; OnPropertyChanged(); }
        }

        public string BannerUrl
        {
            get => _bannerUrl;
            set { _bannerUrl = value; OnPropertyChanged(); }
        }

        public string AboutMe
        {
            get => _aboutMe;
            set { _aboutMe = value; OnPropertyChanged(); }
        }

        public string Pronouns
        {
            get => _pronouns;
            set { _pronouns = value; OnPropertyChanged(); }
        }

        public UserStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusDisplayName));
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public string CustomStatusText
        {
            get => _customStatusText;
            set { _customStatusText = value; OnPropertyChanged(); }
        }

        public string CustomStatusEmoji
        {
            get => _customStatusEmoji;
            set { _customStatusEmoji = value; OnPropertyChanged(); }
        }

        public bool ShowOnlineStatus
        {
            get => _showOnlineStatus;
            set { _showOnlineStatus = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsProfileTabSelected));
                OnPropertyChanged(nameof(IsStatusTabSelected));
                OnPropertyChanged(nameof(IsConnectionsTabSelected));
                OnPropertyChanged(nameof(IsActiveStatusTabSelected));
            }
        }

        public bool IsProfileTabSelected => SelectedTab == "Profile";
        public bool IsStatusTabSelected => SelectedTab == "Status";
        public bool IsConnectionsTabSelected => SelectedTab == "Connections";
        public bool IsActiveStatusTabSelected => SelectedTab == "ActiveStatus";

        public string StatusDisplayName => UserStatusService.GetStatusDisplayName(SelectedStatus);
        public string StatusColor => UserStatusService.GetStatusColor(SelectedStatus);

        public string CurrentUserFullUsername => CurrentUser?.FullUsername ?? string.Empty;

        public ObservableCollection<ProfileConnection> Connections { get; set; } = new();
        public ObservableCollection<string> AvailableServices { get; set; } = new();

        public string NewConnectionService
        {
            get => _newConnectionService;
            set { _newConnectionService = value; OnPropertyChanged(); }
        }

        public string NewConnectionUsername
        {
            get => _newConnectionUsername;
            set { _newConnectionUsername = value; OnPropertyChanged(); }
        }

        public string NewConnectionUrl
        {
            get => _newConnectionUrl;
            set { _newConnectionUrl = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand SaveProfileCommand { get; }
        public ICommand SaveStatusCommand { get; }
        public ICommand ClearCustomStatusCommand { get; }
        public ICommand SetStatusOnlineCommand { get; }
        public ICommand SetStatusAwayCommand { get; }
        public ICommand SetStatusDndCommand { get; }
        public ICommand SetStatusInvisibleCommand { get; }
        public ICommand AddConnectionCommand { get; }
        public ICommand RemoveConnectionCommand { get; }
        public ICommand ToggleConnectionVisibilityCommand { get; }
        public ICommand SaveActiveStatusCommand { get; }
        public ICommand SelectProfileTabCommand { get; }
        public ICommand SelectStatusTabCommand { get; }
        public ICommand SelectConnectionsTabCommand { get; }
        public ICommand SelectActiveStatusTabCommand { get; }

        public ProfileSettingsViewModel()
        {
            SaveProfileCommand = new Command(async () => await SaveProfileAsync());
            SaveStatusCommand = new Command(async () => await SaveStatusAsync());
            ClearCustomStatusCommand = new Command(async () => await ClearCustomStatusAsync());
            SetStatusOnlineCommand = new Command(() => SelectedStatus = UserStatus.Online);
            SetStatusAwayCommand = new Command(() => SelectedStatus = UserStatus.Away);
            SetStatusDndCommand = new Command(() => SelectedStatus = UserStatus.DoNotDisturb);
            SetStatusInvisibleCommand = new Command(() => SelectedStatus = UserStatus.Invisible);
            AddConnectionCommand = new Command(async () => await AddConnectionAsync());
            RemoveConnectionCommand = new Command<ProfileConnection>(async (conn) => await RemoveConnectionAsync(conn));
            ToggleConnectionVisibilityCommand = new Command<ProfileConnection>(async (conn) => await ToggleConnectionVisibilityAsync(conn));
            SaveActiveStatusCommand = new Command(async () => await SaveActiveStatusAsync());
            SelectProfileTabCommand = new Command(() => SelectedTab = "Profile");
            SelectStatusTabCommand = new Command(() => SelectedTab = "Status");
            SelectConnectionsTabCommand = new Command(() => SelectedTab = "Connections");
            SelectActiveStatusTabCommand = new Command(() => SelectedTab = "ActiveStatus");

            foreach (var service in ProfileConnectionService.SupportedServices)
            {
                AvailableServices.Add(service);
            }

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                CurrentUser = CurrentUserService.CurrentUser;

                if (CurrentUser != null)
                {
                    var profile = UserProfileService.GetProfile(CurrentUser.UserId);
                    if (profile != null)
                    {
                        AvatarUrl = profile.AvatarUrl ?? string.Empty;
                        BannerUrl = profile.BannerUrl ?? string.Empty;
                        AboutMe = profile.AboutMe ?? string.Empty;
                        Pronouns = profile.Pronouns ?? string.Empty;
                        SelectedStatus = profile.UserStatus;
                        CustomStatusText = profile.CustomStatusText ?? string.Empty;
                        CustomStatusEmoji = profile.CustomStatusEmoji ?? string.Empty;
                        ShowOnlineStatus = profile.ShowOnlineStatus;
                    }

                    LoadConnections();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading profile: {ex.Message}";
            }
        }

        private void LoadConnections()
        {
            if (CurrentUser == null) return;

            Connections.Clear();
            var connections = ProfileConnectionService.GetConnections(CurrentUser.UserId);
            foreach (var conn in connections)
            {
                Connections.Add(conn);
            }
        }

        private async Task SaveProfileAsync()
        {
            if (CurrentUser == null) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            try
            {
                var success = UserProfileService.UpdateProfile(
                    CurrentUser.UserId,
                    string.IsNullOrWhiteSpace(AvatarUrl) ? null : AvatarUrl,
                    string.IsNullOrWhiteSpace(BannerUrl) ? null : BannerUrl,
                    string.IsNullOrWhiteSpace(AboutMe) ? null : AboutMe,
                    string.IsNullOrWhiteSpace(Pronouns) ? null : Pronouns
                );

                StatusMessage = success ? "Profile updated successfully!" : "Failed to update profile.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task SaveStatusAsync()
        {
            if (CurrentUser == null) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            try
            {
                var statusUpdated = UserStatusService.UpdateStatus(CurrentUser.UserId, SelectedStatus);
                var customStatusUpdated = UserStatusService.SetCustomStatus(
                    CurrentUser.UserId,
                    string.IsNullOrWhiteSpace(CustomStatusText) ? null : CustomStatusText,
                    string.IsNullOrWhiteSpace(CustomStatusEmoji) ? null : CustomStatusEmoji
                );

                StatusMessage = (statusUpdated && customStatusUpdated) 
                    ? "Status updated successfully!" 
                    : "Failed to update status.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ClearCustomStatusAsync()
        {
            if (CurrentUser == null) return;

            CustomStatusText = string.Empty;
            CustomStatusEmoji = string.Empty;

            try
            {
                UserStatusService.ClearCustomStatus(CurrentUser.UserId);
                StatusMessage = "Custom status cleared.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private async Task AddConnectionAsync()
        {
            if (CurrentUser == null) return;
            if (string.IsNullOrWhiteSpace(NewConnectionService) || string.IsNullOrWhiteSpace(NewConnectionUsername))
            {
                StatusMessage = "Please select a service and enter your username.";
                return;
            }

            IsProcessing = true;
            StatusMessage = string.Empty;

            try
            {
                var connection = ProfileConnectionService.AddConnection(
                    CurrentUser.UserId,
                    NewConnectionService,
                    NewConnectionUsername,
                    string.IsNullOrWhiteSpace(NewConnectionUrl) ? null : NewConnectionUrl
                );

                if (connection != null)
                {
                    Connections.Add(connection);
                    NewConnectionService = string.Empty;
                    NewConnectionUsername = string.Empty;
                    NewConnectionUrl = string.Empty;
                    StatusMessage = "Connection added successfully!";
                }
                else
                {
                    StatusMessage = "Failed to add connection. It may already exist.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task RemoveConnectionAsync(ProfileConnection conn)
        {
            if (conn == null) return;

            try
            {
                var success = ProfileConnectionService.RemoveConnection(conn.ConnectionId);
                if (success)
                {
                    Connections.Remove(conn);
                    StatusMessage = "Connection removed.";
                }
                else
                {
                    StatusMessage = "Failed to remove connection.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private async Task ToggleConnectionVisibilityAsync(ProfileConnection conn)
        {
            if (conn == null) return;

            try
            {
                var newVisibility = !conn.IsVisible;
                var success = ProfileConnectionService.SetConnectionVisibility(conn.ConnectionId, newVisibility);
                if (success)
                {
                    conn.IsVisible = newVisibility;
                    LoadConnections();
                    StatusMessage = newVisibility ? "Connection is now visible." : "Connection is now hidden.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private async Task SaveActiveStatusAsync()
        {
            if (CurrentUser == null) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            try
            {
                var success = UserStatusService.SetShowOnlineStatus(CurrentUser.UserId, ShowOnlineStatus);
                StatusMessage = success ? "Active status settings saved!" : "Failed to save active status settings.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
