using System.Collections.Generic;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Model;

public class ContextKeyContainer<TTensor> : BaseKeyAndDataContainer<TTensor>
    where TTensor : notnull
{
    public ContextKeyContainer(TTensor contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
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

public class MindmapContainer : BaseKeyAndDataContainer<string>
{
    public MindmapContainer(string contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}