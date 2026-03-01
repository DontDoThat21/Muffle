using System.Windows.Input;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class ForgotPasswordViewModel : BindableObject
    {
        private bool _isStepOne = true;
        private bool _isStepTwo;
        private bool _isComplete;
        private bool _isProcessing;
        private string _email = string.Empty;
        private string _verificationCode = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private string _simulatedCode = string.Empty;
        private int _pendingUserId;

        // ── Step visibility ────────────────────────────────────────────────

        public bool IsStepOne
        {
            get => _isStepOne;
            set { _isStepOne = value; OnPropertyChanged(); }
        }

        public bool IsStepTwo
        {
            get => _isStepTwo;
            set { _isStepTwo = value; OnPropertyChanged(); }
        }

        public bool IsComplete
        {
            get => _isComplete;
            set { _isComplete = value; OnPropertyChanged(); }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        // ── Input fields ───────────────────────────────────────────────────

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); StatusMessage = string.Empty; }
        }

        public string VerificationCode
        {
            get => _verificationCode;
            set { _verificationCode = value; OnPropertyChanged(); StatusMessage = string.Empty; }
        }

        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); StatusMessage = string.Empty; }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); StatusMessage = string.Empty; }
        }

        // ── Feedback ───────────────────────────────────────────────────────

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

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

        // ── Commands ───────────────────────────────────────────────────────

        public ICommand SendResetCodeCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand BackToLoginCommand { get; }
        public ICommand StartOverCommand { get; }

        public event EventHandler? NavigateToLogin;

        public ForgotPasswordViewModel()
        {
            SendResetCodeCommand = new Command(async () => await OnSendResetCodeAsync());
            ResetPasswordCommand = new Command(async () => await OnResetPasswordAsync());
            BackToLoginCommand = new Command(() => NavigateToLogin?.Invoke(this, EventArgs.Empty));
            StartOverCommand = new Command(OnStartOver);
        }

        private async Task OnSendResetCodeAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                StatusMessage = "Please enter your email address.";
                return;
            }

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(async () =>
            {
                try
                {
                    var result = PasswordChangeService.InitiateForgotPassword(Email.Trim());

                    if (result != null)
                    {
                        _pendingUserId = result.Value.UserId;

                        // Send the code via email
                        var sent = await EmailService.SendPasswordResetEmailAsync(Email.Trim(), result.Value.Code);

                        if (!sent)
                        {
                            // Fall back to showing the code in the UI if email fails
                            MainThread.BeginInvokeOnMainThread(() => SimulatedCode = result.Value.Code);
                        }
                    }

                    // Always advance to step 2 — don't reveal whether the email exists.
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsProcessing = false;
                        IsStepOne = false;
                        IsStepTwo = true;
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsProcessing = false;
                        StatusMessage = $"An error occurred: {ex.Message}";
                    });
                }
            });
        }

        private async Task OnResetPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode))
            {
                StatusMessage = "Please enter the verification code.";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                StatusMessage = "Please enter a new password.";
                return;
            }

            if (NewPassword.Length < 8)
            {
                StatusMessage = "New password must be at least 8 characters.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                StatusMessage = "Passwords do not match.";
                return;
            }

            if (_pendingUserId == 0)
            {
                StatusMessage = "Invalid or expired reset code. Please start over.";
                return;
            }

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var success = PasswordChangeService.VerifyAndChangePassword(_pendingUserId, VerificationCode, NewPassword);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsProcessing = false;

                        if (!success)
                        {
                            StatusMessage = "Invalid or expired code. Please try again.";
                            return;
                        }

                        SimulatedCode = string.Empty;
                        IsStepTwo = false;
                        IsComplete = true;
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsProcessing = false;
                        StatusMessage = $"An error occurred: {ex.Message}";
                    });
                }
            });
        }

        private void OnStartOver()
        {
            Email = string.Empty;
            VerificationCode = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            StatusMessage = string.Empty;
            SimulatedCode = string.Empty;
            _pendingUserId = 0;
            IsStepTwo = false;
            IsComplete = false;
            IsStepOne = true;
        }
    }
}
