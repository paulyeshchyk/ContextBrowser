using ContextBrowser.ContextKit.Parser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Syntax.Parser.ContextInfo;

namespace RoslynKit.Syntax.Parser.Base;

public class PropertyDeclarationParser<TContext> : BaseSyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly PropertyContextInfoBuilder<TContext> _propertyContextInfoBuilder;
    private readonly CommentSyntaxTriviaContentInfoBuilder<TContext> _triviaCommentBuilder;
    private readonly TypeContextInfoBulder<TContext> _typeSyntaxBuilder;
    private readonly IContextCollector<TContext> _collector;

    public PropertyDeclarationParser(
        IContextCollector<TContext> collector,
        PropertyContextInfoBuilder<TContext> propertyContextInfoBuilder,
        CommentSyntaxTriviaContentInfoBuilder<TContext> triviaCommentBuilder,
        TypeContextInfoBulder<TContext> typeSyntaxBuilder,
        OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
        _propertyContextInfoBuilder = propertyContextInfoBuilder;
        _triviaCommentBuilder = triviaCommentBuilder;
        _typeSyntaxBuilder = typeSyntaxBuilder;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is PropertyDeclarationSyntax;

    public override void Parse(MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if (syntax is not PropertyDeclarationSyntax propertySyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not PropertyDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parse property syntax: {propertySyntax.Identifier.Text}", LogLevelNode.Start);

        //1.Создание контекста для самого свойства
        var propertyContext = _propertyContextInfoBuilder.BuildContextInfoForProperty(propertySyntax, model);
        if (propertyContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for property.", LogLevelNode.End);
            return;
        }

        // 2. Парсинг комментариев
        _triviaCommentBuilder.Parse(propertySyntax, propertyContext);

        // 3. Обработка типа свойства (рекурсивный обход)
        var propertyTypeSyntax = propertySyntax.Type;
        if (propertyTypeSyntax != null)
        {
            var typeSymbol = model.GetTypeInfo(propertyTypeSyntax).Type;

            // Проверяем, что это не базовый тип (например, int, string, object)
            if (typeSymbol != null && typeSymbol.Locations.Any())
            {
                // Проверяем, существует ли уже контекст для этого типа, чтобы избежать рекурсии
                var cnt = _collector.BySymbolDisplayName.GetValueOrDefault(typeSymbol.ToDisplayString());
                if (cnt == null)
                {
                    // Получаем синтаксический узел из семантического символа
                    var typeDeclarationSyntax = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

                    // Если это TypeDeclarationSyntax (класс, struct и т.д.), парсим его
                    if (typeDeclarationSyntax is TypeDeclarationSyntax tds)
                    {
                        // Рекурсивный вызов парсера типа
                        _typeSyntaxBuilder.BuildContextInfoForType(tds, model, string.Empty, null);
                    }
                }
            }
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Finished parsing PropertyDeclarationSyntax", LogLevelNode.End);
    }
}