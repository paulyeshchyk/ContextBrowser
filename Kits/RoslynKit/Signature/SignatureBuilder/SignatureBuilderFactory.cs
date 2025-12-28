using System;
using System.Linq;
using System.Text;
using ContextKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

public static class SignatureBuilderFactory
{
    public static SignatureBuilder Create(MethodRepresentationStyle style)
    {
        var builder = new CSharpSignatureBuilder();

        // Флаг для управления квалификацией ТИПОВ
        bool qualifyTypes = false;

        switch (style)
        {
            case MethodRepresentationStyle.Minimal:
                // Actor -> Transition: Remove()
                builder.IncludeContainingType(false)
                       .IncludeReturnType(false)
                       .IncludeParameters(false)
                       .IncludeGenerics(false)
                       .IncludeNamespace(false);
                break;

            case MethodRepresentationStyle.MinimalButType:
                // Actor -> Transition: Remove()
                builder.IncludeContainingType(true)
                       .IncludeReturnType(false)
                       .IncludeParameters(false)
                       .IncludeGenerics(false)
                       .IncludeNamespace(false);
                break;

            case MethodRepresentationStyle.Signature:
                // Transition -> Actor: bool Remove(string actionName, List<string> args)
                builder.IncludeContainingType(true)
                       .IncludeReturnType(true)
                       .IncludeParameters(true)
                       .IncludeGenerics(true)
                       .IncludeNamespace(false); // Namespace только для содержащего типа
                break;

            case MethodRepresentationStyle.QualifiedSignature:
                // My.NS.Transition -> Actor: System.Boolean Remove(string actionName, List<string> args)
                builder.IncludeContainingType(true)
                       .IncludeReturnType(true)
                       .IncludeParameters(true)
                       .IncludeGenerics(true)
                       .IncludeNamespace(true);
                // !!! Здесь мы хотим квалифицировать класс и возвращаемый тип, но не аргументы
                // Это потребует дополнительного флага для управления квалификацией ТИПОВ
                qualifyTypes = true; // Квалификация возвращаемого типа (и generic параметров метода)
                break;

            case MethodRepresentationStyle.FullTypeArguments:
                // My.NS.Transition -> Actor: System.Boolean Remove(System.String actionName, System.Collections.Generic.List<T> args)
                builder.IncludeContainingType(true)
                       .IncludeReturnType(true)
                       .IncludeParameters(true)
                       .IncludeGenerics(true)
                       .IncludeNamespace(true);
                // Требуется полная квалификация всех типов: возвращаемый тип, generic-параметры и аргументы
                qualifyTypes = true;
                break;

            case MethodRepresentationStyle.Debug:
                // public System.Boolean My.NS.Transition.Remove(System.String actionName, System.Collections.Generic.List<T> args)
                // (Максимальная детализация)
                builder.IncludeContainingType(true)
                       .IncludeReturnType(true)
                       .IncludeParameters(true)
                       .IncludeGenerics(true)
                       .IncludeNamespace(true);
                qualifyTypes = true;
                break;
        }

        builder.SetTypeQualification(qualifyTypes);
        return builder;
    }
}
