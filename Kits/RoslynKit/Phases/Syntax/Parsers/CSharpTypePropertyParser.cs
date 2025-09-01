using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using RoslynKit.Phases.ContextInfoBuilder;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Phases.Syntax.Parsers;

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

    public override bool CanParse(object syntax) => syntax is PropertyDeclarationSyntax;

    public override void Parse(TContext? parent, object syntax, ISemanticModelWrapper model, SemanticOptions options, CancellationToken cancellationToken)
    {
        if (syntax is not PropertyDeclarationSyntax propertySyntax)
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Err, $"Syntax is not PropertyDeclarationSyntax");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Dbg, $"Parsing files: phase 1 - property syntax");

        //1.Создание контекста для самого свойства
        var propertyContext = _propertyContextInfoBuilder.BuildContextInfo(parent, propertySyntax, model, cancellationToken);
        if (propertyContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Err, "Failed to build context for property.", LogLevelNode.End);
            return;
        }

        parent?.Properties.Add(propertyContext);
        _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Dbg, $"{parent?.Name}.{propertyContext.Name} added");

        // 2. Парсинг комментариев
        _triviaCommentParser.Parse(propertyContext, propertySyntax, model, options, cancellationToken);

        // 3. Обработка типа свойства (рекурсивный обход)
        var propertyTypeSyntax = propertySyntax.Type;
        if (propertyTypeSyntax == null)
        {
            return;
        }

        var ts = model.GetTypeInfo(propertyTypeSyntax);
        if (ts is not ITypeSymbol typeSymbol)
        {
            throw new Exception("ITypeSymbol is expected");
        }

        // Проверяем, что это не базовый тип (например, int, string, object)
        if (!(typeSymbol != null && typeSymbol.Locations.Any()))
        {
            return;
        }

        // Проверяем, существует ли уже контекст для этого типа, чтобы избежать рекурсии
        var cnt = _collector.BySymbolDisplayName.GetValueOrDefault(typeSymbol.GetFullMemberName(includeParams: true));
        if (cnt != null)
        {
            return;
        }

        var declarationSyntax = typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        if (!typeSymbol.IsImplicitlyDeclared && declarationSyntax == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Trace, $"[{typeSymbol}] Symbol is not implicitly declared");
            return;
        }

        if (declarationSyntax == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Err, $"[{typeSymbol}] Symbol is not implicitly declared, but has no declared syntax");
            return;
        }

        if (declarationSyntax is ClassDeclarationSyntax tds)
        {
            _typeSyntaxBuilder.BuildContextInfo(default, tds, model, cancellationToken);
        }
        else if (declarationSyntax is RecordDeclarationSyntax rds)
        {
            _recordSyntaxBuilder.BuildContextInfo(default, rds, model, cancellationToken);
        }
        else if (declarationSyntax is EnumDeclarationSyntax eds)
        {
            _enumSyntaxBuilder.BuildContextInfo(default, eds, model, cancellationToken);
        }
        else if (declarationSyntax is InterfaceDeclarationSyntax ids)
        {
            _interfaceSyntaxBuilder.BuildContextInfo(default, ids, model, cancellationToken);
        }
        else if (declarationSyntax is TypeParameterSyntax tps)
        {
            //skip
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.R_Cntx, LogLevel.Err, $"Syntax was not parsed: {declarationSyntax} ");
        }
    }
}