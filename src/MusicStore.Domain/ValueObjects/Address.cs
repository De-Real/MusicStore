namespace MusicStore.Domain.ValueObjects;

public record Address(string Street, string City, string Country, string? PostalCode)
{
    public static Address Create(string street, string city, string country, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty.", nameof(country));

        return new Address(street.Trim(), city.Trim(), country.Trim(), postalCode?.Trim());
    }

    public override string ToString() => PostalCode is not null
        ? $"{Street}, {City}, {Country} {PostalCode}"
        : $"{Street}, {City}, {Country}";
}
