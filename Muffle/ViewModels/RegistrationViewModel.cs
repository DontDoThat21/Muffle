using System.Windows.Input;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class RegistrationViewModel : BindableObject
    {
        private string _email = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _isRegistrationStep = true;
        private bool _isVerificationStep = false;
        private string _verificationCode = string.Empty;
        private string _simulatedCode = string.Empty;
        private int _pendingUserId = 0;
        private string _pendingUserEmail = string.Empty;

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

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
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

        public bool IsRegistrationStep
        {
            get => _isRegistrationStep;
            set { _isRegistrationStep = value; OnPropertyChanged(); }
        }

        public bool IsVerificationStep
        {
            get => _isVerificationStep;
            set { _isVerificationStep = value; OnPropertyChanged(); }
        }

        public string VerificationCode
        {
            get => _verificationCode;
            set { _verificationCode = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }

        /// <summary>
        /// Simulated email code shown to the user. In production this would be
        /// sent to the account's registered email address.
        /// </summary>
        public string SimulatedCode
        {
            get => _simulatedCode;
            set { _simulatedCode = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSimulatedCode)); }
        }

        public bool HasSimulatedCode => !string.IsNullOrEmpty(SimulatedCode);

        public string PendingUserEmail => _pendingUserEmail;

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand VerifyEmailCommand { get; }
        public ICommand ResendCodeCommand { get; }

        public event EventHandler? RegistrationSucceeded;
        public event EventHandler? NavigateToLogin;

        public RegistrationViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            NavigateToLoginCommand = new Command(() => NavigateToLogin?.Invoke(this, EventArgs.Empty));
            VerifyEmailCommand = new Command(async () => await OnVerifyEmailAsync());
            ResendCodeCommand = new Command(async () => await OnResendCodeAsync());
        }

        private async Task OnRegisterAsync()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            // Validate email format
            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Invalid email format";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var user = AuthenticationService.RegisterUser(Email, Username, Password);

                    if (user == null)
                    {
                        ErrorMessage = "Email already registered or registration failed";
                        return;
                    }

                    // Generate email verification code (simulated – in production this would be emailed)
                    var code = EmailVerificationService.CreateVerificationToken(user.UserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _pendingUserId = user.UserId;
                        _pendingUserEmail = user.Email;
                        SimulatedCode = code;
                        IsRegistrationStep = false;
                        IsVerificationStep = true;
                    });
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Registration failed: {ex.Message}";
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        private async Task OnVerifyEmailAsync()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode))
            {
                ErrorMessage = "Please enter the verification code.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var success = EmailVerificationService.VerifyEmail(_pendingUserId, VerificationCode);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsLoading = false;

                        if (!success)
                        {
                            ErrorMessage = "Invalid or expired code. Please try again.";
                            return;
                        }

                        RegistrationSucceeded?.Invoke(this, EventArgs.Empty);
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsLoading = false;
                        ErrorMessage = $"Verification failed: {ex.Message}";
                    });
                }
            });
        }

        private async Task OnResendCodeAsync()
        {
            if (_pendingUserId == 0) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var code = EmailVerificationService.ResendVerificationCode(_pendingUserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsLoading = false;

                        if (code == null)
                        {
                            ErrorMessage = "Could not resend code. Email may already be verified.";
                            return;
                        }

                        SimulatedCode = code;
                        VerificationCode = string.Empty;
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsLoading = false;
                        ErrorMessage = $"Failed to resend code: {ex.Message}";
                    });
                }
            });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
