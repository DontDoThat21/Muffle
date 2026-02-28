using Muffle.ViewModels;

namespace Muffle.Views
{
    public partial class TwoFactorAuthView : ContentView
    {
        public TwoFactorAuthView()
        {
            InitializeComponent();
            BindingContext = new TwoFactorAuthViewModel();
        }

        public TwoFactorAuthView(TwoFactorAuthViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
