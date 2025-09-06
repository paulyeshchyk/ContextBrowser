using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Model;


public class ContextKeyContainer : BaseKeyAndDataContainer<IContextKey>
{
    public ContextKeyContainer(IContextKey contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}

public class NamespacenameContainer : BaseKeyAndDataContainer<string>
{
    public NamespacenameContainer(string contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}


public class EntitynameContainer : BaseKeyAndDataContainer<string>
{
    public EntitynameContainer(string contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}