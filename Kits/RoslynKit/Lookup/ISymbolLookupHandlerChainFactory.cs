using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;
using Microsoft.CodeAnalysis;
using RoslynKit;
using RoslynKit.Assembly;
using RoslynKit.Assembly.Strategy.Invocation;
using RoslynKit.Lookup;
using RoslynKit.Model.SyntaxWrapper;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Lookup;

public class RoslynSymbolLookupHandlerChainFactory : ISymbolLookupHandlerChainFactory<ContextInfo, ISemanticModelWrapper>
{
    private readonly IEnumerable<ISymbolLookupHandler<ContextInfo, ISemanticModelWrapper>> _handlers;

    public RoslynSymbolLookupHandlerChainFactory(IEnumerable<ISymbolLookupHandler<ContextInfo, ISemanticModelWrapper>> handlers)
    {
        _handlers = handlers;
    }

    // Логика порядка находится ЗДЕСЬ
    public ISymbolLookupHandler<ContextInfo, ISemanticModelWrapper> BuildChain()
    {
        // 1. Получаем хэндлеры, используя LINQ или прямые ссылки

        var fullNameHandler = _handlers.OfType<RoslynSymbolLookupHandlerFullname<ContextInfo, ISemanticModelWrapper>>().First();
        var methodSymbolHandler = _handlers.OfType<RoslynSymbolLookupHandlerMethod<ContextInfo, ISemanticModelWrapper>>().First();
        var invocationHandler = _handlers.OfType<RoslynSymbolLookupHandlerInvocation<ContextInfo, ISemanticModelWrapper>>().First();

        // 2. Устанавливаем требуемый порядок
        // FullName -> MethodSymbol -> Invocation
        fullNameHandler
            .SetNext(methodSymbolHandler)
            .SetNext(invocationHandler);

        // 3. Возвращаем начало цепочки
        return fullNameHandler;
    }
}