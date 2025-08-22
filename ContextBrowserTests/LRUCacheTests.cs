namespace ContextBrowserTests;

[TestClass]
public class LRUCacheTests
{
    [TestMethod]
    public void Add_Item_ShouldBeStored()
    {
        var cache = new LRUCache<int, string>(2);
        cache.Add(1, "one");

        Assert.IsTrue(cache.TryGetValue(1, out var value));
        Assert.AreEqual("one", value);
    }

    [TestMethod]
    public void Add_MoreThanCapacity_ShouldEvictOldest()
    {
        var cache = new LRUCache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three"); // вытеснит 1

        Assert.IsFalse(cache.TryGetValue(1, out _));
        Assert.IsTrue(cache.TryGetValue(2, out var v2) && v2 == "two");
        Assert.IsTrue(cache.TryGetValue(3, out var v3) && v3 == "three");
    }

    [TestMethod]
    public void Get_UpdatesUsageOrder()
    {
        var cache = new LRUCache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        // обращаемся к 1 → теперь 2 будет вытеснено
        Assert.IsTrue(cache.TryGetValue(1, out _));

        cache.Add(3, "three");

        Assert.IsTrue(cache.TryGetValue(1, out _));
        Assert.IsFalse(cache.TryGetValue(2, out _)); // 2 вытеснился
    }

    [TestMethod]
    public void Add_SameKey_ShouldUpdateValueAndOrder()
    {
        var cache = new LRUCache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(1, "updated");

        Assert.IsTrue(cache.TryGetValue(1, out var value));
        Assert.AreEqual("updated", value);
        Assert.AreEqual(1, cache.Count);
    }

    [TestMethod]
    public void Clear_ShouldEmptyCache()
    {
        var cache = new LRUCache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        cache.Clear();

        Assert.AreEqual(0, cache.Count);
        Assert.IsFalse(cache.TryGetValue(1, out _));
    }
}
