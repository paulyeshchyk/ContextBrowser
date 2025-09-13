using System;
using ContextBrowserKit.Factories;
using ContextBrowserKit.Options;
using HtmlKit.Options;

namespace ContextKit.Model;

public record ContextKey : IContextKey
{
    public string Action { get; set; }

    public string Domain { get; set; }

    public ContextKey(string Action, string Domain)
    {
        this.Action = Action;
        this.Domain = Domain;
    }
}

public class ContextKeyBuilder : IContextKeyBuilder
{
    public TKey BuildContextKey<TKey>(MatrixOrientationType orientationType, string row, string col, Func<string, string, TKey> createKey)
    {
        return orientationType == MatrixOrientationType.ActionRows
            ? createKey(row, col)
            : createKey(col, row);
    }
}


public record DimensionKey
{
    public string TheCol { get; set; }

    public string TheRow { get; set; }

    public DimensionKey(string col, string row)
    {
        this.TheCol = col;
        this.TheRow = row;
    }
}
