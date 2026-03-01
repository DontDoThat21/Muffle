using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class SubscriptionViewModel : BindableObject
    {
        private readonly int _currentUserId;

        private UserSubscription _currentSubscription;
        public UserSubscription CurrentSubscription
        {
            get => _currentSubscription;
            set
            {
                _currentSubscription = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOnFree));
                OnPropertyChanged(nameof(IsOnPremium));
                OnPropertyChanged(nameof(IsOnPremiumPlus));
                OnPropertyChanged(nameof(CurrentTierLabel));
                OnPropertyChanged(nameof(CurrentTierColor));
                OnPropertyChanged(nameof(ExpiryText));
                OnPropertyChanged(nameof(HasActiveSubscription));
            }
        }

        public ObservableCollection<SubscriptionFeature> Features { get; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public bool IsOnFree => CurrentSubscription == null || !CurrentSubscription.IsActive;
        public bool IsOnPremium => CurrentSubscription?.IsPremium == true && !CurrentSubscription.IsPremiumPlus;
        public bool IsOnPremiumPlus => CurrentSubscription?.IsPremiumPlus == true;
        public bool HasActiveSubscription => CurrentSubscription?.IsActive == true;

        public string CurrentTierLabel => CurrentSubscription?.IsActive == true
            ? CurrentSubscription.TierDisplayName
            : "Free";

        public string CurrentTierColor => CurrentSubscription?.IsActive == true
            ? (CurrentSubscription.IsPremiumPlus ? "#F39C12" : "#7289DA")
            : "#43B581";

        public string ExpiryText => CurrentSubscription?.IsActive == true
            ? $"Renews {CurrentSubscription.ExpiresAt:MMM d, yyyy}"
            : "No active subscription";

        public ICommand SubscribePremiumCommand { get; }
        public ICommand SubscribePremiumPlusCommand { get; }
        public ICommand CancelCommand { get; }

        public SubscriptionViewModel()
        {
            var user = CurrentUserService.CurrentUser;
            _currentUserId = user?.UserId ?? 0;

            SubscribePremiumCommand = new Command(async () => await SubscribeAsync(SubscriptionTier.Premium));
            SubscribePremiumPlusCommand = new Command(async () => await SubscribeAsync(SubscriptionTier.PremiumPlus));
            CancelCommand = new Command(async () => await CancelAsync());

            foreach (var f in SubscriptionService.GetFeatureComparison())
                Features.Add(f);

            LoadSubscription();
        }

        private void LoadSubscription()
        {
            try
            {
                CurrentSubscription = SubscriptionService.GetSubscription(_currentUserId);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load subscription: {ex.Message}";
            }
        }

        private async Task SubscribeAsync(SubscriptionTier tier)
        {
            var tierName = tier == SubscriptionTier.PremiumPlus ? "Muffle Premium+" : "Muffle Premium";
            var price = tier == SubscriptionTier.PremiumPlus ? "$9.99/mo" : "$4.99/mo";

            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                $"Subscribe to {tierName}",
                $"You are about to subscribe to {tierName} for {price}.\n\nThis is a simulated subscription — no real payment is processed.",
                "Subscribe", "Cancel");

            if (!confirmed) return;

            IsLoading = true;
            StatusMessage = null;
            try
            {
                await Task.Run(() => SubscriptionService.Subscribe(_currentUserId, tier));
                LoadSubscription();
                StatusMessage = $"Successfully subscribed to {tierName}!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Subscription failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CancelAsync()
        {
            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                "Cancel Subscription",
                $"Cancel your {CurrentTierLabel} subscription? You will keep access until {CurrentSubscription?.ExpiresAt:MMM d, yyyy}.",
                "Cancel Subscription", "Keep Subscription");

            if (!confirmed) return;

            IsLoading = true;
            StatusMessage = null;
            try
            {
                await Task.Run(() => SubscriptionService.CancelSubscription(_currentUserId));
                LoadSubscription();
                StatusMessage = "Subscription cancelled. Access remains until expiry.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Cancellation failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
