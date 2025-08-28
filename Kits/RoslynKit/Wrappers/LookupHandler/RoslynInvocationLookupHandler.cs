using System.Text.RegularExpressions;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations.Lookup;
using RoslynKit.Wrappers.Syntax;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Wrappers.LookupHandler;

public class RoslynInvocationLookupHandler<TContext, TSemanticModel> : SymbolLookupHandler<TContext, TSemanticModel>
    where TContext : ContextInfo, IContextWithReferences<TContext>
    where TSemanticModel : class, ISemanticModelWrapper

{
    private readonly SemanticOptions _options;
    private readonly IContextCollector<TContext> _collector;
    private readonly CSharpMethodContextInfoBuilder<TContext> _methodContextInfoBuilder;
    private readonly CSharpTypeContextInfoBulder<TContext> _typeContextInfoBuilder;

    public RoslynInvocationLookupHandler(IContextCollector<TContext> collector, OnWriteLog? onWriteLog, SemanticOptions options, CSharpTypeContextInfoBulder<TContext> typeContextInfoBuilder, CSharpMethodContextInfoBuilder<TContext> methodContextInfoBuilder) : base(onWriteLog)
    {
        _options = options;
        _typeContextInfoBuilder = typeContextInfoBuilder;
        _methodContextInfoBuilder = methodContextInfoBuilder;
        _collector = collector;
    }

    public override TContext? Handle(BaseSyntaxWrapper symbolDto)
    {
        if (!_options.CreateFailedCallees)
        {
            // Если создание искусственных узлов не разрешено, возвращаем null

            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Warn, $"[MISS] Fallback fake callee be not used for {symbolDto.FullName}, because of disabled option CreateFailedCallees");
            return base.Handle(symbolDto);
        }

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"[CREATE] Fallback fake callee created for: {symbolDto.FullName}");

        // Логика создания искусственного узла

        var nameSpace = symbolDto.Namespace;
        var className = SignatureUtils.ExtractClassName(symbolDto.FullName, nameSpace);
        var fullname = symbolDto.FullName;

        var typeModel = new CSharpTypeSyntaxWrapper(identifier: symbolDto.Identifier, name: className, fullName: fullname, symbolDto.SpanStart, symbolDto.SpanEnd, nameSpace);
        var typeContext = _typeContextInfoBuilder.BuildContextInfo(ownerContext: default, typeModel);
        if (typeContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Err, $"[FAIL] Typecontext not found for: {symbolDto.FullName}");
            return default;
        }

        _collector.Append(typeContext);

        var methodmodel = new CSharpMethodSyntaxWrapper(wrapper: symbolDto);
        var methodContext = _methodContextInfoBuilder.BuildInvocationContextInfo(ownerContext: typeContext, methodmodel);

        if (methodContext == null)
        {
            _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Err, $"[FAIL] Methodcontext not found for: {symbolDto.FullName}");
            return default;
        }

        _onWriteLog?.Invoke(AppLevel.R_Inv, LogLevel.Dbg, $"[DONE] Fallback fake callee created for: {symbolDto.FullName}");
        _collector.Append(methodContext);

        return methodContext;
    }
}

internal static class SignatureUtils
{

    public static string ExtractClassName(string input, string methodNamespace)
    {
        string pattern = $@"(?:^|\s+){Regex.Escape(methodNamespace)}\.([a-zA-Z0-9_]+)\.[a-zA-Z0-9_]+(?:<[^>]+>)?\(";

        Match match = Regex.Match(input, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;
    }
}