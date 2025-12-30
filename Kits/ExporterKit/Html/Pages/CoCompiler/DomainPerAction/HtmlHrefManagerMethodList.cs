using System;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using HtmlKit.Helpers;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlHrefManagerMethodList<TDataTensor> : IHtmlHrefManager<MethodListTensor<TDataTensor>>
    where TDataTensor : notnull
{
    private static readonly long TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private readonly INamingProcessor _namingProcessor;

    public HtmlHrefManagerMethodList(INamingProcessor namingProcessor)
    {
        _namingProcessor = namingProcessor;
    }


    public string GetHrefCell(MethodListTensor<TDataTensor> cell, HtmlTableOptions _options)
    {
        var htmlFilename = _namingProcessor.ClassOnlyHtmlFilename(cell.DomainPerActionTensorContainer.ContextKey.ToString());
        return $"{htmlFilename}?v={TimeStamp}";
    }

    public string GetHrefColHeaderSummary(HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefColSummary(ILabeledValue key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHRefRow(ILabeledValue key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHRefRowHeader(ILabeledValue key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefRowHeaderSummary(HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHRefRowMeta(ILabeledValue key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefRowSummary(ILabeledValue key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefSummary(HtmlTableOptions _options)
    {
        return string.Empty;
    }
}
