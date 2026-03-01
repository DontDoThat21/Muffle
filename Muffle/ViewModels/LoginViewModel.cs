using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;
using Muffle.Services;

namespace Muffle.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _isTwoFactorStep = false;
        private string _twoFactorCode = string.Empty;
        private User? _pendingUser;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
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

        /// <summary>
        /// True when the password step passed and we are waiting for the TOTP code.
        /// </summary>
        public bool IsTwoFactorStep
        {
            get => _isTwoFactorStep;
            set
            {
                _isTwoFactorStep = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCredentialStep));
            }
        }

        public bool IsCredentialStep => !IsTwoFactorStep;

        public string TwoFactorCode
        {
            get => _twoFactorCode;
            set
            {
                _twoFactorCode = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand VerifyTwoFactorCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public event EventHandler<User>? LoginSucceeded;
        public event EventHandler? NavigateToRegister;

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLoginAsync());
            VerifyTwoFactorCommand = new Command(async () => await OnVerifyTwoFactorAsync());
            NavigateToRegisterCommand = new Command(() => NavigateToRegister?.Invoke(this, EventArgs.Empty));
        }

        private async Task OnLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            await Task.Run(async () =>
            {
                try
                {
                    var user = AuthenticationService.LoginUser(Email, Password);

                    if (user == null)
                    {
                        ErrorMessage = AuthenticationService.IsEmailNotVerified(Email)
                            ? "Please verify your email address before logging in."
                            : "Invalid email or password";
                        return;
                    }

                    // Check if 2FA is required before completing login
                    if (user.IsTwoFactorEnabled)
                    {
                        _pendingUser = user;
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            TwoFactorCode = string.Empty;
                            IsTwoFactorStep = true;
                        });
                        return;
                    }

                    await CompleteLoginAsync(user);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Login failed: {ex.Message}";
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        private async Task OnVerifyTwoFactorAsync()
        {
            if (_pendingUser == null) return;

            if (string.IsNullOrWhiteSpace(TwoFactorCode))
            {
                ErrorMessage = "Enter your 6-digit authenticator code";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            await Task.Run(async () =>
            {
                try
                {
                    var twoFa = TwoFactorAuthService.GetTwoFactorAuth(_pendingUser.UserId);
                    bool valid = twoFa?.Secret != null
                        && TwoFactorAuthService.VerifyCode(twoFa.Secret, TwoFactorCode);

                    if (!valid)
                    {
                        // Try backup code
                        valid = TwoFactorAuthService.ValidateBackupCode(_pendingUser.UserId, TwoFactorCode);
                    }

                    if (!valid)
                    {
                        ErrorMessage = "Invalid code. Try again or use a backup code.";
                        return;
                    }

                    await CompleteLoginAsync(_pendingUser);
                    _pendingUser = null;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Verification failed: {ex.Message}";
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        private async Task CompleteLoginAsync(User user)
        {
            var deviceName = DeviceInfo.Current.Name;
            var platform = DeviceInfo.Current.Platform.ToString();
            var token = AuthenticationService.GenerateAuthToken(user.UserId,
                deviceName: deviceName, platform: platform);

            if (token != null)
            {
                await TokenStorageService.SaveAccountAsync(user.UserId, user.Name, user.Email, token);
                CurrentUserService.CurrentAuthToken = token;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoginSucceeded?.Invoke(this, user);
            });
        }
    }
}

