using System;
using TensorKit.Model;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabDefinition
{
    public string ButtonText { get; init; }

    public string TabId { get; init; }

    public Func<ContextKeyContainer<TensorBase<string>>, string> ContentGenerator { get; init; } // Returns a string of HTML

    public bool IsActive { get; init; }

    public HtmlTabsheetTabDefinition(string buttonText, string tabId, Func<ContextKeyContainer<TensorBase<string>>, string> contentGenerator, bool isActive)
    {
        ButtonText = buttonText;
        TabId = tabId;
        ContentGenerator = contentGenerator;
        IsActive = isActive;
    }
}