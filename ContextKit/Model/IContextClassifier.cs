namespace ContextKit.Model;

public interface IContextClassifier
{
    bool IsVerb(string theWord);

    bool IsNoun(string theWord);
}