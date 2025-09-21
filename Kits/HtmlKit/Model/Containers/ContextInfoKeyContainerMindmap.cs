using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Container;

namespace HtmlKit.Model.Containers;

public class ContextInfoKeyContainerMindmap : ContextInfoKeyContainerBase<string>
{
    public ContextInfoKeyContainerMindmap(string contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}