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
                // Check if we have stored accounts
                var storedAccounts = await TokenStorageService.GetAllStoredAccountsAsync();
                
                // Clean up and validate stored accounts
                if (storedAccounts.Count > 0)
                {
                    var validAccounts = AuthenticationService.ValidateStoredAccounts(storedAccounts);
                    
                    // If we have valid accounts, try to restore the last used one
                    if (validAccounts.Count > 0)
                    {
                        var lastUsedToken = await TokenStorageService.GetLastUsedTokenAsync();
                        
                        // Try to use the last used token first
                        if (!string.IsNullOrEmpty(lastUsedToken))
                        {
                            var user = AuthenticationService.GetUserByToken(lastUsedToken);
                            
                            if (user != null)
                            {
                                // Last used token is valid - restore session
                                CurrentUserService.CurrentUser = user;
                                CurrentUserService.CurrentAuthToken = lastUsedToken;

                                // Navigate to main app
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    MainPage = new AppShell();
                                });
                                return;
                            }
                        }
                        
                        // If no last used token or it's invalid, show account selector
                        // For now, just use the first valid account
                        var firstAccount = validAccounts.First();
                        var firstUser = AuthenticationService.GetUserByToken(firstAccount.Token);
                        
                        if (firstUser != null)
                        {
                            CurrentUserService.CurrentUser = firstUser;
                            CurrentUserService.CurrentAuthToken = firstAccount.Token;
                            await TokenStorageService.SetLastUsedTokenAsync(firstAccount.Token);

                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                MainPage = new AppShell();
                            });
                            return;
                        }
                    }
                }

                // No valid accounts found - show authentication page
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
