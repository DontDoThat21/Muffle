using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class PasswordChangeView : ContentView
    {
        public PasswordChangeView()
        {
            InitializeComponent();
            BindingContext = new PasswordChangeViewModel();
        }

        public PasswordChangeView(PasswordChangeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
