using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using ExporterKit.Uml.ClassDiagram;
using UmlKit;

namespace ExporterKit.Uml.ClassDiagram;

internal static class UmlClassDiagramDataMapper
{
    public static IEnumerable<UmlClassDiagramElementDto> Map(List<ContextInfo> methods)
    {
        var result = methods.Select(m =>
        {
            string namespaceName;
            string className;

            if (m.ElementType != ContextInfoElementType.method && m.ElementType != ContextInfoElementType.@delegate)
            {
                namespaceName = m.Namespace;
                className = m.Name;
                return new UmlClassDiagramElementDto(namespaceName, className, m);
            }

            // Case 1: Method has a ClassOwner.
            if (!string.IsNullOrEmpty(m.ClassOwner?.FullName))
            {
                namespaceName = m.ClassOwner.Namespace;
                className = m.ClassOwner.Name;//m.ClassOwner.FullName;
                return new UmlClassDiagramElementDto(namespaceName, className, m);
            }

            // Case 2: Method has no ClassOwner. Parse from FullName.
            var parts = m.FullName.Split('.');
            if (parts.Length < 2)
            {
                // Handle edge case for malformed names
                namespaceName = "GLOBAL";
                className = "UnknownClass";
                return new UmlClassDiagramElementDto(namespaceName, className, m);
            }

            // The class name is the second to last part
            className = string.Join(".", parts.Take(parts.Length - 1));

            // The method name is the last part
            // For namespace, we need to extract from the class name.
            var classNameParts = className.Split('.');
            namespaceName = string.Join(".", classNameParts.Take(classNameParts.Length - 1));

            var theClassNameParts = className.Split('.');
            if (theClassNameParts == null || theClassNameParts.Length == 0)
            {
                return new UmlClassDiagramElementDto(namespaceName, "UnknownClass", m);
            }

            var theClassName = string.Join(".", theClassNameParts[^1]);
            return new UmlClassDiagramElementDto(namespaceName, theClassName, m);
        });
        return result;
    }
}
