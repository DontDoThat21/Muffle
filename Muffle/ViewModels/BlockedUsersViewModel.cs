using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for managing blocked users
    /// </summary>
    public class BlockedUsersViewModel : BindableObject
    {
        private ObservableCollection<BlockedUser> _blockedUsers = new();
        private bool _isLoading = false;
        private string _statusMessage = string.Empty;

        public ObservableCollection<BlockedUser> BlockedUsers
        {
            get => _blockedUsers;
            set
            {
                _blockedUsers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasBlockedUsers));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }

        public bool HasBlockedUsers => BlockedUsers.Count > 0;
        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand UnblockUserCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler? UserUnblocked;

        public BlockedUsersViewModel()
        {
            UnblockUserCommand = new Command<BlockedUser>(async (user) => await UnblockUserAsync(user));
            RefreshCommand = new Command(async () => await LoadBlockedUsersAsync());

            // Load blocked users on initialization
            Task.Run(async () => await LoadBlockedUsersAsync());
        }

        /// <summary>
        /// Load all blocked users for the current user
        /// </summary>
        public async Task LoadBlockedUsersAsync()
        {
            IsLoading = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var blockedUsers = BlockService.GetBlockedUsers(currentUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        BlockedUsers.Clear();
                        foreach (var blockedUser in blockedUsers)
                        {
                            BlockedUsers.Add(blockedUser);
                        }

                        if (blockedUsers.Count == 0)
                        {
                            StatusMessage = "No blocked users";
                        }
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusMessage = $"Error loading blocked users: {ex.Message}";
                    });
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        /// <summary>
        /// Unblock a user
        /// </summary>
        private async Task UnblockUserAsync(BlockedUser? blockedUser)
        {
            if (blockedUser == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var success = BlockService.UnblockUser(currentUserId, blockedUser.BlockedUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = $"Unblocked {blockedUser.BlockedUserFullUsername}";
                            
                            // Remove from list
                            BlockedUsers.Remove(blockedUser);
                            
                            // Notify listeners
                            UserUnblocked?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = $"Failed to unblock {blockedUser.BlockedUserFullUsername}";
                        }
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusMessage = $"Error: {ex.Message}";
                    });
                }
            });
        }
    }
}
