using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for adding friends by searching for users
    /// </summary>
    public class AddFriendViewModel : BindableObject
    {
        private string _searchText = string.Empty;
        private ObservableCollection<User> _searchResults = new();
        private bool _isSearching = false;
        private bool _hasSearched = false;
        private string _statusMessage = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                StatusMessage = string.Empty;
            }
        }

        public ObservableCollection<User> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasResults));
                OnPropertyChanged(nameof(ShowNoResults));
            }
        }

        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                _isSearching = value;
                OnPropertyChanged();
            }
        }

        public bool HasSearched
        {
            get => _hasSearched;
            set
            {
                _hasSearched = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowNoResults));
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

        public bool HasResults => SearchResults.Count > 0;
        public bool ShowNoResults => HasSearched && !HasResults && !IsSearching;
        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand SearchCommand { get; }
        public ICommand SendFriendRequestCommand { get; }
        public ICommand BlockUserCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public event EventHandler? FriendRequestSent;
        public event EventHandler? UserBlocked;

        public AddFriendViewModel()
        {
            SearchCommand = new Command(async () => await SearchUsersAsync());
            SendFriendRequestCommand = new Command<User>(async (user) => await SendFriendRequestAsync(user));
            BlockUserCommand = new Command<User>(async (user) => await BlockUserAsync(user));
            ClearSearchCommand = new Command(ClearSearch);
        }

        /// <summary>
        /// Search for users by email or username
        /// </summary>
        private async Task SearchUsersAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                StatusMessage = "Please enter an email or username to search";
                return;
            }

            if (SearchText.Length < 3)
            {
                StatusMessage = "Please enter at least 3 characters";
                return;
            }

            IsSearching = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var results = FriendRequestService.SearchUsers(SearchText, currentUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        SearchResults.Clear();
                        foreach (var user in results)
                        {
                            SearchResults.Add(user);
                        }

                        HasSearched = true;

                        if (results.Count == 0)
                        {
                            StatusMessage = "No users found";
                        }
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusMessage = $"Search failed: {ex.Message}";
                    });
                }
                finally
                {
                    IsSearching = false;
                }
            });
        }

        /// <summary>
        /// Send a friend request to a user
        /// </summary>
        private async Task SendFriendRequestAsync(User? user)
        {
            if (user == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var request = FriendRequestService.SendFriendRequest(currentUserId, user.UserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (request != null)
                        {
                            StatusMessage = $"Friend request sent to {user.Name}!";
                            
                            // Remove the user from search results
                            SearchResults.Remove(user);
                            
                            // Notify listeners
                            FriendRequestSent?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = $"Failed to send friend request to {user.Name}. A request may already exist.";
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

        /// <summary>
        /// Block a user
        /// </summary>
        private async Task BlockUserAsync(User? user)
        {
            if (user == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var success = BlockService.BlockUser(currentUserId, user.UserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = $"Blocked {user.FullUsername}";
                            
                            // Remove the user from search results
                            SearchResults.Remove(user);
                            
                            // Notify listeners
                            UserBlocked?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = $"Failed to block {user.FullUsername}. User may already be blocked.";
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

        /// <summary>
        /// Clear the search results and text
        /// </summary>
        private void ClearSearch()
        {
            SearchText = string.Empty;
            SearchResults.Clear();
            HasSearched = false;
            StatusMessage = string.Empty;
        }
    }
}
