using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Container;

namespace HtmlKit.Model.Containers;

public class ContextInfoKeyContainerNamespace : ContextInfoKeyContainerBase<string>
{
    public ContextInfoKeyContainerNamespace(string contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}
