using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.Services;

namespace Muffle.ViewModels
{
    /// <summary>
    /// ViewModel for managing multiple accounts and switching between them
    /// </summary>
    public class AccountSwitcherViewModel : BindableObject
    {
        private ObservableCollection<StoredAccount> _accounts = new();
        private bool _isLoading = false;

        public ObservableCollection<StoredAccount> Accounts
        {
            get => _accounts;
            set
            {
                _accounts = value;
                OnPropertyChanged();
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

        public bool HasAccounts => Accounts.Count > 0;

        public ICommand SwitchAccountCommand { get; }
        public ICommand RemoveAccountCommand { get; }
        public ICommand AddAccountCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler<User>? AccountSwitched;
        public event EventHandler? AddAccountRequested;

        public AccountSwitcherViewModel()
        {
            SwitchAccountCommand = new Command<StoredAccount>(async (account) => await SwitchAccountAsync(account));
            RemoveAccountCommand = new Command<StoredAccount>(async (account) => await RemoveAccountAsync(account));
            AddAccountCommand = new Command(() => AddAccountRequested?.Invoke(this, EventArgs.Empty));
            RefreshCommand = new Command(async () => await LoadAccountsAsync());

            // Load accounts on initialization
            Task.Run(async () => await LoadAccountsAsync());
        }

        /// <summary>
        /// Loads all stored accounts from secure storage
        /// </summary>
        public async Task LoadAccountsAsync()
        {
            IsLoading = true;

            try
            {
                var storedAccounts = await TokenStorageService.GetAllStoredAccountsAsync();
                
                // Validate accounts and filter out invalid tokens
                var validAccounts = AuthenticationService.ValidateStoredAccounts(storedAccounts);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Accounts.Clear();
                    foreach (var account in validAccounts.OrderByDescending(a => a.LastUsed))
                    {
                        Accounts.Add(account);
                    }
                    OnPropertyChanged(nameof(HasAccounts));
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading accounts: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Switches to the selected account
        /// </summary>
        private async Task SwitchAccountAsync(StoredAccount? account)
        {
            if (account == null)
            {
                return;
            }

            try
            {
                // Switch the current user context
                var success = CurrentUserService.SwitchAccount(account.Token);

                if (success && CurrentUserService.CurrentUser != null)
                {
                    // Update last used timestamp
                    await TokenStorageService.SetLastUsedTokenAsync(account.Token);
                    
                    // Reload accounts to update the UI with new timestamps
                    await LoadAccountsAsync();
                    
                    // Notify listeners that account was switched
                    AccountSwitched?.Invoke(this, CurrentUserService.CurrentUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error switching account: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes an account from secure storage
        /// </summary>
        private async Task RemoveAccountAsync(StoredAccount? account)
        {
            if (account == null)
            {
                return;
            }

            try
            {
                // Remove from secure storage
                await TokenStorageService.RemoveAccountAsync(account.UserId);
                
                // Revoke the token from the database
                AuthenticationService.RevokeAuthToken(account.Token);
                
                // Reload accounts
                await LoadAccountsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing account: {ex.Message}");
            }
        }
    }
}
