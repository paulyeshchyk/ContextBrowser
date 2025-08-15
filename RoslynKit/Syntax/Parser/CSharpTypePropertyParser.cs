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
    private readonly CSharpPropertyContextInfoBuilder<TContext> _propertyContextInfoBuilder;
    private readonly CSharpCommentTriviaSyntaxParser<TContext> _triviaCommentParser;
    private readonly CSharpRecordContextInfoBuilder<TContext> _recordSyntaxBuilder;
    private readonly CSharpTypeContextInfoBulder<TContext> _typeSyntaxBuilder;
    private readonly CSharpEnumContextInfoBuilder<TContext> _enumSyntaxBuilder;
    private readonly CSharpInterfaceContextInfoBuilder<TContext> _interfaceSyntaxBuilder;
    private readonly IContextCollector<TContext> _collector;

    public CSharpTypePropertyParser(
        IContextCollector<TContext> collector,
        CSharpPropertyContextInfoBuilder<TContext> propertyContextInfoBuilder,
        CSharpCommentTriviaSyntaxParser<TContext> triviaCommentParser,
        CSharpTypeContextInfoBulder<TContext> typeSyntaxBuilder,
        CSharpRecordContextInfoBuilder<TContext> recordSyntaxBuilder,
        CSharpEnumContextInfoBuilder<TContext> enumSyntaxBuilder,
        CSharpInterfaceContextInfoBuilder<TContext> interfaceSyntaxBuilder,
        OnWriteLog? onWriteLog)
        : base(onWriteLog)
    {
        _collector = collector;
        _propertyContextInfoBuilder = propertyContextInfoBuilder;
        _triviaCommentParser = triviaCommentParser;
        _typeSyntaxBuilder = typeSyntaxBuilder;
        _recordSyntaxBuilder = recordSyntaxBuilder;
        _enumSyntaxBuilder = enumSyntaxBuilder;
        _interfaceSyntaxBuilder = interfaceSyntaxBuilder;
    }

    public override bool CanParse(MemberDeclarationSyntax syntax) => syntax is PropertyDeclarationSyntax;

    public override void Parse(TContext? parent, MemberDeclarationSyntax syntax, SemanticModel model)
    {
        if(syntax is not PropertyDeclarationSyntax propertySyntax)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax is not PropertyDeclarationSyntax");
            return;
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - property syntax");

        //1.Создание контекста для самого свойства
        var propertyContext = _propertyContextInfoBuilder.BuildContextInfo(parent, propertySyntax, model);
        if(propertyContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, "Failed to build context for property.", LogLevelNode.End);
            return;
        }


        parent?.Properties.Add(propertyContext);
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"{parent?.Name}.{propertyContext.Name} added");


        // 2. Парсинг комментариев
        _triviaCommentParser.Parse(propertyContext, propertySyntax, model);

        // 3. Обработка типа свойства (рекурсивный обход)
        var propertyTypeSyntax = propertySyntax.Type;
        if(propertyTypeSyntax == null)
        {
            return;
        }

        var typeSymbol = model.GetTypeInfo(propertyTypeSyntax).Type;

        // Проверяем, что это не базовый тип (например, int, string, object)
        if(!(typeSymbol != null && typeSymbol.Locations.Any()))
        {
            return;
        }
        // Проверяем, существует ли уже контекст для этого типа, чтобы избежать рекурсии
        var cnt = _collector.BySymbolDisplayName.GetValueOrDefault(typeSymbol.ToDisplayString());
        if(cnt != null)
        {
            return;
        }

        var declarationSyntax = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

        if(declarationSyntax == null)
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[{typeSymbol}] Symbol has no declared syntax");
            return;
        }

        if(declarationSyntax is ClassDeclarationSyntax tds)
        {
            _typeSyntaxBuilder.BuildContextInfo(default, tds, model);
        }
        else if(declarationSyntax is RecordDeclarationSyntax rds)
        {
            _recordSyntaxBuilder.BuildContextInfo(default, rds, model);
        }
        else if(declarationSyntax is EnumDeclarationSyntax eds)
        {
            _enumSyntaxBuilder.BuildContextInfo(default, eds, model);
        }
        else if(declarationSyntax is InterfaceDeclarationSyntax ids)
        {
            _interfaceSyntaxBuilder.BuildContextInfo(default, ids, model);
        }
        else if(declarationSyntax is TypeParameterSyntax tps)
        {
            //skip
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"Syntax was not parsed: {declarationSyntax} ");
        }
    }
}