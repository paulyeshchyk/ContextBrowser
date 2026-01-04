using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model.Meta;
using RoslynKit.Wrappers.Meta;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Assembly;

public interface ICompilationMapMapper<TSyntaxTreeWrapper>
where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    SemanticCompilationMap MapSemanticModelToCompilationMap(IEnumerable<TSyntaxTreeWrapper> syntaxTrees, ICompilationWrapper compilation);
    CompilationMap MapSemanticModelToSingleMap(ICompilationWrapper compilation, TSyntaxTreeWrapper tree);
}
