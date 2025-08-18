namespace ContextBrowser.ContextSamples.ErrorTransitionTest;

// context: S1, model
internal static class Actor
{
    // context: S1, model
    public static void Action1()
    {
        Transition.Add();
    }

    // context: S1, model
    public static void Action2()
    {
        Transition.Remove();
    }
}
