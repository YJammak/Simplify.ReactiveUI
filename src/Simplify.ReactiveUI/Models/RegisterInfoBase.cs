namespace Simplify.ReactiveUI.Models;

public abstract class RegisterInfoBase
{
    public string? Contract { get; set; }

    public string? ServiceType { get; set; }

    public string ImplementationType { get; set; } = null!;
}
