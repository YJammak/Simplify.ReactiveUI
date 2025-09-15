namespace Simplify.ReactiveUI.Test;

[SplatRegister]
public class Address
{
    public required string AddressLine1 { get; init; }
    public required string AddressLine2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; }
}
