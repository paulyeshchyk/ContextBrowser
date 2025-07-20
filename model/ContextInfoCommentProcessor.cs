namespace ContextBrowser.model;

public interface IContextInfoCommentProcessor<T>
{
    void Process(string comment, T target);
}

internal class ContextInfoCommentProcessor<T> : IContextInfoCommentProcessor<T>
    where T : ContextInfo
{
    public void Process(string comment, T target)
    {
        if(comment.StartsWith("context:", StringComparison.OrdinalIgnoreCase))
        {
            var tags = comment.Substring("context:".Length)
                              .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => t.Trim().ToLowerInvariant());

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
