namespace ContextBrowser.ContextCommentsParser;

public interface IContextClassifier
{
    bool IsVerb(string theWord);

    bool IsNoun(string theWord);
}