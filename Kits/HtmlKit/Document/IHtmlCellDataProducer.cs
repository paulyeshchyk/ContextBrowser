using ContextKit.Model;

namespace HtmlKit.Document;

public interface IHtmlCellDataProducer
{
    string ProduceData(IContextKey container);
}
