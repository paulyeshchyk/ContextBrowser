namespace CB.ContextCommentsParser;

public interface IContextClassifier
{
    bool IsVerb(string theWord);

    bool IsNoun(string theWord);
}