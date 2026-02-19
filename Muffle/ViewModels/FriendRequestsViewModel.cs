using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for viewing and managing friend requests
    /// </summary>
    public class FriendRequestsViewModel : BindableObject
    {
        private ObservableCollection<FriendRequest> _incomingRequests = new();
        private ObservableCollection<FriendRequest> _outgoingRequests = new();
        private bool _isLoading = false;
        private string _statusMessage = string.Empty;
        private int _selectedTabIndex = 0;

        public ObservableCollection<FriendRequest> IncomingRequests
        {
            get => _incomingRequests;
            set
            {
                _incomingRequests = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasIncomingRequests));
                OnPropertyChanged(nameof(IncomingRequestCount));
            }
        }

        public ObservableCollection<FriendRequest> OutgoingRequests
        {
            get => _outgoingRequests;
            set
            {
                _outgoingRequests = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasOutgoingRequests));
                OnPropertyChanged(nameof(OutgoingRequestCount));
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

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        public bool HasIncomingRequests => IncomingRequests.Count > 0;
        public bool HasOutgoingRequests => OutgoingRequests.Count > 0;
        public int IncomingRequestCount => IncomingRequests.Count;
        public int OutgoingRequestCount => OutgoingRequests.Count;
        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand CancelRequestCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler? RequestAccepted;
        public event EventHandler? RequestDeclined;

        public FriendRequestsViewModel()
        {
            AcceptRequestCommand = new Command<FriendRequest>(async (request) => await AcceptRequestAsync(request));
            DeclineRequestCommand = new Command<FriendRequest>(async (request) => await DeclineRequestAsync(request));
            CancelRequestCommand = new Command<FriendRequest>(async (request) => await CancelRequestAsync(request));
            RefreshCommand = new Command(async () => await LoadRequestsAsync());

            // Load requests on initialization
            Task.Run(async () => await LoadRequestsAsync());
        }

        /// <summary>
        /// Load all pending friend requests
        /// </summary>
        public async Task LoadRequestsAsync()
        {
            IsLoading = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var allRequests = FriendRequestService.GetPendingFriendRequests(currentUserId);

                    // Separate into incoming and outgoing
                    var incoming = allRequests.Where(r => r.ReceiverId == currentUserId).ToList();
                    var outgoing = allRequests.Where(r => r.SenderId == currentUserId).ToList();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IncomingRequests.Clear();
                        foreach (var request in incoming.OrderByDescending(r => r.CreatedAt))
                        {
                            IncomingRequests.Add(request);
                        }

                        OutgoingRequests.Clear();
                        foreach (var request in outgoing.OrderByDescending(r => r.CreatedAt))
                        {
                            OutgoingRequests.Add(request);
                        }

                        if (incoming.Count == 0 && outgoing.Count == 0)
                        {
                            StatusMessage = "No pending friend requests";
                        }
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusMessage = $"Error loading requests: {ex.Message}";
                    });
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        /// <summary>
        /// Accept an incoming friend request
        /// </summary>
        private async Task AcceptRequestAsync(FriendRequest? request)
        {
            if (request == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var success = FriendRequestService.AcceptFriendRequest(request.RequestId, currentUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = $"Accepted friend request from {request.SenderName}!";
                            
                            // Remove from incoming requests
                            IncomingRequests.Remove(request);
                            
                            // Notify listeners
                            RequestAccepted?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = $"Failed to accept request from {request.SenderName}";
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
        /// Decline an incoming friend request
        /// </summary>
        private async Task DeclineRequestAsync(FriendRequest? request)
        {
            if (request == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var success = FriendRequestService.DeclineFriendRequest(request.RequestId, currentUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = $"Declined friend request from {request.SenderName}";
                            
                            // Remove from incoming requests
                            IncomingRequests.Remove(request);
                            
                            // Notify listeners
                            RequestDeclined?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = $"Failed to decline request from {request.SenderName}";
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
        /// Cancel an outgoing friend request
        /// </summary>
        private async Task CancelRequestAsync(FriendRequest? request)
        {
            if (request == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                try
                {
                    var currentUserId = CurrentUserService.GetCurrentUserId();
                    var success = FriendRequestService.CancelFriendRequest(request.RequestId, currentUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = $"Cancelled friend request to {request.ReceiverName}";
                            
                            // Remove from outgoing requests
                            OutgoingRequests.Remove(request);
                        }
                        else
                        {
                            StatusMessage = $"Failed to cancel request to {request.ReceiverName}";
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
