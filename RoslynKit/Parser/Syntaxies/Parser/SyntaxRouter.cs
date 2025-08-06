using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynKit.Parser.Syntaxies.Parser;

public class SyntaxRouter<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly OnWriteLog? _onWriteLog;
    private readonly IEnumerable<ISyntaxParser<TContext>> _parsers;

    public SyntaxRouter(OnWriteLog? onWriteLog, IEnumerable<ISyntaxParser<TContext>> parsers)
    {
        _onWriteLog = onWriteLog;
        _parsers = parsers;
    }

    public void Route(IEnumerable<MemberDeclarationSyntax> availableSyntaxies, SemanticModel model)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Routing {availableSyntaxies.Count()} syntaxes", LogLevelNode.Start);

        foreach(var syntax in availableSyntaxies)
        {
            var parser = _parsers.FirstOrDefault(p => p.CanParse(syntax));

            if(parser == null)
            {
                _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Err, $"[MISS]: No parser found for syntax type: {syntax.GetType()}");
                continue;
            }

            parser.Parse(syntax, model);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
