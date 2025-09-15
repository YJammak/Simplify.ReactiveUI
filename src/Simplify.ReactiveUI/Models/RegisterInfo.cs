namespace Simplify.ReactiveUI.Models;

public record struct RegisterInfo
{
    public string? Contract { get; set; }

    public string? ServiceType { get; set; }

    public string ImplementationType { get; set; }

    public readonly bool Equals(RegisterInfo other)
    {
        return Contract == other.Contract &&
               ServiceType == other.ServiceType &&
               ImplementationType == other.ImplementationType;
    }

    public readonly override int GetHashCode()
    {
        unchecked
        {
            var hashCode = ImplementationType.GetHashCode();
            hashCode = (hashCode * 397) ^ (Contract != null ? Contract.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ServiceType != null ? ServiceType.GetHashCode() : 0);
            return hashCode;
        }
    }
}
