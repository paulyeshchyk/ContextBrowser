using System;

namespace HtmlKit.Model.Tabsheet;

public class HtmlTabsheetTabDefinition
{
    public string ButtonText { get; init; }
    public string TabId { get; init; }
    public Func<ContextKeyContainer, string> ContentGenerator { get; init; } // Returns a string of HTML
    public bool IsActive { get; init; }

    public HtmlTabsheetTabDefinition(string buttonText, string tabId, Func<ContextKeyContainer, string> contentGenerator, bool isActive)
    {
        ButtonText = buttonText;
        TabId = tabId;
        ContentGenerator = contentGenerator;
        IsActive = isActive;
    }
}