using System.Windows.Input;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class PasswordChangeViewModel : BindableObject
    {
        private bool _isStepOne = true;
        private bool _isStepTwo;
        private bool _isComplete;
        private bool _isProcessing;
        private string _currentPassword = string.Empty;
        private string _verificationCode = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private string _simulatedCode = string.Empty;

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

        public string CurrentPassword
        {
            get => _currentPassword;
            set { _currentPassword = value; OnPropertyChanged(); StatusMessage = string.Empty; }
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

        public string UserEmail => CurrentUserService.CurrentUser?.Email ?? string.Empty;

        // ── Commands ───────────────────────────────────────────────────────

        public ICommand RequestCodeCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand StartOverCommand { get; }

        public PasswordChangeViewModel()
        {
            RequestCodeCommand = new Command(async () => await OnRequestCodeAsync());
            ChangePasswordCommand = new Command(async () => await OnChangePasswordAsync());
            CancelCommand = new Command(OnCancel);
            StartOverCommand = new Command(OnStartOver);
        }

        private async Task OnRequestCodeAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                StatusMessage = "Please enter your current password.";
                return;
            }

            var userId = CurrentUserService.CurrentUser?.UserId ?? 0;
            if (userId == 0) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var code = PasswordChangeService.InitiatePasswordChange(userId, CurrentPassword);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsProcessing = false;

                        if (code == null)
                        {
                            StatusMessage = "Current password is incorrect. Please try again.";
                            return;
                        }

                        // In production, mail this code to UserEmail.
                        // For now, display it so the flow can be tested.
                        SimulatedCode = code;
                        CurrentPassword = string.Empty;
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

        private async Task OnChangePasswordAsync()
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

            var userId = CurrentUserService.CurrentUser?.UserId ?? 0;
            if (userId == 0) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var success = PasswordChangeService.VerifyAndChangePassword(userId, VerificationCode, NewPassword);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsProcessing = false;

                        if (!success)
                        {
                            StatusMessage = "Invalid or expired verification code. Please try again.";
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

        private void OnCancel()
        {
            ResetState();
        }

        private void OnStartOver()
        {
            ResetState();
        }

        private void ResetState()
        {
            CurrentPassword = string.Empty;
            VerificationCode = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            StatusMessage = string.Empty;
            SimulatedCode = string.Empty;
            IsStepTwo = false;
            IsComplete = false;
            IsStepOne = true;
        }
    }
}
