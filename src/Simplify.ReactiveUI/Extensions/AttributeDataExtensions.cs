using Microsoft.CodeAnalysis;

namespace Simplify.ReactiveUI.Extensions;

internal static class AttributeDataExtensions
{
    public static string? GetGenericType(this AttributeData attributeData)
    {
        return attributeData.AttributeClass?.ToDisplayString().GetGenericType();
    }
}
