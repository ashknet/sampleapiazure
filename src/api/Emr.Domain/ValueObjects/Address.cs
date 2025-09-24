namespace Emr.Domain.ValueObjects;

public class Address : IEquatable<Address>
{
    public string Street1 { get; }
    public string? Street2 { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }
    
    public Address(string street1, string? street2, string city, string state, string postalCode, string country = "USA")
    {
        Street1 = street1;
        Street2 = street2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
    
    public bool Equals(Address? other)
    {
        if (other is null) return false;
        
        return Street1 == other.Street1 &&
               Street2 == other.Street2 &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }
    
    public override bool Equals(object? obj) => obj is Address address && Equals(address);
    
    public override int GetHashCode() => HashCode.Combine(Street1, Street2, City, State, PostalCode, Country);
    
    public override string ToString() => $"{Street1}, {City}, {State} {PostalCode}";
}