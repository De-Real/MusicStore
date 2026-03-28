using MusicStore.Domain.Entities;

namespace MusicStore.Application.DTOs.Customer;

public class UpdateCustomerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public LoyaltyTier LoyaltyTier { get; set; }
}
