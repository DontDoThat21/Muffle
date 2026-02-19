using Muffle.Data.Services;
using Muffle.Views;
using Muffle.Services;

namespace Muffle
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Try to restore session from stored token
            MainPage = new ContentPage(); // Temporary placeholder
            Task.Run(async () => await InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Check if we have a stored authentication token
                var storedToken = await TokenStorageService.GetTokenAsync();

                if (!string.IsNullOrEmpty(storedToken))
                {
                    // Validate the token and get the user
                    var user = AuthenticationService.GetUserByToken(storedToken);

                    if (user != null)
                    {
                        // Token is valid - restore session
                        CurrentUserService.CurrentUser = user;
                        CurrentUserService.CurrentAuthToken = storedToken;

                        // Navigate to main app
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            MainPage = new AppShell();
                        });
                        return;
                    }
                    else
                    {
                        // Token is invalid or expired - remove it
                        TokenStorageService.RemoveToken();
                    }
                }

                // No valid token found - show authentication page
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var authPage = new AuthenticationPage();
                    authPage.AuthenticationSucceeded += OnAuthenticationSucceeded;
                    MainPage = authPage;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing app: {ex.Message}");
                
                // Fall back to authentication page
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var authPage = new AuthenticationPage();
                    authPage.AuthenticationSucceeded += OnAuthenticationSucceeded;
                    MainPage = authPage;
                });
            }
        }

        private void OnAuthenticationSucceeded(object? sender, Data.Models.User user)
        {
            // Store the current user (token already saved by LoginViewModel)
            CurrentUserService.CurrentUser = user;

            // Navigate to main app
            MainPage = new AppShell();
        }
    }
}
