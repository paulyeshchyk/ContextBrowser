using System.Collections.Generic;
using System.Linq;
using UmlKit.Builders.Model;

namespace UmlKit.DataProviders;

public class GrouppedSortedTransitionList
{
    private readonly Dictionary<string, SortedList<int, UmlTransitionDto>> _dataset;

    public GrouppedSortedTransitionList()
    {
        _dataset = new Dictionary<string, SortedList<int, UmlTransitionDto>>();
    }

    public GrouppedSortedTransitionList(GrouppedSortedTransitionList initialData) : this(initialData.GetDataset())
    {
    }

    public GrouppedSortedTransitionList(IReadOnlyDictionary<string, SortedList<int, UmlTransitionDto>> initialData)
    {
        _dataset = new Dictionary<string, SortedList<int, UmlTransitionDto>>();
        foreach (var pair in initialData)
        {
            _dataset.Add(pair.Key, new SortedList<int, UmlTransitionDto>(pair.Value));
        }
    }

    public IReadOnlyDictionary<string, SortedList<int, UmlTransitionDto>> GetDataset() => _dataset;

    public void Add(UmlTransitionDto transitionDto, string key)
    {
        if (!_dataset.TryGetValue(key, out var transitionList))
        {
            transitionList = new SortedList<int, UmlTransitionDto>();
            _dataset.Add(key, transitionList);
        }

        transitionList.Add(transitionList.Count, transitionDto);
    }

    public SortedList<int, UmlTransitionDto>? GetTransitionList(string key)
    {
        return _dataset.TryGetValue(key, out var result) ? result : null;
    }

    public IEnumerable<string> GetAllKeys()
    {
        return _dataset.Keys;
    }

    public List<UmlTransitionDto> GetTransitionList()
    {
        return _dataset
            .OrderBy(pair => pair.Key)
            .SelectMany(pair => pair.Value.Values)
            .ToList();
    }

    public bool HasTransitions(string key)
    {
        return _dataset.TryGetValue(key, out var list) && list.Any();
    }

    public bool HasTransitions()
    {
        return _dataset.Any(pair => pair.Value.Any());
    }

    public void Merge(GrouppedSortedTransitionList sourceB)
    {
        var _finalDataset = Concat(sourceB);
        _dataset.Clear();
        foreach (var pair in _finalDataset)
        {
            _dataset.Add(pair.Key, new SortedList<int, UmlTransitionDto>(pair.Value));
        }
    }

    public GrouppedSortedTransitionList Copy(GrouppedSortedTransitionList sourceB)
    {
        return new GrouppedSortedTransitionList(Concat(sourceB));
    }

    public Dictionary<string, SortedList<int, UmlTransitionDto>> Concat(GrouppedSortedTransitionList sourceB)
    {
        var allKeys = _dataset.Keys.Union(sourceB.GetDataset().Keys);

        return allKeys.ToDictionary(
            key => key,
            key =>
            {
                var combinedList = new SortedList<int, UmlTransitionDto>();

                if (sourceB.GetDataset().TryGetValue(key, out var listB))
                {
                    foreach (var item in listB.Values)
                    {
                        combinedList.Add(combinedList.Count, item);
                    }
                }

                if (_dataset.TryGetValue(key, out var listA))
                {
                    foreach (var item in listA.Values)
                    {
                        combinedList.Add(combinedList.Count, item);
                    }
                }
                return combinedList;
            });
    }

    public List<UmlTransitionDto> ConcatToList(GrouppedSortedTransitionList sourceB)
    {
        var resultList = new List<UmlTransitionDto>();

        foreach (var list in sourceB.GetDataset().Values)
        {
            resultList.AddRange(list.Values);
        }

        var uniqueKeysFromA = _dataset.Keys.Except(sourceB.GetDataset().Keys);
        foreach (var key in uniqueKeysFromA)
        {
            resultList.AddRange(_dataset[key].Values);
        }

        return resultList;
    }
}
