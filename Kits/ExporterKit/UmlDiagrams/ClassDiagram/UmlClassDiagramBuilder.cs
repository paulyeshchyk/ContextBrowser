using ContextKit.Model;
using UmlKit.Model;
using UmlKit.PlantUmlSpecification;

namespace UmlKit.UmlDiagrams.ClassDiagram;

internal static class UmlClassDiagramBuilder
{
    //context: uml, build, classdiagram
    public static void Build(UmlClassDiagram diagram, IEnumerable<UmlClassDiagramElementDto> allElements)
    {
        var namespaces = allElements.GroupBy(e => e.Namespace);

        foreach (var nsGroup in namespaces)
        {
            var package = new UmlPackage(nsGroup.Key);
            diagram.Add(package);

            var classes = nsGroup.GroupBy(e => e.ClassName);

            foreach (var classGroup in classes)
            {
                var umlClass = new UmlClass(classGroup.Key);
                package.Add(umlClass);

                foreach (var element in classGroup)
                {
                    var umlMethod = new UmlMethod(element.Method.Name);
                    umlClass.Add(umlMethod);
                }
            }
        }
    }

    public static void BuildSquaredLayout(UmlClassDiagram diagram, IEnumerable<UmlClassDiagramElementDto> allElements)
    {
        var packageList = allElements.GroupBy(e => e.Namespace).Distinct().Select(e => e.Key).ToArray();
        int packageCount = packageList.Count();

        // Определяем количество колонок, например, по квадратному корню
        int columns = (int)Math.Ceiling(Math.Sqrt(packageCount));

        // Или задаём фиксированное количество колонок для контроля
        // int columns = 2;
        for (int i = 0; i < packageCount; i++)
        {
            var currentPackage = packageList[i];

            // Горизонтальные связи: соединяем текущий пакет с пакетом справа
            if ((i + 1) % columns != 0 && (i + 1) < packageCount)
            {
                var nextPackage = packageList[i + 1];
                diagram.Add(new UmlComponentRelation(currentPackage, nextPackage, UmlArrowDirection.ToRight, "hidden"));
            }

            // Вертикальные связи: соединяем текущий пакет с пакетом ниже
            if ((i + columns) < packageCount)
            {
                var nextRowPackage = packageList[i + columns];
                diagram.Add(new UmlComponentRelation(currentPackage, nextRowPackage, UmlArrowDirection.ToLeft, "hidden"));
            }
        }

    }
}
