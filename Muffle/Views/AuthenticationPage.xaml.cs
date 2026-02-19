using Muffle.Data.Models;

namespace Muffle.Views
{
    public partial class AuthenticationPage : ContentPage
    {
        private LoginView? _loginView;
        private RegistrationView? _registrationView;

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

            AuthContentFrame.Content = _loginView;
        }

        private void ShowRegistration()
        {
            _registrationView = new RegistrationView();
            _registrationView.RegistrationSucceeded += OnRegistrationSucceeded;
            _registrationView.NavigateToLogin += OnNavigateToLogin;

            AuthContentFrame.Content = _registrationView;
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
    }
}
