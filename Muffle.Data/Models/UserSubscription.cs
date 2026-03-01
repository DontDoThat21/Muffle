namespace Muffle.Data.Models
{
    public enum SubscriptionTier
    {
        Free = 0,
        Premium = 1,
        PremiumPlus = 2
    }

    public enum SubscriptionStatus
    {
        None = 0,
        Active = 1,
        Expired = 2,
        Cancelled = 3
    }

    public class UserSubscription
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public SubscriptionTier Tier { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public bool IsActive => Status == SubscriptionStatus.Active && ExpiresAt > DateTime.UtcNow;
        public bool IsPremium => IsActive && Tier >= SubscriptionTier.Premium;
        public bool IsPremiumPlus => IsActive && Tier == SubscriptionTier.PremiumPlus;

        public string TierDisplayName => Tier switch
        {
            SubscriptionTier.Premium => "Muffle Premium",
            SubscriptionTier.PremiumPlus => "Muffle Premium+",
            _ => "Free"
        };

        public string MonthlyPrice => Tier switch
        {
            SubscriptionTier.Premium => "$4.99/mo",
            SubscriptionTier.PremiumPlus => "$9.99/mo",
            _ => "Free"
        };
    }

    public class SubscriptionFeature
    {
        public string Name { get; set; }
        public string FreeValue { get; set; }
        public string PremiumValue { get; set; }
        public string PremiumPlusValue { get; set; }
    }
}
