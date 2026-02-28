using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class TwoFactorAuthViewModel : BindableObject
    {
        private bool _isEnabled;
        private bool _isSetupMode;
        private bool _isDisableMode;
        private bool _showBackupCodes;
        private bool _isProcessing;
        private string _statusMessage = string.Empty;
        private string _secret = string.Empty;
        private string _setupUri = string.Empty;
        private string _verificationCode = string.Empty;
        private ObservableCollection<string> _backupCodes = new();

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotEnabled)); }
        }

        public bool IsNotEnabled => !IsEnabled;

        public bool IsSetupMode
        {
            get => _isSetupMode;
            set { _isSetupMode = value; OnPropertyChanged(); }
        }

        public bool IsDisableMode
        {
            get => _isDisableMode;
            set { _isDisableMode = value; OnPropertyChanged(); }
        }

        public bool ShowBackupCodes
        {
            get => _showBackupCodes;
            set { _showBackupCodes = value; OnPropertyChanged(); }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        /// <summary>
        /// The Base32 secret shown to the user during setup.
        /// </summary>
        public string Secret
        {
            get => _secret;
            set { _secret = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// The otpauth:// URI for the authenticator app.
        /// </summary>
        public string SetupUri
        {
            get => _setupUri;
            set { _setupUri = value; OnPropertyChanged(); }
        }

        public string VerificationCode
        {
            get => _verificationCode;
            set { _verificationCode = value; OnPropertyChanged(); StatusMessage = string.Empty; }
        }

        public ObservableCollection<string> BackupCodes
        {
            get => _backupCodes;
            set { _backupCodes = value; OnPropertyChanged(); }
        }

        public ICommand BeginSetupCommand { get; }
        public ICommand VerifyAndEnableCommand { get; }
        public ICommand CancelSetupCommand { get; }
        public ICommand BeginDisableCommand { get; }
        public ICommand ConfirmDisableCommand { get; }
        public ICommand CancelDisableCommand { get; }
        public ICommand RegenerateBackupCodesCommand { get; }
        public ICommand DismissBackupCodesCommand { get; }

        public TwoFactorAuthViewModel()
        {
            BeginSetupCommand = new Command(OnBeginSetup);
            VerifyAndEnableCommand = new Command(async () => await OnVerifyAndEnableAsync());
            CancelSetupCommand = new Command(OnCancelSetup);
            BeginDisableCommand = new Command(OnBeginDisable);
            ConfirmDisableCommand = new Command(async () => await OnConfirmDisableAsync());
            CancelDisableCommand = new Command(OnCancelDisable);
            RegenerateBackupCodesCommand = new Command(OnRegenerateBackupCodes);
            DismissBackupCodesCommand = new Command(() => ShowBackupCodes = false);

            LoadState();
        }

        private void LoadState()
        {
            var userId = CurrentUserService.CurrentUser?.UserId ?? 0;
            if (userId == 0) return;

            IsEnabled = TwoFactorAuthService.IsTwoFactorEnabled(userId);
        }

        private void OnBeginSetup()
        {
            var user = CurrentUserService.CurrentUser;
            if (user == null) return;

            Secret = TwoFactorAuthService.GenerateSecret();
            SetupUri = TwoFactorAuthService.GenerateSetupUri(Secret, user.Email);
            VerificationCode = string.Empty;
            StatusMessage = string.Empty;
            IsSetupMode = true;
        }

        private async Task OnVerifyAndEnableAsync()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode))
            {
                StatusMessage = "Enter the 6-digit code from your authenticator app.";
                return;
            }

            var user = CurrentUserService.CurrentUser;
            if (user == null) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    if (!TwoFactorAuthService.VerifyCode(Secret, VerificationCode))
                    {
                        StatusMessage = "Code is incorrect. Make sure your device clock is accurate.";
                        return;
                    }

                    if (!TwoFactorAuthService.EnableTwoFactor(user.UserId, Secret))
                    {
                        StatusMessage = "Failed to enable 2FA. Please try again.";
                        return;
                    }

                    // Update the in-memory user so the rest of the session reflects the change
                    user.IsTwoFactorEnabled = true;

                    var codes = TwoFactorAuthService.GenerateBackupCodes(user.UserId);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        BackupCodes = new ObservableCollection<string>(codes);
                        IsEnabled = true;
                        IsSetupMode = false;
                        ShowBackupCodes = true;
                        StatusMessage = string.Empty;
                    });
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error: {ex.Message}";
                }
                finally
                {
                    IsProcessing = false;
                }
            });
        }

        private void OnCancelSetup()
        {
            IsSetupMode = false;
            Secret = string.Empty;
            SetupUri = string.Empty;
            VerificationCode = string.Empty;
            StatusMessage = string.Empty;
        }

        private void OnBeginDisable()
        {
            VerificationCode = string.Empty;
            StatusMessage = string.Empty;
            IsDisableMode = true;
        }

        private async Task OnConfirmDisableAsync()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode))
            {
                StatusMessage = "Enter your current authenticator code to confirm.";
                return;
            }

            var user = CurrentUserService.CurrentUser;
            if (user == null) return;

            IsProcessing = true;
            StatusMessage = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    var twoFa = TwoFactorAuthService.GetTwoFactorAuth(user.UserId);
                    bool valid = twoFa?.Secret != null
                        && TwoFactorAuthService.VerifyCode(twoFa.Secret, VerificationCode);

                    if (!valid)
                        valid = TwoFactorAuthService.ValidateBackupCode(user.UserId, VerificationCode);

                    if (!valid)
                    {
                        StatusMessage = "Code is incorrect. Two-factor authentication was NOT disabled.";
                        return;
                    }

                    if (!TwoFactorAuthService.DisableTwoFactor(user.UserId))
                    {
                        StatusMessage = "Failed to disable 2FA. Please try again.";
                        return;
                    }

                    user.IsTwoFactorEnabled = false;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsEnabled = false;
                        IsDisableMode = false;
                        VerificationCode = string.Empty;
                        BackupCodes.Clear();
                        StatusMessage = "Two-factor authentication has been disabled.";
                    });
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error: {ex.Message}";
                }
                finally
                {
                    IsProcessing = false;
                }
            });
        }

        private void OnCancelDisable()
        {
            IsDisableMode = false;
            VerificationCode = string.Empty;
            StatusMessage = string.Empty;
        }

        private void OnRegenerateBackupCodes()
        {
            var user = CurrentUserService.CurrentUser;
            if (user == null) return;

            var codes = TwoFactorAuthService.GenerateBackupCodes(user.UserId);
            BackupCodes = new ObservableCollection<string>(codes);
            ShowBackupCodes = true;
            StatusMessage = "New backup codes generated. Store them somewhere safe.";
        }
    }
}
