using System;
using System.Text.RegularExpressions;
using SemanticKit.Model;

namespace RoslynKit.Signature;

internal static class SignatureUtils
{
    public static CSharpMethodSignature Parse(string input)
    {
        // ищем первую точку после namespace
        var regex = new Regex(@"\b([A-Z][a-zA-Z0-9_]+)\.");
        var nsMatch = regex.Match(input);
        if (nsMatch == null || !nsMatch.Success)
        {
            throw new ArgumentException("Не удалось разобрать часть с Namespace", nameof(input));
        }

        var index = nsMatch.Index;
        // всё до namespace = ResultType
        string resultType = input[..index].Trim();

        // теперь матчим Namespace + Class + Method + Args
        string pattern = @"
            (?<Namespace>[a-zA-Z0-9_]+)
            \.
            (?<ClassName>
                (?:[a-zA-Z0-9_]+
                    (?:<
                        (?>[^<>]+|(?<Open><)|(?<-Open>>))*(?(Open)(?!))
                    >)?
                    \.
                )*
                [a-zA-Z0-9_]+
                (?:<
                    (?>[^<>]+|(?<Open><)|(?<-Open>>))*(?(Open)(?!))
                >)?
            )
            \.
            (?<MethodName>[a-zA-Z0-9_]+
                (?:<
                    (?>[^<>]+|(?<Open><)|(?<-Open>>))*(?(Open)(?!))
                >)?
            )
            \(
                (?<Arguments>
                    (?>
                        [^()<>]+
                        | \((?<Depth>)
                        | \)(?<-Depth>)
                        | <(?<Open>)
                        | >(?<-Open>)
                    )*
                    (?(Depth)(?!))
                    (?(Open)(?!))
                )
            \)
        ";

        var match = Regex.Match(input[index..], pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        if (!match.Success)
        {
            string p2 = @"([A-Z][a-zA-Z0-9_])\.([a-zA-Z0-9_]+)(\(\))?";
            var m2 = Regex.Match(input, p2, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            return new CSharpMethodSignature(
                        ResultType: "void",
                        Namespace: (m2.Success) ? m2.Groups[1].Value : "FakeNs",
                        ClassName: "FakeClass",
                        MethodName: (m2.Success) ? m2.Groups[2].Value : "FakeMeth",
                        Arguments: string.Empty,
                        Raw: input
            );
        }


        return new CSharpMethodSignature(
            ResultType: resultType,
            Namespace: match.Groups["Namespace"].Value,
            ClassName: match.Groups["ClassName"].Value,
            MethodName: match.Groups["MethodName"].Value,
            Arguments: match.Groups["Arguments"].Value.Trim(),
            Raw: input
        );
    }

}


public static class CSharpMethodSignatureExtensions
{
    public static CSharpMethodSignature FakeCSharpMethodSignature() => new(ResultType: "void", Namespace: "FakeNamespace", ClassName: "FakeClass", MethodName: "FakeMethod", Arguments: string.Empty, Raw: $"void FakeNamespace.FakeClass.FakeMethod()");


    public static string GetFullName(this ICustomMethodSignature signature)
    {
        return $"{signature.Raw}";
    }

    public static string GetMethodName(this ICustomMethodSignature signature)
    {
        return $"{signature.MethodName}";
    }

    public static string GetShortName(this ICustomMethodSignature signature)
    {
        return $"{signature.MethodName}";
    }

    public static string GetIdentifier(this ICustomMethodSignature signature)
    {
        return $"{signature.Raw}";
    }

    public static string GetNamespace(this ICustomMethodSignature signature)
    {
        return $"{signature.Namespace}";
    }
}