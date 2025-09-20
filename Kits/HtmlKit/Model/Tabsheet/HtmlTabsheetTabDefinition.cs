using System;
using TensorKit.Model.DomainPerAction;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabDefinition
{
    public string ButtonText { get; init; }

    public string TabId { get; init; }

    public Func<ContextKeyContainer<DomainPerActionTensor>, string> ContentGenerator { get; init; } // Returns a string of HTML

    public bool IsActive { get; init; }

#warning remove domainperaction
    public HtmlTabsheetTabDefinition(string buttonText, string tabId, Func<ContextKeyContainer<DomainPerActionTensor>, string> contentGenerator, bool isActive)
    {
        ButtonText = buttonText;
        TabId = tabId;
        ContentGenerator = contentGenerator;
        IsActive = isActive;
    }
}