using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Manages the current logged-in user session
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
