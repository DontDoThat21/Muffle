using Muffle.Data.Services;
using Muffle.Views;

namespace Muffle
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Show authentication page first
            var authPage = new AuthenticationPage();
            authPage.AuthenticationSucceeded += OnAuthenticationSucceeded;
            MainPage = authPage;
        }

        private void OnAuthenticationSucceeded(object? sender, Data.Models.User user)
        {
            // Store the current user
            CurrentUserService.CurrentUser = user;

            // Navigate to main app
            MainPage = new AppShell();
        }
    }
}
