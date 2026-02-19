using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Manages the current logged-in user session
    /// </summary>
    public static class CurrentUserService
    {
        private static User? _currentUser;

        public static User? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public static bool IsLoggedIn => _currentUser != null;

        public static void Logout()
        {
            _currentUser = null;
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
