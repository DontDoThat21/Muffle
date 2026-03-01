using Muffle.ViewModels;

namespace Muffle.Views;

public partial class GiftSubscriptionView : ContentView
{
    private readonly GiftSubscriptionViewModel _viewModel;

    public GiftSubscriptionView(GiftSubscriptionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void SendTabButton_OnClicked(object sender, EventArgs e)
        => _viewModel.SelectedTabIndex = 0;

    private void InboxTabButton_OnClicked(object sender, EventArgs e)
        => _viewModel.SelectedTabIndex = 1;

    private void SentTabButton_OnClicked(object sender, EventArgs e)
        => _viewModel.SelectedTabIndex = 2;

    private void SelectPremiumTier_OnClicked(object sender, EventArgs e)
        => _viewModel.SelectedTierIndex = 0;

    private void SelectPremiumPlusTier_OnClicked(object sender, EventArgs e)
        => _viewModel.SelectedTierIndex = 1;
}
