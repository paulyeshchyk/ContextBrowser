using System.Collections.Generic;
using System.IO;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Page;
using TensorKit.Model;

namespace HtmlKit.Writer;

public interface IHtmlContentInjector<TKey>
    where TKey : notnull
{
    string Inject(TKey container, int cnt);
}
