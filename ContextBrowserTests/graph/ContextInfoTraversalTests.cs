using ContextBrowser.ContextKit.graph;
using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.graph.Tests;

[TestClass]
public class ContextInfoTraversalTests
{
    private static List<ContextInfo> DefaultContextList()
    {
        var a = new ContextInfo { Name = "A", Domains = { "DomainA" } };
        var b = new ContextInfo { Name = "B", Domains = { "DomainB" } };
        var c = new ContextInfo { Name = "C", Domains = { "DomainC" } };
        var d = new ContextInfo { Name = "D" };
        var e = new ContextInfo { Name = "E" };
        var f = new ContextInfo { Name = "F" };
        var g = new ContextInfo { Name = "G", Domains = { "DomainG" } };

        c.References.Add(d);
        d.References.Add(e);
        e.References.Add(f);
        f.References.Add(g);

        return [a, b, c, d, e, f, g];
    }

    /// <summary>
    /// Начинаем обход с домена [startFromDomain]<br/>
    /// Ожидаем дойти до узла[targetName]<br/>
    /// Во время обхода проходим узлы [nodes]<br/>
    /// </summary>
    /// <param name="startFromDomain"></param>
    /// <param name="nodes"></param>
    /// <param name="targetName"></param>
    [TestMethod]
    [DataRow("DomainC", "C,D,E,F,G", "G")]
    public void Test_WalkFromDomain_C_to_G(string startFromDomain, string nodes, string targetName)
    {
        var all = DefaultContextList();
        var walker = new DomainWalker(startFromDomain);
        walker.Walk(all);

        var targetItem = all.Where(i => i.Name?.Equals(targetName) ?? false).First();

        // Проверим, что дошли до Target
        Assert.IsTrue(walker.Visited.Contains(targetItem));

        // Проверим, что в обход попали только нужные узлы
        var visitedNames = walker.Visited.Select(v => v.Name).ToHashSet();
        CollectionAssert.AreEquivalent(nodes.Split(","), visitedNames.ToList());
    }
}
