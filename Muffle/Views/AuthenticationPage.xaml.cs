using Muffle.Data.Models;

namespace Muffle.Views
{
    public partial class AuthenticationPage : ContentPage
    {
        private LoginView? _loginView;
        private RegistrationView? _registrationView;
        private ForgotPasswordView? _forgotPasswordView;

        public event EventHandler<User>? AuthenticationSucceeded;

        public AuthenticationPage()
        {
            InitializeComponent();
            ShowLogin();
        }

        private void ShowLogin()
        {
            _loginView = new LoginView();
            _loginView.LoginSucceeded += OnLoginSucceeded;
            _loginView.NavigateToRegister += OnNavigateToRegister;
            _loginView.NavigateToForgotPassword += OnNavigateToForgotPassword;

            AuthContentFrame.Content = _loginView;
        }

        private void ShowRegistration()
        {
            _registrationView = new RegistrationView();
            _registrationView.RegistrationSucceeded += OnRegistrationSucceeded;
            _registrationView.NavigateToLogin += OnNavigateToLogin;

            AuthContentFrame.Content = _registrationView;
        }

        private void ShowForgotPassword()
        {
            _forgotPasswordView = new ForgotPasswordView();
            _forgotPasswordView.NavigateToLogin += OnNavigateToLogin;

            AuthContentFrame.Content = _forgotPasswordView;
        }

        private void OnLoginSucceeded(object? sender, User user)
        {
            AuthenticationSucceeded?.Invoke(this, user);
        }

        private void OnRegistrationSucceeded(object? sender, EventArgs e)
        {
            // After successful registration, show login page
            ShowLogin();
            Application.Current?.MainPage?.DisplayAlert("Success", "Registration successful! Please log in.", "OK");
        }

        private void OnNavigateToRegister(object? sender, EventArgs e)
        {
            ShowRegistration();
        }

        private void OnNavigateToLogin(object? sender, EventArgs e)
        {
            ShowLogin();
        }

        private void OnNavigateToForgotPassword(object? sender, EventArgs e)
        {
            ShowForgotPassword();
        }
    }
}
