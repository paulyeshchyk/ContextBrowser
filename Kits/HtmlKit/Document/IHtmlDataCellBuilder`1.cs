
using System.IO;
using ContextKit.Model;

namespace HtmlKit.Document;

public interface IHtmlDataCellBuilder<TKey>
    where TKey : ContextKey
{
    void BuildDataCell(TextWriter textWriter, TKey cell);
}
