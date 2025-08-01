﻿using Microsoft.CodeAnalysis;

namespace ContextKit.Model;

public interface IContextWithReferences<T>
    where T : IContextWithReferences<T>
{
    string? Name { get; set; }

    string? Action { get; set; }

    string SymbolName { get; set; }

    HashSet<string> Domains { get; }

    ContextInfoElementType ElementType { get; set; }

    HashSet<T> References { get; }

    HashSet<T> InvokedBy { get; }

    public T? MethodOwner { get; set; }

    public SyntaxNode? SyntaxNode { get; set; }
}