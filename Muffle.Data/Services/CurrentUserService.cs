using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Manages the current logged-in user session
    /// Supports multiple account switching
    /// </summary>
    public static class CurrentUserService
    {
        private static User? _currentUser;
        private static string? _currentAuthToken;

        public static User? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public static string? CurrentAuthToken
        {
            get => _currentAuthToken;
            set => _currentAuthToken = value;
        }

        public static bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// Event raised when the current user changes (e.g., account switching)
        /// </summary>
        public static event EventHandler<User>? CurrentUserChanged;

        /// <summary>
        /// Switches to a different account using a stored authentication token
        /// </summary>
        /// <param name="token">The authentication token for the account to switch to</param>
        /// <returns>True if the switch was successful, false otherwise</returns>
        public static bool SwitchAccount(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            // Validate the token and get the user
            var user = AuthenticationService.GetUserByToken(token);

            if (user != null)
            {
                _currentUser = user;
                _currentAuthToken = token;
                
                // Raise event to notify UI of user change
                CurrentUserChanged?.Invoke(null, user);
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Logs out the current user and revokes their authentication token
        /// </summary>
        public static void Logout()
        {
            // Revoke the token from the database
            if (!string.IsNullOrEmpty(_currentAuthToken))
            {
                AuthenticationService.RevokeAuthToken(_currentAuthToken);
            }

            // Clear in-memory state
            _currentUser = null;
            _currentAuthToken = null;
            
            // Raise event to notify UI of logout
            CurrentUserChanged?.Invoke(null, null!);
        }

        /// <summary>
        /// Logs out the current user without revoking the token (keeps the account stored for quick switching)
        /// </summary>
        public static void LogoutWithoutRevoke()
        {
            // Clear in-memory state but keep token valid
            _currentUser = null;
            _currentAuthToken = null;
            
            // Raise event to notify UI of logout
            CurrentUserChanged?.Invoke(null, null!);
        }

        public static int GetCurrentUserId()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("No user is currently logged in");
            }

            return _currentUser.UserId;
        }
    }
}
