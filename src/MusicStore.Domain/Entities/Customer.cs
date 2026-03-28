namespace MusicStore.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public LoyaltyTier LoyaltyTier { get; set; } = LoyaltyTier.Bronze;
    public decimal TotalSpent { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
}
