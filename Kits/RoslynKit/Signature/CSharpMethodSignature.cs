using System.Text.RegularExpressions;
using SemanticKit.Model;

namespace RoslynKit.Signature;


public record struct CSharpMethodSignature(string ResultType,
                                           string Namespace,
                                           string ClassName,
                                           string MethodName,
                                           string Arguments,
                                           string Raw) : ICustomMethodSignature
{
    public readonly bool Equals(ICustomMethodSignature other)
    {
        if (other is not CSharpMethodSignature csharpms)
            return false;

        var result = csharpms.Raw.Equals(Raw);
        return result;
    }
}