using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

// context: syntax, build, roslyn
public class CSharpSyntaxParserTypeProperty<TContext> : SyntaxParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly CSharpSyntaxParserCommentTrivia<TContext> _triviaCommentParser;
    private readonly IContextCollector<TContext> _collector;
    private readonly ContextInfoBuilderDispatcher<TContext> _contextInfoBuilderDispatcher;

    public CSharpSyntaxParserTypeProperty(
        IContextCollector<TContext> collector,
        CSharpSyntaxParserCommentTrivia<TContext> triviaCommentParser,
        ContextInfoBuilderDispatcher<TContext> contextInfoBuilderDispatcher,
        IAppLogger<AppLevel> logger)
        : base(logger)
    {
        _collector = collector;
        _triviaCommentParser = triviaCommentParser;
        _contextInfoBuilderDispatcher = contextInfoBuilderDispatcher;
    }

    public override bool CanParseSyntax(object syntax) => syntax is PropertyDeclarationSyntax;

    public override async Task ParseAsync(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not PropertyDeclarationSyntax propertySyntax)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, "Syntax is not PropertyDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, "Parsing files: phase 1 - property syntax");

        //1.Создание контекста для самого свойства
        var propertyContext = await _contextInfoBuilderDispatcher.DispatchAndBuildAsync(parent, propertySyntax, model, cancellationToken).ConfigureAwait(false);
        if (propertyContext == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, "Failed to build context for property.", LogLevelNode.End);
            return;
        }

        parent?.Properties.Add(propertyContext);
        _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Dbg, $"{parent?.Name}.{propertyContext.Name} added");

        // 2. Парсинг комментариев
        await _triviaCommentParser.ParseAsync(propertyContext, propertySyntax, model, options, cancellationToken).ConfigureAwait(false);

        // 3. Обработка типа свойства (рекурсивный обход)
        var propertyTypeSyntax = propertySyntax.Type;
        var ts = model.GetTypeInfo(propertyTypeSyntax);
        if (ts is not ITypeSymbol typeSymbol)
        {
            throw new Exception("ITypeSymbol is expected");
        }

        // Проверяем, что это не базовый тип (например, int, string, object)
        if (!typeSymbol.Locations.Any())
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"[{typeSymbol}] Symbol has no locations");
            return;
        }

        // Проверяем, существует ли уже контекст для этого типа, чтобы избежать рекурсии
        var cnt = _collector.Collection.GetValueOrDefault(typeSymbol.BuildFullMemberName());
        if (cnt != null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"[{typeSymbol}] Context allready defined");
            return;
        }

        var declarationSyntax = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        if (!typeSymbol.IsImplicitlyDeclared && declarationSyntax == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Trace, $"[{typeSymbol}] Symbol is not implicitly declared");
            return;
        }

        if (declarationSyntax == null)
        {
            _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"[{typeSymbol}] Symbol is not implicitly declared, but has no declared syntax");
            return;
        }

        await _contextInfoBuilderDispatcher.DispatchAndBuildAsync(default, declarationSyntax, model, cancellationToken).ConfigureAwait(false);
    }
}