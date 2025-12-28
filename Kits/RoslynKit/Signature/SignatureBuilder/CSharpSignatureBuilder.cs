using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace RoslynKit.AWrappers;

// context: roslyn, signature, build
public class CSharpSignatureBuilder : SignatureBuilder
{
    public override string Build(ISymbol symbol)
    {
        return symbol switch
        {
            IMethodSymbol methodSymbol => new CSharpSignatureBuilderMethod(this).Build(methodSymbol),
            ITypeSymbol typeSymbol => new CSharpSignatureBuilderType(this).Build(typeSymbol),
            _ => new CSharpSignatureBuilderDefault(this).Build(symbol)
        };
    }
}