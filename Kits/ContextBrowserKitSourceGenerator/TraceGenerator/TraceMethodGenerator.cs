using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowserKitSourceGenerator.TraceGenerator
{
    [Generator]
    public class TraceMethodGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Provider to find methods with TraceMethodEndAttribute
            var traceEndMethods = context.SyntaxProvider.ForAttributeWithMetadataName(
                "CompileTraceAttributes.TraceMethodEndAttribute",
                predicate: (node, _) => node is MethodDeclarationSyntax,
                transform: (ctx, token) => new
                {
                    MethodDeclaration = (MethodDeclarationSyntax)ctx.TargetNode,
                    AttributeData = ctx.Attributes.First(),
                    MethodSymbol = ctx.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ctx.TargetNode, token)
                });

            // Provider to find methods with TraceMethodStartAttribute
            var traceStartMethods = context.SyntaxProvider.ForAttributeWithMetadataName(
                "CompileTraceAttributes.TraceMethodStartAttribute",
                predicate: (node, _) => node is MethodDeclarationSyntax,
                transform: (ctx, token) => new
                {
                    MethodDeclaration = (MethodDeclarationSyntax)ctx.TargetNode,
                    AttributeData = ctx.Attributes.First(),
                    MethodSymbol = ctx.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ctx.TargetNode, token)
                });

            // Register source output for TraceMethodEndAttribute
            context.RegisterSourceOutput(traceEndMethods, (sourceContext, item) =>
            {
                var method = item.MethodDeclaration;
                var attributeData = item.AttributeData;
                var methodSymbol = item.MethodSymbol;

                if (method.Body == null || methodSymbol == null)
                {
                    return;
                }

                var newBody = GenerateTraceEndCode(method, attributeData, methodSymbol);

                var newMethodDeclaration = method.WithBody(null).WithExpressionBody(null).WithSemicolonToken(default)
                    .WithLeadingTrivia(method.GetLeadingTrivia())
                    .WithTrailingTrivia(method.GetTrailingTrivia())
                    .WithBody(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseStatement(newBody) as BlockSyntax);

                var newClass = method.Parent as ClassDeclarationSyntax;
                var newClassDeclaration = newClass.ReplaceNode(method, newMethodDeclaration);

                var newNamespace = newClass?.Parent as BaseNamespaceDeclarationSyntax;
                SyntaxNode sourceTree;
                if (newNamespace != null)
                {
                    sourceTree = newNamespace.ReplaceNode(newClass, newClassDeclaration);
                }
                else
                {
                    sourceTree = newClassDeclaration;
                }

                sourceContext.AddSource($"{method.Identifier.Text}.g.cs", sourceTree.ToFullString());
            });

            // Register source output for TraceMethodStartAttribute
            context.RegisterSourceOutput(traceStartMethods, (sourceContext, item) =>
            {
                var method = item.MethodDeclaration;
                var attributeData = item.AttributeData;

                if (method.Body == null)
                {
                    return;
                }

                var newBody = GenerateTraceStartCode(method, attributeData);

                var newMethodDeclaration = method.WithBody(null).WithExpressionBody(null).WithSemicolonToken(default)
                    .WithLeadingTrivia(method.GetLeadingTrivia())
                    .WithTrailingTrivia(method.GetTrailingTrivia())
                    .WithBody(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseStatement(newBody) as BlockSyntax);

                var newClass = method.Parent as ClassDeclarationSyntax;
                var newClassDeclaration = newClass.ReplaceNode(method, newMethodDeclaration);

                var newNamespace = newClass?.Parent as BaseNamespaceDeclarationSyntax;
                SyntaxNode sourceTree;
                if (newNamespace != null)
                {
                    sourceTree = newNamespace.ReplaceNode(newClass, newClassDeclaration);
                }
                else
                {
                    sourceTree = newClassDeclaration;
                }

                sourceContext.AddSource($"{method.Identifier.Text}.g.cs", sourceTree.ToFullString());
            });
        }

        /// <summary>
        /// Generates the code for a method decorated with TraceMethodEndAttribute.
        /// </summary>
        /// <param name="method">The method syntax node.</param>
        /// <param name="attributeData">The attribute data.</param>
        /// <param name="methodSymbol">The method's semantic symbol.</param>
        /// <returns>The generated method body as a string.</returns>
        private static string GenerateTraceEndCode(MethodDeclarationSyntax method, AttributeData attributeData, IMethodSymbol methodSymbol)
        {
            var logger = attributeData.ConstructorArguments[0].Value.ToString();
            var appLevel = attributeData.ConstructorArguments[1].ToCSharpString();
            var logLevel = attributeData.ConstructorArguments[2].ToCSharpString();
            var executionStartText = attributeData.ConstructorArguments[3].ToCSharpString();
            var executionEndText = attributeData.ConstructorArguments.Length > 4
                ? attributeData.ConstructorArguments[4].ToCSharpString()
                : "\"\"";

            if (methodSymbol.ReturnsVoid == false)
            {
                var returnType = methodSymbol.ReturnType.ToDisplayString();
                return $@"
                    {{
                        {logger}?.Invoke({appLevel}, {logLevel}, {executionStartText}, LogLevelNode.Start);
                        {returnType} result = default;
                        try
                        {{
                            {method.Body.ToString().TrimStart('{').TrimEnd('}').Trim()}
                        }}
                        finally
                        {{
                            {logger}?.Invoke({appLevel}, {logLevel}, {executionEndText}, LogLevelNode.End);
                        }}
                        return result;
                    }}";
            }
            else
            {
                return $@"
                    {{
                        {logger}?.Invoke({appLevel}, {logLevel}, {executionStartText}, LogLevelNode.Start);
                        try
                        {{
                            {method.Body.ToString().TrimStart('{').TrimEnd('}').Trim()}
                        }}
                        finally
                        {{
                            {logger}?.Invoke({appLevel}, {logLevel}, {executionEndText}, LogLevelNode.End);
                        }}
                    }}";
            }
        }

        /// <summary>
        /// Generates the code for a method decorated with TraceMethodStartAttribute.
        /// </summary>
        /// <param name="method">The method syntax node.</param>
        /// <param name="attributeData">The attribute data.</param>
        /// <returns>The generated method body as a string.</returns>
        private static string GenerateTraceStartCode(MethodDeclarationSyntax method, AttributeData attributeData)
        {
            var logger = attributeData.ConstructorArguments[0].Value.ToString();
            var appLevel = attributeData.ConstructorArguments[1].ToCSharpString();
            var logLevel = attributeData.ConstructorArguments[2].ToCSharpString();
            var executionText = attributeData.ConstructorArguments[3].ToCSharpString();

            return $@"
                {{
                    {logger}?.Invoke({appLevel}, {logLevel}, {executionText}, LogLevelNode.Start);
                    {method.Body.ToString().TrimStart('{').TrimEnd('}').Trim()}
                }}";
        }
    }

    internal class TraceMethodGeneratorLegacy : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Provider to find methods with TraceMethodEndAttribute
            var traceEndMethods = context.SyntaxProvider.ForAttributeWithMetadataName(
                "CompileTraceAttributes.TraceMethodEndAttribute",
                predicate: (node, _) => node is MethodDeclarationSyntax,
                transform: (ctx, token) => new
                {
                    MethodDeclaration = (MethodDeclarationSyntax)ctx.TargetNode,
                    AttributeData = ctx.Attributes.First(),
                    MethodSymbol = ctx.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ctx.TargetNode, token)
                });

            // Provider to find methods with TraceMethodStartAttribute
            var traceStartMethods = context.SyntaxProvider.ForAttributeWithMetadataName(
                "CompileTraceAttributes.TraceMethodStartAttribute",
                predicate: (node, _) => node is MethodDeclarationSyntax,
                transform: (ctx, token) => new
                {
                    MethodDeclaration = (MethodDeclarationSyntax)ctx.TargetNode,
                    AttributeData = ctx.Attributes.First(),
                    MethodSymbol = ctx.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ctx.TargetNode, token)
                });

            // Register source output for TraceMethodEndAttribute
            context.RegisterSourceOutput(traceEndMethods, (sourceContext, item) =>
            {
                var method = item.MethodDeclaration;
                var attributeData = item.AttributeData;
                var methodSymbol = item.MethodSymbol;

                if (method.Body == null || methodSymbol == null)
                {
                    return;
                }

                var newBodyBuilder = new StringBuilder();
                newBodyBuilder.Append("{");

                var logger = attributeData.ConstructorArguments[0].Value.ToString();
                var appLevel = attributeData.ConstructorArguments[1].ToCSharpString();
                var logLevel = attributeData.ConstructorArguments[2].ToCSharpString();
                var executionStartText = attributeData.ConstructorArguments[3].ToCSharpString();
                var executionEndText = attributeData.ConstructorArguments.Length > 4
                    ? attributeData.ConstructorArguments[4].ToCSharpString()
                    : "\"\"";

                newBodyBuilder.AppendLine($"\n\t\t{logger}?.WriteLog({appLevel}, {logLevel}, {executionStartText}, LogLevelNode.Start);");

                if (methodSymbol.ReturnsVoid == false)
                {
                    var returnType = methodSymbol.ReturnType.ToDisplayString();
                    newBodyBuilder.AppendLine($"\t\t{returnType} result = default;");
                    newBodyBuilder.AppendLine("\t\ttry");
                    newBodyBuilder.AppendLine("\t\t{");
                    newBodyBuilder.AppendLine($"\t\t\t{method.Body.ToString().TrimStart('{').TrimEnd('}').Trim()}");
                    newBodyBuilder.AppendLine("\t\t}");
                    newBodyBuilder.AppendLine("\t\tfinally");
                    newBodyBuilder.AppendLine("\t\t{");
                    newBodyBuilder.AppendLine($"\t\t\t{logger}?.WriteLog({appLevel}, {logLevel}, {executionEndText}, LogLevelNode.End);");
                    newBodyBuilder.AppendLine("\t\t}");
                    newBodyBuilder.AppendLine("\t\treturn result;");
                }
                else
                {
                    newBodyBuilder.AppendLine("\t\ttry");
                    newBodyBuilder.AppendLine("\t\t{");
                    newBodyBuilder.AppendLine($"\t\t\t{method.Body.ToString().TrimStart('{').TrimEnd('}').Trim()}");
                    newBodyBuilder.AppendLine("\t\t}");
                    newBodyBuilder.AppendLine("\t\tfinally");
                    newBodyBuilder.AppendLine("\t\t{");
                    newBodyBuilder.AppendLine($"\t\t\t{logger}?.WriteLog({appLevel}, {logLevel}, {executionEndText}, LogLevelNode.End);");
                    newBodyBuilder.AppendLine("\t\t}");
                }

                newBodyBuilder.Append("}");

                var newMethodDeclaration = method.WithBody(null).WithExpressionBody(null).WithSemicolonToken(default)
                    .WithLeadingTrivia(method.GetLeadingTrivia())
                    .WithTrailingTrivia(method.GetTrailingTrivia())
                    .WithBody(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseStatement(newBodyBuilder.ToString()) as BlockSyntax);

                var newClass = method.Parent as ClassDeclarationSyntax;
                var newClassDeclaration = newClass.ReplaceNode(method, newMethodDeclaration);

                var newNamespace = newClass?.Parent as BaseNamespaceDeclarationSyntax;
                SyntaxNode sourceTree;
                if (newNamespace != null)
                {
                    sourceTree = newNamespace.ReplaceNode(newClass, newClassDeclaration);
                }
                else
                {
                    sourceTree = newClassDeclaration;
                }

                sourceContext.AddSource($"{method.Identifier.Text}.g.cs", sourceTree.ToFullString());
            });

            // Register source output for TraceMethodStartAttribute
            context.RegisterSourceOutput(traceStartMethods, (sourceContext, item) =>
            {
                var method = item.MethodDeclaration;
                var attributeData = item.AttributeData;

                if (method.Body == null)
                {
                    return;
                }

                var newBodyBuilder = new StringBuilder();
                newBodyBuilder.Append("{");

                var logger = attributeData.ConstructorArguments[0].Value.ToString();
                var appLevel = attributeData.ConstructorArguments[1].ToCSharpString();
                var logLevel = attributeData.ConstructorArguments[2].ToCSharpString();
                var executionText = attributeData.ConstructorArguments[3].ToCSharpString();

                newBodyBuilder.AppendLine($"\n\t\t{logger}?.WriteLog({appLevel}, {logLevel}, {executionText});");
                newBodyBuilder.AppendLine($"\t\t{method.Body}");
                newBodyBuilder.Append("}");

                var newMethodDeclaration = method.WithBody(null).WithExpressionBody(null).WithSemicolonToken(default)
                    .WithLeadingTrivia(method.GetLeadingTrivia())
                    .WithTrailingTrivia(method.GetTrailingTrivia())
                    .WithBody(Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseStatement(newBodyBuilder.ToString()) as BlockSyntax);

                var newClass = method.Parent as ClassDeclarationSyntax;
                var newClassDeclaration = newClass.ReplaceNode(method, newMethodDeclaration);

                var newNamespace = newClass?.Parent as BaseNamespaceDeclarationSyntax;
                SyntaxNode sourceTree;
                if (newNamespace != null)
                {
                    sourceTree = newNamespace.ReplaceNode(newClass, newClassDeclaration);
                }
                else
                {
                    sourceTree = newClassDeclaration;
                }

                sourceContext.AddSource($"{method.Identifier.Text}.g.cs", sourceTree.ToFullString());
            });
        }
    }
}