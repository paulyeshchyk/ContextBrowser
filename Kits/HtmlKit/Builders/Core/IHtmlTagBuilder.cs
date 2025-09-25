using System.IO;

namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlTagBuilder
{
    void Start(TextWriter sb, IHtmlTagAttributes? attrs = null);

    void End(TextWriter sb);
}
