using System;

namespace ContextKit.Model;

public enum MethodRepresentationStyle
{
    // High-level overview, only class and method name.
    Minimal,

    // High-level overview, only class and method name.
    MinimalButType,

    // Standard C# signature without namespaces. Good for clean UML diagrams.
    Signature,

    // Qualified class name and return type. Resolves ambiguity between classes with the same name.
    QualifiedSignature,

    // Full type names for all arguments. Critical when tracing data flow and complex types.
    FullTypeArguments,

    // Maximum information: includes access modifiers and argument names. For detailed comparison with source code.
    Debug
}