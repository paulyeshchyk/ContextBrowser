using System.Text.RegularExpressions;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature;

public static class CSharpMethodSignatureExtensions
{
    public static string GetFullName(this ISignature signature)
    {
        return $"{signature.Raw}";
    }

    public static string GetMethodName(this ISignature signature)
    {
        return $"{signature.MethodName}";
    }

    public static string GetShortName(this ISignature signature)
    {
        return $"{signature.MethodName}";
    }

    public static string GetIdentifier(this ISignature signature)
    {
        return $"{signature.Raw}";
    }

    public static string GetNamespace(this ISignature signature)
    {
        return $"{signature.Namespace}";
    }
}