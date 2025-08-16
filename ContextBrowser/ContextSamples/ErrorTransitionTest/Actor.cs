namespace ContextBrowser.ContextSamples.ErrorTransitionTest;

// context: TestTra, model
internal static class Actor
{
    // context: TestTra, model
    public static void Action1()
    {
        Transition.Add();
    }

    // context: TestTra, model
    public static void Action2()
    {
        Transition.Remove();
    }
}
