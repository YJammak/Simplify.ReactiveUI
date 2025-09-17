using Microsoft.CodeAnalysis;

namespace Simplify.ReactiveUI.Extensions;

internal static class AttributeDataExtensions
{
    public static string? GetGenericType(this AttributeData attributeData)
    {
        var success = attributeData?.AttributeClass?.ToDisplayString();
        var start = success?.IndexOf('<') + 1 ?? 0;
        if (start == 0)
            return null;
        return success?.Substring(start, success.Length - start - 1);
    }
}
