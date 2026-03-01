namespace Muffle.Data.Models
{
    public enum GiftStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Cancelled = 3
    }

    public class SubscriptionGift
    {
        public int GiftId { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public SubscriptionTier Tier { get; set; }
        public GiftStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? RedeemedAt { get; set; }

        // Denormalized display names (populated via JOIN or separate query)
        public string SenderName { get; set; }
        public string RecipientName { get; set; }

        public string TierDisplayName => Tier switch
        {
            SubscriptionTier.PremiumPlus => "Muffle Premium+",
            SubscriptionTier.Premium => "Muffle Premium",
            _ => "Free"
        };

        public string TierPrice => Tier switch
        {
            SubscriptionTier.PremiumPlus => "$9.99",
            SubscriptionTier.Premium => "$4.99",
            _ => "Free"
        };

        public string StatusDisplayName => Status switch
        {
            GiftStatus.Accepted => "Accepted",
            GiftStatus.Declined => "Declined",
            GiftStatus.Cancelled => "Cancelled",
            _ => "Pending"
        };

        public string StatusColor => Status switch
        {
            GiftStatus.Accepted => "#43B581",
            GiftStatus.Declined => "#F04747",
            GiftStatus.Cancelled => "#757575",
            _ => "#F39C12"
        };

        public bool IsPending => Status == GiftStatus.Pending;
    }
}
