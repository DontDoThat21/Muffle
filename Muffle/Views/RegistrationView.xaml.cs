using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class RegistrationView : ContentView
    {
        private readonly RegistrationViewModel _viewModel;

        public event EventHandler? RegistrationSucceeded;
        public event EventHandler? NavigateToLogin;

        public RegistrationView()
        {
            InitializeComponent();

            _viewModel = new RegistrationViewModel();
            BindingContext = _viewModel;

            _viewModel.RegistrationSucceeded += OnRegistrationSucceeded;
            _viewModel.NavigateToLogin += OnNavigateToLogin;
        }

        private void OnRegistrationSucceeded(object? sender, EventArgs e)
        {
            RegistrationSucceeded?.Invoke(this, EventArgs.Empty);
        }

        private void OnNavigateToLogin(object? sender, EventArgs e)
        {
            NavigateToLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}
