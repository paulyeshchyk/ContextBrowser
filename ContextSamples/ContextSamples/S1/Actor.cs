namespace ContextSamples.ContextSamples.S1;

// context: S1, model
internal static class Actor
{
    // context: S1, model
    public static void Action1(string actionName)
    {
        Transition.Add();
    }

    // context: S1, model
    public static bool Action2(string actionName)
    {
        return Transition.Remove();
    }
}
