using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using UmlKit.Builders.Model;

namespace ExporterKit.Uml.Model;

public static class UmlDiagramMaxNamelengthExtractor
{
    public static int Extract(IEnumerable<UmlClassDiagramElementDto> allElements, HashSet<UmlDiagramMaxNamelengthExtractorType> typesToConsider)
    {
        var lengths = new List<int>();

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@namespace) && allElements.Any())
        {
            lengths.Add(allElements.Max(e => e.Namespace.Length));
        }

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@entity) && allElements.Any())
        {
            lengths.Add(allElements.Max(e => e.ClassName.Length));
        }

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@method) && allElements.Any())
        {
            var l = allElements.Where(e => e.ContextInfo.ElementType == ContextInfoElementType.method);
            if (l.Any())
            {
                lengths.Add(l.Max(m => m.ContextInfo.Name.Length));
            }
        }

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@property) && allElements.Any())
        {
            var l = allElements.Where(e => e.ContextInfo.ElementType == ContextInfoElementType.property);
            if (l.Any())
            {
                lengths.Add(l.Max(m => m.ContextInfo.Name.Length));
            }
        }
        return lengths.Any() ? lengths.Max() : 0;
    }

    public static int Extract(IEnumerable<IContextInfo> allElements, HashSet<UmlDiagramMaxNamelengthExtractorType> typesToConsider)
    {
        var lengths = new List<int>();

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@namespace) && allElements.Any())
        {
            lengths.Add(allElements.Max(e => e.Namespace.Length));
        }

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@entity) && allElements.Any())
        {
            var l = allElements.Where(e => e.ElementType == ContextInfoElementType.@record || e.ElementType == ContextInfoElementType.@class || e.ElementType == ContextInfoElementType.@struct);
            if (l.Any())
            {
                lengths.Add(l.Max(e => e.Name.Length));
            }
        }

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@method) && allElements.Any())
        {
            var l = allElements.Where(e => e.ElementType == ContextInfoElementType.method);
            if (l.Any())
            {
                lengths.Add(l.Max(m => m.Name.Length));
            }
        }

        if (typesToConsider.Contains(UmlDiagramMaxNamelengthExtractorType.@property) && allElements.Any())
        {
            var l = allElements.Where(e => e.ElementType == ContextInfoElementType.property);
            if (l.Any())
            {
                lengths.Add(l.Max(m => m.Name.Length));
            }
        }

        return lengths.Any() ? lengths.Max() : 0;
    }
}

public enum UmlDiagramMaxNamelengthExtractorType
{
    @namespace,
    @entity,
    @method,
    @property
}