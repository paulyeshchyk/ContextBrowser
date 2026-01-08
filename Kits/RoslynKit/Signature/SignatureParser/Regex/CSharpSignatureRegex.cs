using System.Text.RegularExpressions;

namespace RoslynKit.Signature;

/// <summary>
/// Содержит строковые константы регулярных выражений для парсинга сигнатур C#.
/// Эти константы могут быть легко преобразованы в ресурсы (например, в .resx файл).
/// </summary>
public static class CSharpSignatureRegex
{
    // Регулярное выражение для поиска Namespace + Class + Method + Arguments
    // Включает сложные балансирующие группы для обработки обобщений (<...>) в типах и методах.
    public const string SNamespaceClassMethodArgsRegexTemplate = @"
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

    // Регулярное выражение для поиска Namespace + Class, например, 'SomeNs.SomeClass.Method'
    // Группа 1: Namespace (или TypeAndClass), Группа 2: MethodName
    public const string NamespaceClassRegex = @"([A-Z][a-zA-Z0-9_])\.([a-zA-Z0-9_]+)(\(\))?";

    // Регулярное выражение для поиска первого Namespace (частично)
    public const string NamespaceRegex = @"\b([A-Z][a-zA-Z0-9_]+)\.";

    // Регулярное выражение для разделения полного имени класса/типа на Namespace и Class.
    // Группа 1: Namespace (все до последней точки)
    // Группа 2: ClassName (все после последней точки)
    public const string FullTypeSplitRegex = @"^(?<Namespace>.*)\.(?<ClassName>[^.]+)$";

    public const string DelegateSignatureRegex = @"
        ^
        # Опциональный модификатор (public)
        (
            [^ ]+? \s+
        )?
        # 1. Result Type
        (?<ResultType>
            [^ ]+?
            (?:<
                (?>[^<>]+|(?<OpenR><)|(?<-OpenR>>))*(?(OpenR)(?!))
            >)?
        )
        \s+
        delegate \s+
        # 2. Имя делегата (MethodName, без ClassName/Namespace)
        (?<MethodName>
            [a-zA-Z0-9_]+
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
}
