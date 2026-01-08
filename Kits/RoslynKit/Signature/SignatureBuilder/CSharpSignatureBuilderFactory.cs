using ContextKit.Model;
using Microsoft.CodeAnalysis;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature.SignatureBuilder;

public static class CSharpSignatureBuilderFactory
{
    public static SignatureBuilder<ISymbol> Create(MethodRepresentationStyle style)
    {
        var builder = new CSharpSignatureBuilder();

        // Флаг для управления квалификацией ТИПОВ
        bool qualifyTypes = false;

        switch (style)
        {
            case MethodRepresentationStyle.Minimal:
                // Actor -> Transition: Remove()
                builder.SetIncludeContainingType(false)
                       .SetIncludeReturnType(false)
                       .SetIncludeParameters(false)
                       .SetIncludeGenerics(false)
                       .SetIncludeNamespace(false);
                break;

            case MethodRepresentationStyle.MinimalButType:
                // Actor -> Transition: Remove()
                builder.SetIncludeContainingType(true)
                       .SetIncludeReturnType(false)
                       .SetIncludeParameters(false)
                       .SetIncludeGenerics(false)
                       .SetIncludeNamespace(false);
                break;

            case MethodRepresentationStyle.Signature:
                // Transition -> Actor: bool Remove(string actionName, List<string> args)
                builder.SetIncludeContainingType(true)
                       .SetIncludeReturnType(true)
                       .SetIncludeParameters(true)
                       .SetIncludeGenerics(true)
                       .SetIncludeNamespace(false); // Namespace только для содержащего типа
                break;

            case MethodRepresentationStyle.QualifiedSignature:
                // My.NS.Transition -> Actor: System.Boolean Remove(string actionName, List<string> args)
                builder.SetIncludeContainingType(true)
                       .SetIncludeReturnType(true)
                       .SetIncludeParameters(true)
                       .SetIncludeGenerics(true)
                       .SetIncludeNamespace(true);
                // !!! Здесь мы хотим квалифицировать класс и возвращаемый тип, но не аргументы
                // Это потребует дополнительного флага для управления квалификацией ТИПОВ
                qualifyTypes = true; // Квалификация возвращаемого типа (и generic параметров метода)
                break;

            case MethodRepresentationStyle.FullTypeArguments:
                // My.NS.Transition -> Actor: System.Boolean Remove(System.String actionName, System.Collections.Generic.List<T> args)
                builder.SetIncludeContainingType(true)
                       .SetIncludeReturnType(true)
                       .SetIncludeParameters(true)
                       .SetIncludeGenerics(true)
                       .SetIncludeNamespace(true);
                // Требуется полная квалификация всех типов: возвращаемый тип, generic-параметры и аргументы
                qualifyTypes = true;
                break;

            case MethodRepresentationStyle.Debug:
                // public System.Boolean My.NS.Transition.Remove(System.String actionName, System.Collections.Generic.List<T> args)
                // (Максимальная детализация)
                builder.SetIncludeContainingType(true)
                       .SetIncludeReturnType(true)
                       .SetIncludeParameters(true)
                       .SetIncludeGenerics(true)
                       .SetIncludeNamespace(true);
                qualifyTypes = true;
                break;
        }

        builder.SetTypeQualification(qualifyTypes);
        return builder;
    }
}
