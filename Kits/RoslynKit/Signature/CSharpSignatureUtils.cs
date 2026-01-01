using System.Text.RegularExpressions;
using SemanticKit.Model.Signature;

namespace RoslynKit.Signature;

internal static class CSharpSignatureUtils
{
    public static SignatureDefault Parse(string input)
    {
        // ищем первую точку после namespace
        var regex = new Regex(@"\b([A-Z][a-zA-Z0-9_]+)\.");
        var nsMatch = regex.Match(input);
        if (!nsMatch.Success)
        {
            // предположим, что это функция, напр, nameof()
            return new SignatureDefault(
                ResultType: "void",
                Namespace: "FakeNs",
                ClassName: "FakeClass1",
                MethodName: input,
                Arguments: string.Empty,
                Raw: input);

            //throw new ArgumentException("Не удалось разобрать часть с Namespace", nameof(input));
        }

        var index = nsMatch.Index;

        // всё до namespace = ResultType
        string resultType = input[..index].Trim();

        // теперь матчим Namespace + Class + Method + Args
        string pattern = @"
            ^
            # 1. Result Type
            (?<ResultType>
                [^ ]+?
                (?:<
                    (?>[^<>]+|(?<OpenR><)|(?<-OpenR>>))*(?(OpenR)(?!))
                >)?
            )
            \s+
            # 2. Полное имя метода
            (?<FullName>
                (?<TypeAndClass>
                    (?:[a-zA-Z0-9_]+\.)*
                    [a-zA-Z0-9_]+
                    (?:<
                        (?>[^<>]+|(?<OpenC><)|(?<-OpenC>>))*(?(OpenC)(?!))
                    >)?
                )
                \.
                (?<MethodName>
                    [a-zA-Z0-9_]+
                    (?:<
                        (?>[^<>]+|(?<OpenM><)|(?<-OpenM>>))*(?(OpenM)(?!))
                    >)?
                )
            )
            \(
                (?<Arguments>
                    (?>
                        [^()<>]+
                        | \( (?<Depth>)
                        | \) (?<-Depth>)
                        | <(?<Depth>)
                        | >(?<-Depth>)
                    )*
                    (?(Depth)(?!))
                )
            \)
            $
        ";

        var match = Regex.Match(input[index..], pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        if (!match.Success)
        {
            string p2 = @"([A-Z][a-zA-Z0-9_])\.([a-zA-Z0-9_]+)(\(\))?";
            var m2 = Regex.Match(input, p2, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            return new SignatureDefault(
                        ResultType: "void",
                        Namespace: (m2.Success) ? m2.Groups[1].Value : "Unknown namespace",
                        ClassName: "Unknown class",
                        MethodName: (m2.Success) ? m2.Groups[2].Value : "Unknown method",
                        Arguments: string.Empty,
                        Raw: input);
        }

        return new SignatureDefault(
            ResultType: resultType,
            Namespace: match.Groups["Namespace"].Value,
            ClassName: match.Groups["ClassName"].Value,
            MethodName: match.Groups["MethodName"].Value,
            Arguments: match.Groups["Arguments"].Value.Trim(),
            Raw: input);
    }
}

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