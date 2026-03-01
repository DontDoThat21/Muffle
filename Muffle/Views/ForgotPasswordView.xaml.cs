using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class ForgotPasswordView : ContentView
    {
        public event EventHandler? NavigateToLogin;

        public ForgotPasswordView()
        {
            InitializeComponent();

            var vm = new ForgotPasswordViewModel();
            vm.NavigateToLogin += (s, e) => NavigateToLogin?.Invoke(this, EventArgs.Empty);
            BindingContext = vm;
        }
    }
}
