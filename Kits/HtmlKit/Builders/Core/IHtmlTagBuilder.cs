using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlKit.Builders.Core;

// pattern: Template method
public interface IHtmlTagBuilder
{
    void Start(TextWriter sb, IHtmlTagAttributes? attrs = null);

    void End(TextWriter sb);
}
