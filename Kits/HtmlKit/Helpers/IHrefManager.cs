using System;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Options;
using TensorKit.Model;

namespace HtmlKit.Helpers;

public interface IHrefManager
{
    string GetHrefCell(DomainPerActionTensor cell, HtmlTableOptions _options);

    string GetHrefColSummary(string key, HtmlTableOptions _options);

    string GetHRefRow(string key, HtmlTableOptions _options);

    string GetHRefRowHeader(string key, HtmlTableOptions _options);

    string GetHrefRowHeaderSummary(HtmlTableOptions _options);

    string GetHRefRowMeta(string key, HtmlTableOptions _options);

    string GetHrefRowSummary(string key, HtmlTableOptions _options);

    string GetHrefSummary(HtmlTableOptions _options);

    string GetHrefColHeaderSummary(HtmlTableOptions _options);

    string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options);
}
