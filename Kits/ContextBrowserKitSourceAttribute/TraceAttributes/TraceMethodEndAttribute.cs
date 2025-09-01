using System;
using System.Linq;
using System.Text;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContextBrowserKitSourceAttribute.TraceGenerator
{
    /// <summary>
    /// This attribute wraps the method code with tracing calls before and after the method execution.
    /// It requires 5 arguments: logger, appLevel, logLevel, start text and an end text for tracing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TraceMethodEndAttribute : Attribute
    {
        public TraceMethodEndAttribute(object logger, int appLevel, int logLevel, string executionStartText, string executionEndText = "")
        {
            Logger = logger;
            AppLevel = appLevel;
            LogLevel = logLevel;
            ExecutionStartText = executionStartText;
            ExecutionEndText = executionEndText;
        }

        public object Logger { get; }
        public int AppLevel { get; }
        public int LogLevel { get; }
        public string ExecutionStartText { get; }
        public string ExecutionEndText { get; }
    }

    /// <summary>
    /// This attribute adds a tracing call at the beginning of the method.
    /// It requires 4 arguments: logger, appLevel, logLevel and an execution text for tracing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TraceMethodStartAttribute : Attribute
    {
        public TraceMethodStartAttribute(object logger, int appLevel, int logLevel, string executionText)
        {
            Logger = logger;
            AppLevel = appLevel;
            LogLevel = logLevel;
            ExecutionText = executionText;
        }

        public object Logger { get; }
        public int AppLevel { get; }
        public int LogLevel { get; }
        public string ExecutionText { get; }
    }
}