using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Container;

namespace HtmlKit.Model.Containers;

public class ContextInfoKeyContainerEntityName : ContextInfoKeyContainerBase<string>
{
    public ContextInfoKeyContainerEntityName(string contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}
