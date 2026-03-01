using Muffle.ViewModels;

namespace Muffle.Views;

public partial class SubscriptionView : ContentView
{
    public SubscriptionView(SubscriptionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
