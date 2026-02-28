using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for viewing and managing active login sessions across devices.
    /// </summary>
    public class ConnectedDevicesViewModel : BindableObject
    {
        private ObservableCollection<UserSession> _sessions = new();
        private bool _isLoading = false;
        private bool _hasOtherSessions = false;
        private string _statusMessage = string.Empty;

        public ObservableCollection<UserSession> Sessions
        {
            get => _sessions;
            set
            {
                _sessions = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSessions));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool HasOtherSessions
        {
            get => _hasOtherSessions;
            set { _hasOtherSessions = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool HasSessions => Sessions.Count > 0;
        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand RevokeSessionCommand { get; }
        public ICommand RevokeAllOtherSessionsCommand { get; }
        public ICommand RefreshCommand { get; }

        public ConnectedDevicesViewModel()
        {
            RevokeSessionCommand = new Command<UserSession>(async (session) => await RevokeSessionAsync(session));
            RevokeAllOtherSessionsCommand = new Command(async () => await RevokeAllOtherSessionsAsync());
            RefreshCommand = new Command(async () => await LoadSessionsAsync());

            Task.Run(async () => await LoadSessionsAsync());
        }

        public async Task LoadSessionsAsync()
        {
            IsLoading = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var user = CurrentUserService.CurrentUser;
                    if (user == null) return;

                    var currentToken = CurrentUserService.CurrentAuthToken;
                    var sessions = UserSessionService.GetActiveSessions(user.UserId);

                    foreach (var s in sessions)
                        s.IsCurrentSession = s.Token == currentToken;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Sessions.Clear();
                        foreach (var s in sessions)
                            Sessions.Add(s);

                        HasOtherSessions = sessions.Any(s => !s.IsCurrentSession);

                        if (sessions.Count == 0)
                            StatusMessage = "No active sessions found.";
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                        StatusMessage = $"Error loading sessions: {ex.Message}");
                }
            });

            IsLoading = false;
        }

        private async Task RevokeSessionAsync(UserSession session)
        {
            if (session == null || session.IsCurrentSession) return;

            try
            {
                var success = UserSessionService.RevokeSession(session.TokenId);
                if (success)
                {
                    Sessions.Remove(session);
                    HasOtherSessions = Sessions.Any(s => !s.IsCurrentSession);
                    OnPropertyChanged(nameof(HasSessions));
                    StatusMessage = "Session revoked.";
                    await Task.Delay(2000);
                    StatusMessage = string.Empty;
                }
                else
                {
                    StatusMessage = "Failed to revoke session.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error revoking session: {ex.Message}";
            }
        }

        private async Task RevokeAllOtherSessionsAsync()
        {
            try
            {
                var user = CurrentUserService.CurrentUser;
                var currentToken = CurrentUserService.CurrentAuthToken;
                if (user == null || currentToken == null) return;

                var count = UserSessionService.RevokeAllOtherSessions(user.UserId, currentToken);
                await LoadSessionsAsync();

                StatusMessage = count > 0
                    ? $"Logged out {count} other session{(count == 1 ? "" : "s")}."
                    : "No other sessions to log out.";

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error logging out other sessions: {ex.Message}";
            }
        }
    }
}
