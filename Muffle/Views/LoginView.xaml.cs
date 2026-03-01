using Muffle.Data.Models;
using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class LoginView : ContentView
    {
        private readonly LoginViewModel _viewModel;

        public event EventHandler<User>? LoginSucceeded;
        public event EventHandler? NavigateToRegister;
        public event EventHandler? NavigateToForgotPassword;

        public LoginView()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel();
            BindingContext = _viewModel;

            _viewModel.LoginSucceeded += OnLoginSucceeded;
            _viewModel.NavigateToRegister += OnNavigateToRegister;
            _viewModel.NavigateToForgotPassword += OnNavigateToForgotPassword;
        }

        private void OnLoginSucceeded(object? sender, User user)
        {
            LoginSucceeded?.Invoke(this, user);
        }

        private void OnNavigateToRegister(object? sender, EventArgs e)
        {
            NavigateToRegister?.Invoke(this, EventArgs.Empty);
        }

        private void OnNavigateToForgotPassword(object? sender, EventArgs e)
        {
            NavigateToForgotPassword?.Invoke(this, EventArgs.Empty);
        }
    }
}
