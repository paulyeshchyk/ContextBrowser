using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Semantic.Builder;

namespace RoslynKit.Syntax.Parser;

public class CSharpTypePropertyParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly PropertyContextInfoBuilder<TContext> _propertyContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly RecordContextInfoBuilder<TContext> _recordSyntaxBuilder;
    private readonly TypeContextInfoBulder<TContext> _typeSyntaxBuilder;
    private readonly EnumContextInfoBuilder<TContext> _enumSyntaxBuilder;
    private readonly IContextCollector<TContext> _collector;

    public CSharpTypePropertyParser(
        IContextCollector<TContext> collector,
        PropertyContextInfoBuilder<TContext> propertyContextInfoBuilder,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        TypeContextInfoBulder<TContext> typeSyntaxBuilder,
        RecordContextInfoBuilder<TContext> recordSyntaxBuilder,
        EnumContextInfoBuilder<TContext> enumSyntaxBuilder,
        OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
        _propertyContextInfoBuilder = propertyContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _typeSyntaxBuilder = typeSyntaxBuilder;
        _recordSyntaxBuilder = recordSyntaxBuilder;
        _enumSyntaxBuilder = enumSyntaxBuilder;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is PropertyDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not PropertyDeclarationSyntax propertySyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not PropertyDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 1 - property syntax");

        //1.Создание контекста для самого свойства
        var propertyContext = _propertyContextInfoBuilder.BuildContextInfoForProperty(propertySyntax, model);
        if(propertyContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for property.", LogLevelNode.End);
            return;
        }

        // 2. Парсинг комментариев
        _triviaCommentParser.Parse(propertySyntax, propertyContext);

        // 3. Обработка типа свойства (рекурсивный обход)
        var propertyTypeSyntax = propertySyntax.Type;
        if(propertyTypeSyntax != null)
        {
            var typeSymbol = model.GetTypeInfo(propertyTypeSyntax).Type;

            // Проверяем, что это не базовый тип (например, int, string, object)
            if(typeSymbol != null && typeSymbol.Locations.Any())
            {
                // Проверяем, существует ли уже контекст для этого типа, чтобы избежать рекурсии
                var cnt = _collector.BySymbolDisplayName.GetValueOrDefault(typeSymbol.ToDisplayString());
                if(cnt == null)
                {
                    // Получаем синтаксический узел из семантического символа
                    var typeDeclarationSyntax = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

                    if(typeDeclarationSyntax != null)
                    {
                        // Если это TypeDeclarationSyntax (класс, struct и т.д.), парсим его
                        if(typeDeclarationSyntax is ClassDeclarationSyntax tds)
                        {
                            // Рекурсивный вызов парсера типа
                            _typeSyntaxBuilder.BuildContextInfoForType(tds, model, string.Empty, null);
                        }
                        else if(typeDeclarationSyntax is RecordDeclarationSyntax rds)
                        {
                            // Рекурсивный вызов парсера типа
                            _recordSyntaxBuilder.BuildContextInfoForRecord(rds, model, string.Empty, null);
                        }
                        else if(typeDeclarationSyntax is EnumDeclarationSyntax eds)
                        {
                            // Рекурсивный вызов парсера типа
                            _enumSyntaxBuilder.BuildContextInfoForEnum(eds, model);
                        }
                        else
                        {
                            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax was not parsed: {typeDeclarationSyntax} ");
                        }
                    }
                    else
                    {
                        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Trace, $"Symbol has no declared syntax: {typeSymbol} ");
                    }
                }
            }
        }
    }
}