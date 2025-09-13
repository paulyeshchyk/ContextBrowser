using System.Collections.Generic;
using System.IO;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Page;

namespace HtmlKit.Writer;

public interface IHtmlContentInjector
{
    string Inject(IContextKey container, int cnt);
}
