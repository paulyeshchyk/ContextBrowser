using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.Parser;

public class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    private IContextClassifier _contextClassifier { get; }

    public ContextInfoCommentProcessor(IContextClassifier contextClassifier)
    {
        _contextClassifier = contextClassifier;
    }

    public void Process(string comment, T target)
    {
        if(comment.StartsWith("context:", StringComparison.OrdinalIgnoreCase))
        {
            var tags = comment.Substring("context:".Length)
                              .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => t.Trim().ToLowerInvariant());

            var actions = tags.Where(t => _contextClassifier.IsVerb(t)).ToList();
            target.Action = string.Join(";", actions);

            var domains = tags.Where(t => _contextClassifier.IsNoun(t)).ToList();
            foreach(var domain in domains)
            {
                target.Domains.Add(domain);
            }
            foreach(var tag in tags)
                target.Contexts.Add(tag);
        }


        if(comment.StartsWith("coverage:", StringComparison.OrdinalIgnoreCase))
        {
            var val = comment.Substring("coverage:".Length).Trim();
            target.Dimensions["coverage"] = val;
        }
    }
}
