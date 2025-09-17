namespace Simplify.ReactiveUI.Extensions;

internal static class StringExtensions
{
    public static string? GetGenericType(this string? typename)
    {
        var start = typename?.IndexOf('<') + 1 ?? 0;
        if (start == 0)
            return null;
        return typename?.Substring(start, typename.Length - start - 1);
    }
}
