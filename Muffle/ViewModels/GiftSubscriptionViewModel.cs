using System.Collections.ObjectModel;
using System.Windows.Input;
using Muffle.Data.Models;
using Muffle.Data.Services;

namespace Muffle.ViewModels
{
    public class GiftSubscriptionViewModel : BindableObject
    {
        private readonly int _currentUserId;

        // ── Send tab ──────────────────────────────────────────────────────────
        public List<Friend> Friends { get; private set; } = new();

        private Friend _selectedFriend;
        public Friend SelectedFriend
        {
            get => _selectedFriend;
            set { _selectedFriend = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSend)); }
        }

        private int _selectedTierIndex = 0; // 0 = Premium, 1 = Premium+
        public int SelectedTierIndex
        {
            get => _selectedTierIndex;
            set { _selectedTierIndex = value; OnPropertyChanged(); OnPropertyChanged(nameof(SelectedTierLabel)); OnPropertyChanged(nameof(SelectedTierPrice)); }
        }

        public string SelectedTierLabel => SelectedTierIndex == 1 ? "Muffle Premium+" : "Muffle Premium";
        public string SelectedTierPrice => SelectedTierIndex == 1 ? "$9.99" : "$4.99";

        private string _giftMessage;
        public string GiftMessage
        {
            get => _giftMessage;
            set { _giftMessage = value; OnPropertyChanged(); }
        }

        public bool CanSend => SelectedFriend != null;

        // ── Inbox / Sent tabs ─────────────────────────────────────────────────
        public ObservableCollection<SubscriptionGift> ReceivedGifts { get; } = new();
        public ObservableCollection<SubscriptionGift> SentGifts { get; } = new();

        private int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { _selectedTabIndex = value; OnPropertyChanged(); }
        }

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
        public bool HasReceivedGifts => ReceivedGifts.Count > 0;
        public bool HasSentGifts => SentGifts.Count > 0;

        public int PendingInboxCount => ReceivedGifts.Count(g => g.IsPending);

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand SendGiftCommand { get; }
        public ICommand AcceptGiftCommand { get; }
        public ICommand DeclineGiftCommand { get; }
        public ICommand CancelSentGiftCommand { get; }
        public ICommand SelectFriendCommand { get; }
        public ICommand RefreshCommand { get; }

        public GiftSubscriptionViewModel()
        {
            var user = CurrentUserService.CurrentUser;
            _currentUserId = user?.UserId ?? 0;

            SendGiftCommand = new Command(async () => await SendGiftAsync(), () => CanSend);
            AcceptGiftCommand = new Command<SubscriptionGift>(async g => await AcceptGiftAsync(g));
            DeclineGiftCommand = new Command<SubscriptionGift>(async g => await DeclineGiftAsync(g));
            CancelSentGiftCommand = new Command<SubscriptionGift>(async g => await CancelSentGiftAsync(g));
            SelectFriendCommand = new Command<Friend>(f => SelectedFriend = f);
            RefreshCommand = new Command(async () => await LoadGiftsAsync());

            Friends = UsersService.GetUsersFriends() ?? new List<Friend>();
            _ = LoadGiftsAsync();
        }

        private async Task LoadGiftsAsync()
        {
            IsLoading = true;
            StatusMessage = null;
            try
            {
                var received = await Task.Run(() => SubscriptionGiftService.GetReceivedGifts(_currentUserId));
                var sent = await Task.Run(() => SubscriptionGiftService.GetSentGifts(_currentUserId));

                ReceivedGifts.Clear();
                foreach (var g in received) ReceivedGifts.Add(g);

                SentGifts.Clear();
                foreach (var g in sent) SentGifts.Add(g);

                OnPropertyChanged(nameof(HasReceivedGifts));
                OnPropertyChanged(nameof(HasSentGifts));
                OnPropertyChanged(nameof(PendingInboxCount));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load gifts: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SendGiftAsync()
        {
            if (SelectedFriend == null) return;

            var tier = SelectedTierIndex == 1 ? SubscriptionTier.PremiumPlus : SubscriptionTier.Premium;
            var msg = GiftMessage?.Trim() ?? string.Empty;

            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                "Send Gift",
                $"Send {SelectedTierLabel} ({SelectedTierPrice}/mo) to {SelectedFriend.Name}?\n\nThis is a simulated gift — no real payment is processed.",
                "Send Gift", "Cancel");

            if (!confirmed) return;

            IsLoading = true;
            StatusMessage = null;
            try
            {
                await Task.Run(() => SubscriptionGiftService.SendGift(_currentUserId, SelectedFriend.Id, tier, msg));
                GiftMessage = string.Empty;
                SelectedFriend = null;
                SelectedTierIndex = 0;
                await LoadGiftsAsync();
                StatusMessage = $"Gift sent successfully!";
                SelectedTabIndex = 2; // Switch to Sent tab
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to send gift: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AcceptGiftAsync(SubscriptionGift gift)
        {
            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                "Accept Gift",
                $"Accept {gift.TierDisplayName} from {gift.SenderName}? This will activate a 1-month subscription.",
                "Accept", "Cancel");

            if (!confirmed) return;

            IsLoading = true;
            StatusMessage = null;
            try
            {
                await Task.Run(() => SubscriptionGiftService.AcceptGift(gift.GiftId, _currentUserId));
                await LoadGiftsAsync();
                StatusMessage = $"{gift.TierDisplayName} activated! Enjoy your subscription.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to accept gift: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeclineGiftAsync(SubscriptionGift gift)
        {
            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                "Decline Gift",
                $"Decline {gift.TierDisplayName} from {gift.SenderName}?",
                "Decline", "Keep");

            if (!confirmed) return;

            IsLoading = true;
            StatusMessage = null;
            try
            {
                await Task.Run(() => SubscriptionGiftService.DeclineGift(gift.GiftId, _currentUserId));
                await LoadGiftsAsync();
                StatusMessage = "Gift declined.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to decline gift: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CancelSentGiftAsync(SubscriptionGift gift)
        {
            bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
                "Cancel Gift",
                $"Cancel the {gift.TierDisplayName} gift to {gift.RecipientName}?",
                "Cancel Gift", "Keep");

            if (!confirmed) return;

            IsLoading = true;
            StatusMessage = null;
            try
            {
                await Task.Run(() => SubscriptionGiftService.CancelGift(gift.GiftId, _currentUserId));
                await LoadGiftsAsync();
                StatusMessage = "Gift cancelled.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to cancel gift: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
