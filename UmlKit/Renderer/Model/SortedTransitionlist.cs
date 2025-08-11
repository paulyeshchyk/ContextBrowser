namespace UmlKit.Renderer.Model;

public class SortedTransitionlist : SortedList<int, UmlTransitionDto>
{
    public SortedTransitionlist()
    {
    }

    public SortedTransitionlist(SortedTransitionlist source) : base(source)
    {
    }
}
