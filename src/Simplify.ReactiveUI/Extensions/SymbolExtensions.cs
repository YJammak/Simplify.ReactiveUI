using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Simplify.ReactiveUI.Extensions;

internal static class SymbolExtensions
{
    public static bool TryGetAttributeWithFullyQualifiedMetadataName(
        this ISymbol symbol,
        string name,
        out AttributeData? attributeData)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var className = attribute.AttributeClass?.ToDisplayString();
            if (string.IsNullOrWhiteSpace(className) || !Regex.IsMatch(className, $"^{name}(<.+>)?$"))
                continue;

            attributeData = attribute;
            return true;
        }

        attributeData = null;
        return false;
    }
}
