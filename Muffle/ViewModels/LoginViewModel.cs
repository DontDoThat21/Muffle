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

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public event EventHandler<User>? LoginSucceeded;
        public event EventHandler? NavigateToRegister;

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLoginAsync());
            NavigateToRegisterCommand = new Command(() => NavigateToRegister?.Invoke(this, EventArgs.Empty));
        }

        private async Task OnLoginAsync()
        {
            // Validate inputs
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
                        ErrorMessage = "Invalid email or password";
                        return;
                    }

                    // Generate authentication token
                    var token = AuthenticationService.GenerateAuthToken(user.UserId);

                    if (token != null)
                    {
                        // Save token to secure storage
                        await TokenStorageService.SaveTokenAsync(token);
                        
                        // Store token in current session
                        CurrentUserService.CurrentAuthToken = token;
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        LoginSucceeded?.Invoke(this, user);
                    });
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
    }
}
