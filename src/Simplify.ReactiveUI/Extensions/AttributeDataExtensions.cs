using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Simplify.ReactiveUI.Extensions;

internal static class AttributeDataExtensions
{
    public static IEnumerable<T?> GetConstructorArguments<T>(this AttributeData attributeData)
        where T : class
    {
        static IEnumerable<T?> Enumerate(IEnumerable<TypedConstant> constants)
        {
            foreach (var constant in constants)
            {
                if (constant.IsNull)
                    yield return null;

                if (constant is { Kind: TypedConstantKind.Primitive, Value: T value })
                    yield return value;
                else if (constant.Kind == TypedConstantKind.Array)
                    foreach (var item in Enumerate(constant.Values))
                        yield return item;
            }
        }

        return Enumerate(attributeData.ConstructorArguments);
    }

    public static string? GetGenericType(this AttributeData attributeData)
    {
        var success = attributeData?.AttributeClass?.ToDisplayString();
        var start = success?.IndexOf('<') + 1 ?? 0;
        return success?.Substring(start, success.Length - start - 1);
    }
}
