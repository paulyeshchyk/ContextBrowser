using System;
using System.Collections.Generic;
using ContextKit.ContextData.Naming;

namespace GraphKit.Walkers;

/// <summary>
/// # Потенциальные проблемы и ограничения
///
/// ## Память (HashSet<TItem> visited)
///
/// При очень больших графах память будет расти, т.к.мы храним visited для каждого пути.
/// Сделано visited.Remove(current) после обхода детей -> это значит, что память не будет утекать вширь, но на очень глубоких деревьях нагрузка останется.
/// Если граф реально «вселенский» (десятки тысяч нод), нужно будет думать о батчевом обходе или потоковой обработке.
///
/// ## Глубина рекурсии
/// Сейчас обходчик рекурсивный(TraverseInternal вызывает сам себя).
/// Для глубины ~40–200 это не проблема.
/// Но если когда-нибудь прилетит граф на 10 000 уровней вглубь словим StackOverflowException.
///
/// ### Решение:
/// сделать итеративный обход через Stack<(item, node, state)>.
///
/// ## Циклы помечаются только через IsCycle
/// В текущей версии цикл не разрывается автоматически — нода создаётся и помечается IsCycle.
/// Для визуализации нормально, но, если потом кто-то начнёт без проверки IsCycle снова обходить Children, можно опять уйти в бесконечный цикл.
///
/// ### Рекомендация
/// при IsCycle == true не добавлять детей вообще, иначе кто-то может забыть фильтровать.
///
/// ## Свойство IsCycle делается через Reflection
/// Сейчас оно ищется через typeof(TNode).GetProperty("IsCycle").
/// Это медленно, но удобно.
/// Если обход будет массовым и частым, лучше сделать IHasCycleFlag интерфейс, или передавать делегат markCycleNode(node) снаружи.
///
/// ## Порядок обхода (DFS)
/// Алгоритм сейчас DFS(Depth-First Search).
/// Это значит, что одна ветка обходится до конца, только потом берётся следующая.
///
/// ## Параллельность
/// Обходчик сейчас однопоточный.
/// Если нужна многопоточность - нужно будет защищать его lock’ом или делать ConcurrentDictionary.
/// </summary>
public static class DfsWalker_Traversal
{
    public static List<TNode> Run<TItem, TNode>(
        IEnumerable<TItem> startItems,
        Func<TItem, IEnumerable<TItem>> nextSelector,
        Func<TItem, INamingProcessor, TNode> createNode,
        Action<TNode, TNode> linkNodes,
        INamingProcessor namingProcessor)
        where TNode : class
    {
        var results = new List<TNode>();
        var visited = new HashSet<TItem>();

        foreach (var item in startItems)
        {
            var node = TraverseInternal(item, nextSelector, createNode, linkNodes, visited, namingProcessor);
            if (node != null)
                results.Add(node);
        }

        return results;
    }

    private static TNode? TraverseInternal<TItem, TNode>(
        TItem current,
        Func<TItem, IEnumerable<TItem>> nextSelector,
        Func<TItem, INamingProcessor, TNode> createNode,
        Action<TNode, TNode> linkNodes,
        HashSet<TItem> visited,
        INamingProcessor namingProcessor)
        where TNode : class
    {
        if (!visited.Add(current))
        {
            // нашли цикл -> создаём ноду и помечаем её как IsCycle (если поддерживает)
            var cycleNode = createNode(current, namingProcessor);
            var prop = typeof(TNode).GetProperty("IsCycle");
            if (prop != null && prop.PropertyType == typeof(bool))
                prop.SetValue(cycleNode, true);

            return cycleNode;
        }

        var node = createNode(current, namingProcessor);

        foreach (var childItem in nextSelector(current))
        {
            var childNode = TraverseInternal(childItem, nextSelector, createNode, linkNodes, visited, namingProcessor);
            if (childNode != null)
                linkNodes(node, childNode);
        }

        visited.Remove(current); // важно: siblings должны строиться независимо
        return node;
    }
}