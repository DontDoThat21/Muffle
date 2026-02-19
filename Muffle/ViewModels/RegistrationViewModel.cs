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

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public event EventHandler? RegistrationSucceeded;
        public event EventHandler? NavigateToLogin;

        public RegistrationViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            NavigateToLoginCommand = new Command(() => NavigateToLogin?.Invoke(this, EventArgs.Empty));
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

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        RegistrationSucceeded?.Invoke(this, EventArgs.Empty);
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
