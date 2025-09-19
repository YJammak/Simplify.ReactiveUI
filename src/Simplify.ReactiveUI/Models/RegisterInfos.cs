using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Simplify.ReactiveUI.Models;

public record struct RegisterInfos
{
    public Diagnostic? Diagnostic { get; set; }

    public RegisterType Type { get; set; }

    public IEnumerable<string>? InterfaceNames { get; set; }

    public IEnumerable<RegisterInfo> Infos { get; set; }

    public readonly bool Equals(RegisterInfos other)
    {
        return Type == other.Type &&
               Infos.SequenceEqual(other.Infos);
    }

    public readonly override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Type.GetHashCode();
            hashCode = (hashCode * 397) ^ (Diagnostic?.GetHashCode() ?? 0);
            hashCode = (hashCode * 397) ^ (InterfaceNames?.Aggregate(hashCode, (current, prop) =>
                current ^ prop.GetHashCode()) ?? 0);
            hashCode = (hashCode * 397) ^ Infos.Aggregate(hashCode, (current, prop) =>
                current ^ prop.GetHashCode());
            return hashCode;
        }
    }
}
