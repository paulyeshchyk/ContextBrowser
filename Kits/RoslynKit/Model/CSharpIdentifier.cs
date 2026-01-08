using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ContextBrowserKit.Options;
using SemanticKit.Model.Options;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature;

public class CSharpIdentifier : ISignatureTypeIdentifier
{
    public string Key => "csharp";
    public string DisplayName => "C# Signature";
}