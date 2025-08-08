using ContextBrowser.DiagramFactory.Renderer.Model;
using ContextKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.DiagramFactory.Builders.TransitionDirectionBuilder;

public class GroupedTransitionList : List<UmlTransitionDto>
{
}

public class SortedGrouppedTransitionList
{
    private SortedList<int, Dictionary<Guid, IEnumerable<UmlTransitionDto>>> _groupedTransitions =
        new SortedList<int, Dictionary<Guid, IEnumerable<UmlTransitionDto>>>();

    // Добавляем один переход. 
    // Предположим, что у UmlTransitionDto есть свойство Guid
    public void Add(int index, UmlTransitionDto transition)
    {
        // Проверяем, существует ли уже элемент с таким ключом index
        if(!_groupedTransitions.ContainsKey(index))
        {
            // Если нет, создаем новый словарь и добавляем его
            _groupedTransitions.Add(index, new Dictionary<Guid, IEnumerable<UmlTransitionDto>>());
        }

        // Получаем словарь для текущего индекса
        var transitionsForIndex = _groupedTransitions[index];

        // Предположим, что у UmlTransitionDto есть свойство Guid, по которому будем группировать
        var groupKey = transition.Uid ?? Guid.Empty; // Или любое другое свойство для группировки

        // Проверяем, существует ли уже группа для данного Guid
        if(transitionsForIndex.ContainsKey(groupKey))
        {
            // Если да, добавляем переход к существующей коллекции
            var currentTransitions = transitionsForIndex[groupKey].ToList();
            currentTransitions.Add(transition);
            transitionsForIndex[groupKey] = currentTransitions;
        }
        else
        {
            // Если нет, создаем новую группу
            transitionsForIndex.Add(groupKey, new List<UmlTransitionDto> { transition });
        }
    }

    public bool Any() => _groupedTransitions.Any();

    public IOrderedEnumerable<KeyValuePair<int, Dictionary<Guid, IEnumerable<UmlTransitionDto>>>> OrderBy<TKey>(Func<KeyValuePair<int, Dictionary<Guid, IEnumerable<UmlTransitionDto>>>, TKey> keySelector)
    {
        return _groupedTransitions.OrderBy(keySelector);
    }
}

public interface ITransitionBuilder
{
    DiagramDirection Direction { get; }

    GroupedTransitionList BuildTransitions(List<ContextInfo> domainMethods, List<ContextInfo> allContexts);
}
