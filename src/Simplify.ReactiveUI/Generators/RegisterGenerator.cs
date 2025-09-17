using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Simplify.ReactiveUI.Extensions;
using Simplify.ReactiveUI.Models;

namespace Simplify.ReactiveUI.Generators;

[Generator]
public class RegisterGenerator : IIncrementalGenerator
{
    private const string SplatRegisterAttribute =
        """
        #nullable enable

        using System;

        namespace Simplify.ReactiveUI;

        [AttributeUsage(AttributeTargets.Class)]
        internal class SplatRegisterAttribute : Attribute
        {
            /// <summary>
            /// The types array which is used for the registration.
            /// </summary>
            public Type[]? ServiceTypes { get; }
            
            /// <summary>
            /// An optional contract value which will indicates to only generate the value if this contract is specified.
            /// </summary>
            public string? Contract { get; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool IncludeBaseClass { get; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool IncludeInterfaces { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="serviceTypes">The types array which is used for the registration.</param>
            /// <param name="contract">An optional contract value which will indicates to only generate the value if this contract is specified.</param>
            /// <param name="includeBaseClass"></param>
            /// <param name="includeInterfaces"></param>
            public SplatRegisterAttribute(
                Type[]? serviceTypes,
                string? contract = null,
                bool includeBaseClass = false,
                bool includeInterfaces = false)
            {
                ServiceTypes = serviceTypes;
                Contract = contract;
                IncludeBaseClass = includeBaseClass;
                IncludeInterfaces = includeInterfaces;
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="serviceType">The type which is used for the registration.</param>
            /// <param name="contract">An optional contract value which will indicates to only generate the value if this contract is specified.</param>
            /// <param name="includeBaseClass"></param>
            /// <param name="includeInterfaces"></param>
            public SplatRegisterAttribute(
                Type? serviceType = null,
                string? contract = null,
                bool includeBaseClass = false,
                bool includeInterfaces = false)
            {
                if (serviceType != null)
                    ServiceTypes = [serviceType];
                Contract = contract;
                IncludeBaseClass = includeBaseClass;
                IncludeInterfaces = includeInterfaces;
            }
        }
        """;

    private const string SplatRegisterConstantAttribute =
        """
        #nullable enable

        using System;

        namespace Simplify.ReactiveUI;

        [AttributeUsage(AttributeTargets.Class)]
        internal class SplatRegisterConstantAttribute : Attribute
        {
            public Type[]? ServiceTypes { get; }

            public string? Contract { get; }

            public bool IncludeBaseClass { get; }

            public bool IncludeInterfaces { get; }

            public SplatRegisterConstantAttribute(
                Type[]? serviceTypes,
                string? contract = null,
                bool includeBaseClass = false,
                bool includeInterfaces = false)
            {
                ServiceTypes = serviceTypes;
                Contract = contract;
                IncludeBaseClass = includeBaseClass;
                IncludeInterfaces = includeInterfaces;
            }
            
            public SplatRegisterConstantAttribute(
                Type? serviceType = null,
                string? contract = null,
                bool includeBaseClass = false,
                bool includeInterfaces = false)
            {
                if (serviceType != null)
                    ServiceTypes = [serviceType];
                Contract = contract;
                IncludeBaseClass = includeBaseClass;
                IncludeInterfaces = includeInterfaces;
            }
        }
        """;

    private const string SplatRegisterLazySingletonAttribute =
        """
        #nullable enable

        using System;

        namespace Simplify.ReactiveUI;

        [AttributeUsage(AttributeTargets.Class)]
        internal class SplatRegisterLazySingletonAttribute : Attribute
        {
            public Type[]? ServiceTypes { get; }
            
            public string? Contract { get; }
            
            public bool IncludeBaseClass { get; }
            
            public bool IncludeInterfaces { get; }
            
            public SplatRegisterLazySingletonAttribute(
                Type[]? serviceTypes,
                string? contract = null,
                bool includeBaseClass = false,
                bool includeInterfaces = false)
            {
                ServiceTypes = serviceTypes;
                Contract = contract;
                IncludeBaseClass = includeBaseClass;
                IncludeInterfaces = includeInterfaces;
            }
            
            public SplatRegisterLazySingletonAttribute(
                Type? serviceType = null,
                string? contract = null,
                bool includeBaseClass = false,
                bool includeInterfaces = false)
            {
                if (serviceType != null)
                    ServiceTypes = [serviceType];
                Contract = contract;
                IncludeBaseClass = includeBaseClass;
                IncludeInterfaces = includeInterfaces;
            }
        }
        """;

    private const string SplatRegisterViewModelAttribute =
        """
        #nullable enable

        using System;

        namespace Simplify.ReactiveUI;

        [AttributeUsage(AttributeTargets.Class)]
        internal class SplatRegisterViewModelAttribute<T> : Attribute;

        [AttributeUsage(AttributeTargets.Class)]
        internal class SplatRegisterViewModelAttribute : Attribute
        {
            public Type ViewModel { get; }

            public SplatRegisterViewModelAttribute(Type viewModel)
            {
                ViewModel = viewModel;
            }
        }
        """;

    private const string RegisterClassTemplate =
        """
        //-------------------------------------------------------------------------------
        //<auto-generated>
        //    This code was generated by Simplify.ReactiveUI.Generators.RegisterGenerator{{generatedAt}}.
        //</auto-generated>
        //-------------------------------------------------------------------------------

        {{reactiveUI}}using Splat;

        namespace {{namespace}};

        public static class SplatRegisterExtensions
        {
            /// <summary>
            /// Register all items using SplatRegisterAttribute, SplatRegisterConstantAttribute, SplatRegisterLazySingletonAttribute, SplatRegisterViewModelAttribute
            /// </summary>
            public static void RegisterAll{{method}}(this IMutableDependencyResolver resolver)
            {
        {{registers}}
            }
        }
        """;

    private const string RegisterTemplate =
        """
                resolver.{{method}}({{implementationType}}{{serviceType}}{{contract}});
        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("SplatRegisterAttribute.g.cs",
                SourceText.From(SplatRegisterAttribute, Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("SplatRegisterConstantAttribute.g.cs",
                SourceText.From(SplatRegisterConstantAttribute, Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("SplatRegisterLazySingletonAttribute.g.cs",
                SourceText.From(SplatRegisterLazySingletonAttribute, Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("SplatRegisterViewModelAttribute.g.cs",
                SourceText.From(SplatRegisterViewModelAttribute, Encoding.UTF8)));

        var defaultNamespace = context.AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                // 尝试从项目配置中获取默认命名空间
                if (options.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNs))
                    return rootNs;

                if (options.GlobalOptions.TryGetValue("build_property.DefaultNamespace", out var defaultNs))
                    return defaultNs;

                // 如果都获取不到，返回空字符串
                return string.Empty;
            });

        var registerInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Simplify.ReactiveUI.SplatRegisterAttribute",
                Predicate,
                RegisterTransform)
            .Collect();

        var registerConstantInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Simplify.ReactiveUI.SplatRegisterConstantAttribute",
                Predicate,
                RegisterConstantTransform)
            .Collect();

        var registerLazySingletonInfos = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Simplify.ReactiveUI.SplatRegisterLazySingletonAttribute",
                Predicate,
                RegisterLazySingletonTransform)
            .Collect();

        var registerViewModelInfos = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                RegisterViewModelTransform)
            .Collect();

        var combinedData = defaultNamespace
            .Combine(registerInfos)
            .Combine(registerConstantInfos)
            .Combine(registerLazySingletonInfos)
            .Combine(registerViewModelInfos);

        context.RegisterSourceOutput(combinedData, Generate);
    }

    private void Generate(SourceProductionContext context,
        ((((string Namespace, ImmutableArray<RegisterInfos> RegisterInfos) Left, ImmutableArray<RegisterInfos>
            RegisterConstantInfos) Left,
            ImmutableArray<RegisterInfos> RegisterLazySingletonInfos) Left, ImmutableArray<RegisterInfos>
            RegisterViewModelInfos) args)
    {
        var namespaceString = args.Left.Left.Left.Namespace;

        var registerInfos = args.Left.Left.Left.RegisterInfos.IsDefaultOrEmpty
            ? []
            : args.Left.Left.Left.RegisterInfos.SelectMany(r => r.Infos).ToList();

        var registerConstantInfos = args.Left.Left.RegisterConstantInfos.IsDefaultOrEmpty
            ? []
            : args.Left.Left.RegisterConstantInfos.SelectMany(r => r.Infos).ToList();

        var registerLazySingletonInfos = args.Left.RegisterLazySingletonInfos.IsDefaultOrEmpty
            ? []
            : args.Left.RegisterLazySingletonInfos.SelectMany(r => r.Infos).ToList();

        var registerViewModelInfos = args.RegisterViewModelInfos.IsDefaultOrEmpty
            ? []
            : args.RegisterViewModelInfos.SelectMany(r => r.Infos).ToList();

        var hasDiagnostic = false;
        var registerDictionary = new Dictionary<string, RegisterInfo>();
        foreach (var info in registerInfos
                     .Concat(registerConstantInfos)
                     .Concat(registerLazySingletonInfos)
                     .Concat(registerViewModelInfos))
        {
            var key = $"{info.ImplementationType}_{info.ServiceType}_{info.Contract}";
            if (registerDictionary.TryGetValue(key, out _) && info.Location != null)
            {
                context.ReportDiagnostic(RegisterRepeated(info.Location, info.ImplementationType));
                hasDiagnostic = true;
                continue;
            }

            registerDictionary.Add(key, info);
        }

        if (hasDiagnostic)
            return;

        var builder = new StringBuilder();
        foreach (var info in registerInfos)
            builder.AppendLine(RegisterTemplate
                .Replace("{{method}}", "Register")
                .Replace("{{implementationType}}", $"() => new {info.ImplementationType}()")
                .Replace("{{serviceType}}",
                    string.IsNullOrWhiteSpace(info.ServiceType) ? "" : $", typeof({info.ServiceType})")
                .Replace("{{contract}}", $", {(string.IsNullOrWhiteSpace(info.Contract) ? "null" : info.Contract)}"));

        foreach (var info in registerConstantInfos)
            builder.AppendLine(RegisterTemplate
                .Replace("{{method}}", "RegisterConstant")
                .Replace("{{implementationType}}", $"new {info.ImplementationType}()")
                .Replace("{{serviceType}}",
                    string.IsNullOrWhiteSpace(info.ServiceType) ? "" : $", typeof({info.ServiceType})")
                .Replace("{{contract}}", $", {(string.IsNullOrWhiteSpace(info.Contract) ? "null" : info.Contract)}"));

        foreach (var info in registerLazySingletonInfos)
            builder.AppendLine(RegisterTemplate
                .Replace("{{method}}", "RegisterLazySingleton")
                .Replace("{{implementationType}}", $"() => new {info.ImplementationType}()")
                .Replace("{{serviceType}}",
                    string.IsNullOrWhiteSpace(info.ServiceType) ? "" : $", typeof({info.ServiceType})")
                .Replace("{{contract}}", $", {(string.IsNullOrWhiteSpace(info.Contract) ? "null" : info.Contract)}"));

        foreach (var info in registerViewModelInfos)
            builder.AppendLine(RegisterTemplate
                .Replace("{{method}}", "Register")
                .Replace("{{implementationType}}", $"() => new {info.ImplementationType}()")
                .Replace("{{serviceType}}", $", typeof({info.ServiceType})")
                .Replace("{{contract}}", ", null"));

        var registers = builder.Length > 0 ? builder.ToString(0, builder.Length - 2) : string.Empty;

        var output = RegisterClassTemplate
#if DEBUG
            .Replace("{{generatedAt}}", $" at {DateTime.Now:s}")
#else
            .Replace("{{generatedAt}}", string.Empty)
#endif
            .Replace("{{reactiveUI}}", registerViewModelInfos.Count == 0 ? "" : "using ReactiveUI;\r\n")
            .Replace("{{namespace}}",
                string.IsNullOrWhiteSpace(namespaceString) ? "Simplify.ReactiveUI" : namespaceString)
            .Replace("{{method}}", namespaceString.Replace(".", ""))
            .Replace("{{registers}}", registers);

        context.AddSource("SplatRegisterExtensions.g.cs", SourceText.From(output, Encoding.UTF8));
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        return true;
    }

    private static RegisterInfos RegisterTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        var infos = GetInfos(context, token);
        if (infos == null)
            return new RegisterInfos
            {
                Type = RegisterType.Normal,
                Infos = []
            };

        var (serviceTypes, className, contract, location) = infos.Value;

        if (serviceTypes == null)
            return new RegisterInfos
            {
                Type = RegisterType.Normal,
                Infos = []
            };

        if (serviceTypes.Count == 0)
            return new RegisterInfos
            {
                Type = RegisterType.Normal,
                Infos =
                [
                    new RegisterInfo
                    {
                        ImplementationType = className,
                        Contract = contract,
                        Location = location
                    }
                ]
            };

        return new RegisterInfos
        {
            Type = RegisterType.Normal,
            Infos = serviceTypes.Select(s => new RegisterInfo
            {
                ServiceType = s,
                ImplementationType = className,
                Contract = contract,
                Location = location
            }).ToList()
        };
    }

    private static RegisterInfos RegisterConstantTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        var infos = GetInfos(context, token);
        if (infos == null)
            return new RegisterInfos
            {
                Type = RegisterType.Constant,
                Infos = []
            };

        var (serviceTypes, className, contract, location) = infos.Value;

        if (serviceTypes == null)
            return new RegisterInfos
            {
                Type = RegisterType.Constant,
                Infos = []
            };

        if (serviceTypes.Count == 0)
            return new RegisterInfos
            {
                Type = RegisterType.Constant,
                Infos =
                [
                    new RegisterInfo
                    {
                        ImplementationType = className,
                        Contract = contract,
                        Location = location
                    }
                ]
            };

        return new RegisterInfos
        {
            Type = RegisterType.Constant,
            Infos = serviceTypes.Select(s => new RegisterInfo
            {
                ServiceType = s,
                ImplementationType = className,
                Contract = contract,
                Location = location
            }).ToList()
        };
    }

    private static RegisterInfos RegisterLazySingletonTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        var infos = GetInfos(context, token);
        if (infos == null)
            return new RegisterInfos
            {
                Type = RegisterType.LazySingleton,
                Infos = []
            };

        var (serviceTypes, className, contract, location) = infos.Value;

        if (serviceTypes.Count == 0)
            return new RegisterInfos
            {
                Type = RegisterType.LazySingleton,
                Infos =
                [
                    new RegisterInfo
                    {
                        ImplementationType = className,
                        Contract = contract,
                        Location = location
                    }
                ]
            };

        return new RegisterInfos
        {
            Type = RegisterType.LazySingleton,
            Infos = serviceTypes.Select(s => new RegisterInfo
            {
                ServiceType = s,
                ImplementationType = className,
                Contract = contract,
                Location = location
            }).ToList()
        };
    }

    private RegisterInfos RegisterViewModelTransform(
        GeneratorSyntaxContext context,
        CancellationToken token)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token);

        if (symbol is null)
            return new RegisterInfos
            {
                Type = RegisterType.ViewModel,
                Infos = []
            };

        if (!symbol.TryGetAttributeWithFullyQualifiedMetadataName(
                "Simplify.ReactiveUI.SplatRegisterViewModelAttribute",
                out var attribute))
            return new RegisterInfos
            {
                Type = RegisterType.ViewModel,
                Infos = []
            };

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token);
        if (classSymbol is not INamedTypeSymbol namedTypeSymbol)
            return new RegisterInfos
            {
                Type = RegisterType.ViewModel,
                Infos = []
            };

        var location = attribute!.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        var genericArgument = attribute.GetGenericType();
        var viewModel = string.IsNullOrWhiteSpace(genericArgument)
            ? attribute.ConstructorArguments[0].Value?.ToString()
            : genericArgument;
        if (string.IsNullOrWhiteSpace(viewModel))
            return new RegisterInfos
            {
                Type = RegisterType.ViewModel,
                Infos = []
            };

        var className = namedTypeSymbol.ToDisplayString();
        return new RegisterInfos
        {
            Type = RegisterType.ViewModel,
            Infos =
            [
                new RegisterInfo
                {
                    ServiceType = $"IViewFor<{viewModel}>",
                    ImplementationType = className,
                    Contract = null,
                    Location = location
                }
            ]
        };
    }

    private static (List<string> ServiceTypes, string ImplementationType, string? Contract, Location? Location)?
        GetInfos(
            GeneratorAttributeSyntaxContext syntaxContext,
            CancellationToken _)
    {
        var classSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.TargetNode);
        if (classSymbol is not INamedTypeSymbol namedTypeSymbol)
            return null;

        var attribute = syntaxContext.Attributes.Single();
        var serviceTypes = attribute.ConstructorArguments[0].Kind == TypedConstantKind.Array
            ? attribute.ConstructorArguments[0].Values.IsDefaultOrEmpty
                ? []
                : attribute.ConstructorArguments[0].Values
                    .Select(t => t.Value?.ToString() ?? "")
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList()
            : attribute.ConstructorArguments[0].Value == null
                ? []
                : [attribute.ConstructorArguments[0].Value?.ToString()];

        var contract = attribute.ConstructorArguments[1].Value as string;
        var includeBaseType = attribute.ConstructorArguments[2].Value as bool? ?? false;
        var includeInterfaces = attribute.ConstructorArguments[3].Value as bool? ?? false;

        var className = namedTypeSymbol.ToDisplayString();
        var baseClassName = GetDirectBaseClassName(namedTypeSymbol);

        if (includeBaseType && baseClassName != null)
            serviceTypes.Add(baseClassName);

        if (includeInterfaces)
        {
            var interfaces = namedTypeSymbol.Interfaces
                .Where(i => IsDirectlyImplementedInterface(namedTypeSymbol, i))
                .ToList();
            foreach (var @interface in interfaces)
            {
                var interfaceName = @interface.ToDisplayString();
                serviceTypes.Add(interfaceName);
            }
        }

        var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        return (serviceTypes, className, contract, location);
    }

    private static bool IsDirectlyImplementedInterface(INamedTypeSymbol namedTypeSymbol, ITypeSymbol @interface)
    {
        if (namedTypeSymbol.BaseType == null)
            return true;

        return !namedTypeSymbol.BaseType.Interfaces.Any(i =>
            SymbolEqualityComparer.Default.Equals(i, @interface));
    }

    private static string? GetDirectBaseClassName(INamedTypeSymbol namedTypeSymbol)
    {
        var directBaseClass = namedTypeSymbol.BaseType;
        if (directBaseClass == null)
            return null;

        var baseClassFullName = directBaseClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return baseClassFullName is "System.Object" or "object" ? null : baseClassFullName;
    }

    private static Diagnostic RegisterRepeated(Location location, string identifier)
    {
        var descriptor = new DiagnosticDescriptor(
            "RegisterRepeated",
            "Register Cannot Be Repeated",
            "Cannot generate a register for '{0}' because it is a duplicate.",
            "Simplify.ReactiveUI.Generators",
            DiagnosticSeverity.Error,
            true
        );
        var result = Diagnostic.Create(descriptor, location, identifier);

        return result;
    }
}
