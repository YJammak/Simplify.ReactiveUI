using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Simplify.ReactiveUI.Generators;

[Generator]
public class BindGenerator : IIncrementalGenerator
{
    private const string OneWayBindAttribute =
        """
        #nullable enable

        using System;

        namespace Simplify.ReactiveUI;

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        internal class OneWayBindAttribute : Attribute
        {
            public string ViewModelValue { get; set; }

            public string View { get; set; }

            public string ViewValue { get; set; }

            public OneWayBindAttribute(
                string viewModelValue,
                string view,
                string viewValue)
            {
                ViewModelValue = viewModelValue;
                View = view;
                ViewValue = viewValue;
            }
        }
        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("OneWayBindAttribute.g.cs",
                SourceText.From(OneWayBindAttribute, Encoding.UTF8)));

        var oneWayBindProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Simplify.ReactiveUI.OneWayBindAttribute",
                Predicate,
                OneWayBindTransform);
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        return true;
    }

    private static object OneWayBindTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        return null;
    }
}
