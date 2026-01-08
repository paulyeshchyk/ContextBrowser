using System;
using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Container;

namespace HtmlKit.Model.Containers;

public class ContextInfoKeyContainerTensor<TTensor> : ContextInfoKeyContainerBase<TTensor>
    where TTensor : notnull
{
    public ContextInfoKeyContainerTensor(TTensor contextKey, IEnumerable<IContextInfo> contextInfoList) : base(contextKey, contextInfoList)
    {
    }
}
