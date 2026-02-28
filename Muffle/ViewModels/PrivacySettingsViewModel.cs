using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for privacy and safety settings — DM privacy, friend request filtering, and content filtering.
    /// </summary>
    public class PrivacySettingsViewModel : BindableObject
    {
        private string _selectedDmPrivacy = "Friends Only";
        private string _selectedFriendRequestFilter = "Everyone";
        private string _selectedContentFilter = "Medium";
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public string SelectedDmPrivacy
        {
            get => _selectedDmPrivacy;
            set { _selectedDmPrivacy = value; OnPropertyChanged(); }
        }

        public string SelectedFriendRequestFilter
        {
            get => _selectedFriendRequestFilter;
            set { _selectedFriendRequestFilter = value; OnPropertyChanged(); }
        }

        public string SelectedContentFilter
        {
            get => _selectedContentFilter;
            set { _selectedContentFilter = value; OnPropertyChanged(); }
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

        public ObservableCollection<string> DmPrivacyOptions { get; } = new();
        public ObservableCollection<string> FriendRequestFilterOptions { get; } = new();
        public ObservableCollection<string> ContentFilterOptions { get; } = new();

        public ICommand SavePrivacySettingsCommand { get; }

        public PrivacySettingsViewModel()
        {
            SavePrivacySettingsCommand = new Command(async () => await SavePrivacySettingsAsync());

            PopulateOptions();
            LoadCurrentSettings();
        }

        private void PopulateOptions()
        {
            DmPrivacyOptions.Clear();
            foreach (var option in PrivacySettingsService.DmPrivacyOptions)
                DmPrivacyOptions.Add(option);

            FriendRequestFilterOptions.Clear();
            foreach (var option in PrivacySettingsService.FriendRequestFilterOptions)
                FriendRequestFilterOptions.Add(option);

            ContentFilterOptions.Clear();
            foreach (var option in PrivacySettingsService.ContentFilterOptions)
                ContentFilterOptions.Add(option);
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                if (user == null) return;

                var settings = PrivacySettingsService.GetPrivacySettings(user.UserId);

                SelectedDmPrivacy = DmPrivacyOptions.ElementAtOrDefault(settings.DmPrivacy)
                    ?? DmPrivacyOptions[1];

                SelectedFriendRequestFilter = FriendRequestFilterOptions.ElementAtOrDefault(settings.FriendRequestFilter)
                    ?? FriendRequestFilterOptions[0];

                SelectedContentFilter = ContentFilterOptions.ElementAtOrDefault(settings.ContentFilter)
                    ?? ContentFilterOptions[1];
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading privacy settings: {ex.Message}";
            }
        }

        private async Task SavePrivacySettingsAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = string.Empty;

                var user = CurrentUserService.CurrentUser;
                if (user == null)
                {
                    StatusMessage = "No user is logged in.";
                    return;
                }

                var settings = new PrivacySettings
                {
                    UserId = user.UserId,
                    DmPrivacy = DmPrivacyOptions.IndexOf(SelectedDmPrivacy),
                    FriendRequestFilter = FriendRequestFilterOptions.IndexOf(SelectedFriendRequestFilter),
                    ContentFilter = ContentFilterOptions.IndexOf(SelectedContentFilter)
                };

                var success = PrivacySettingsService.SavePrivacySettings(settings);
                StatusMessage = success ? "Privacy settings saved." : "Failed to save privacy settings.";

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving privacy settings: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
