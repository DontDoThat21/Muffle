using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.Services;
using Muffle.Views;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for account settings and management
    /// </summary>
    public class AccountSettingsViewModel : BindableObject
    {
        private User? _currentUser;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentUserFullUsername));
                OnPropertyChanged(nameof(CurrentUserEmail));
            }
        }

        public string CurrentUserFullUsername => CurrentUser?.FullUsername ?? string.Empty;
        public string CurrentUserEmail => CurrentUser?.Email ?? string.Empty;

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

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand DisableAccountCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        public event EventHandler? AccountDisabled;
        public event EventHandler? AccountDeleted;

        public AccountSettingsViewModel()
        {
            DisableAccountCommand = new Command(async () => await DisableAccountAsync());
            DeleteAccountCommand = new Command(async () => await DeleteAccountAsync());

            // Load current user
            LoadCurrentUser();
        }

        private void LoadCurrentUser()
        {
            try
            {
                CurrentUser = CurrentUserService.CurrentUser;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading user: {ex.Message}";
            }
        }

        /// <summary>
        /// Disable the current user's account
        /// </summary>
        private async Task DisableAccountAsync()
        {
            if (CurrentUser == null)
            {
                StatusMessage = "No user logged in";
                return;
            }

            // Confirm action
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Disable Account",
                "Are you sure you want to disable your account? You can re-enable it later by contacting support.",
                "Yes, Disable",
                "Cancel");

            if (!confirm)
            {
                return;
            }

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var success = AccountManagementService.DisableAccount(CurrentUser.UserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = "Account disabled successfully. You will be logged out.";
                            
                            // Logout the user
                            CurrentUserService.Logout();
                            
                            // Clear stored tokens
                            TokenStorageService.ClearAllAccounts();
                            
                            // Navigate to login
                            Task.Delay(2000).ContinueWith(_ =>
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    var authPage = new AuthenticationPage();
                                    authPage.AuthenticationSucceeded += (s, user) =>
                                    {
                                        CurrentUserService.CurrentUser = user;
                                        Application.Current!.MainPage = new AppShell();
                                    };
                                    Application.Current!.MainPage = authPage;
                                });
                            });
                            
                            // Notify listeners
                            AccountDisabled?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = "Failed to disable account";
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
                finally
                {
                    IsProcessing = false;
                }
            });
        }

        /// <summary>
        /// Permanently delete the current user's account
        /// </summary>
        private async Task DeleteAccountAsync()
        {
            if (CurrentUser == null)
            {
                StatusMessage = "No user logged in";
                return;
            }

            // First confirmation
            var confirm1 = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Account",
                "Are you absolutely sure you want to permanently delete your account? This action CANNOT be undone!",
                "I understand, continue",
                "Cancel");

            if (!confirm1)
            {
                return;
            }

            // Second confirmation
            var confirm2 = await Application.Current!.MainPage!.DisplayAlert(
                "Final Confirmation",
                "This will permanently delete your account, all your messages, and all your data. Are you absolutely certain?",
                "Yes, Delete Permanently",
                "Cancel");

            if (!confirm2)
            {
                return;
            }

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var success = AccountManagementService.DeleteAccount(CurrentUser.UserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (success)
                        {
                            StatusMessage = "Account deleted permanently. Goodbye.";
                            
                            // Logout the user
                            CurrentUserService.Logout();
                            
                            // Clear stored tokens
                            TokenStorageService.ClearAllAccounts();
                            
                            // Navigate to login
                            Task.Delay(2000).ContinueWith(_ =>
                            {
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    var authPage = new AuthenticationPage();
                                    authPage.AuthenticationSucceeded += (s, user) =>
                                    {
                                        CurrentUserService.CurrentUser = user;
                                        Application.Current!.MainPage = new AppShell();
                                    };
                                    Application.Current!.MainPage = authPage;
                                });
                            });
                            
                            // Notify listeners
                            AccountDeleted?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            StatusMessage = "Failed to delete account";
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
                finally
                {
                    IsProcessing = false;
                }
            });
        }
    }
}
