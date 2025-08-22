namespace ContextBrowserTests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    [DataRow("// context: Test1", "Test1")]
    public void TestParseSingle(string comment, string context)
    {
    }

    [TestMethod]
    [DataRow("// context: Test1, Test2", "Test1")]
    [DataRow("// context: Test1, Test2", "Test2")]
    public void TestParseDouble(string comment, string context)
    {
    }
}