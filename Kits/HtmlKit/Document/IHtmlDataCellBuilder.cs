using System;
using System.IO;
using ContextKit.Model;
using HtmlKit.Options;

namespace HtmlKit.Document;

public interface IHtmlDataCellBuilder<TKey>
{
    void BuildDataCell(TextWriter textWriter, TKey key, HtmlTableOptions options);
}
