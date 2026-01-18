using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CommandlineKit.Model;
using ContextBrowserKit.Commandline.Polyfills;

namespace CommandlineKit;

// context: commandline, build
public record HelpTarget(Type? TargetType = null, object? TargetInstance = null, string? Description = null);

public static class CommandLineHelpProducer
{
    public static bool ShowHelp<T>(string[] args, T? referenceInstance = null) where T : class
    {
        var appVersion = AppVersionReader.GetVersionFromGit();
        var helpPattern = $"{CommandLineDefaults.SArgumentPrefix}{CommandLineDefaults.SHelpKeyword}";
        int helpIndex = Array.FindIndex(args, a => a.Equals(helpPattern, StringComparison.OrdinalIgnoreCase));

        // Если параметров нет или явно запрошен help
        if (args.Length == 0 || helpIndex != -1)
        {
            string? path = (helpIndex != -1 && helpIndex + 1 < args.Length) ? args[helpIndex + 1] : null;
            bool expand = GetExpandFlag(args);
            HelpGenerator.GenerateHelp(referenceInstance, appVersion, path, expand);
            return true;
        }

        return false;
    }

    private static bool GetExpandFlag(string[] args)
    {
        int idx = Array.FindIndex(args, a => a.Equals("--expand", StringComparison.OrdinalIgnoreCase));
        if (idx != -1 && idx + 1 < args.Length && bool.TryParse(args[idx + 1], out var res))
            return res;
        return args.Any(a => a.Equals("--expand", StringComparison.OrdinalIgnoreCase));
    }
}

public static class HelpPrinter
{
    private const string Prefix = "--";

    public static void PrintProperty(PropertyInfo prop, object? instance, string path, string indent)
    {
        var attr = prop.GetCustomAttribute<CommandLineArgumentAttribute>()!;
        bool isComplex = TypeHelper.IsComplexType(prop.PropertyType);
        bool isRequired = prop.GetCustomAttribute<RequiredMemberAttribute>() != null ||
                         prop.GetCustomAttribute<RequiredAttribute>() != null;

        // Линия 1: Имя аргумента (выделяем ярко)
        string fullArgName = string.IsNullOrEmpty(path) || path == "Root" ? attr.Name : $"{path}.{attr.Name}";

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{indent}{Prefix}{fullArgName}");
        Console.ResetColor();

        // Линия 2: Описание (с отступом +2)
        string contentIndent = indent + "  ";

        if (isRequired)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{contentIndent}[ОБЯЗАТЕЛЬНО] ");
            Console.ResetColor();
        }
        else
        {
            Console.Write(contentIndent);
        }

        Console.WriteLine(attr.Description);

        // Линия 3: Значение по умолчанию (если есть)
        if (instance != null && !isComplex)
        {
            PrintValue(prop.GetValue(instance), contentIndent, fullArgName);
        }

        // Линия 4: Enum детали
        if (!isComplex)
        {
            PrintEnumDetails(prop.PropertyType, contentIndent);
        }

        Console.WriteLine(); // Пробел между блоками для читаемости
    }

    private static void PrintValue(object? val, string indent, string fullArgPath)
    {
        if (val == null)
            return;

        Console.ForegroundColor = ConsoleColor.DarkGray;

        // 1. Логика для списков
        if (val is System.Collections.IEnumerable enumerable && val is not string)
        {
            var list = enumerable.Cast<object>().ToList();
            if (list.Count > 0)
            {
                Console.WriteLine($"{indent}Текущий состав:");
                foreach (var item in list)
                    Console.WriteLine($"{indent}  - {item}");
            }
            else
            {
                Console.WriteLine($"{indent}Текущее значение: [Список пуст]");
            }
        }

        // 2. Логика для сложных объектов (Классы-реализации)
        else if (TypeHelper.IsComplexType(val.GetType()))
        {
            string className = val.GetType().Name;
            Console.WriteLine($"{indent}Реализация: {className}");

            // ПОДСКАЗКА ДЛЯ НАВИГАЦИИ
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{indent}Подробности: --help {fullArgPath}");
        }

        // 3. Логика для простых значений
        else
        {
            string strVal = val.ToString() ?? string.Empty;
            if (strVal.Length > 60)
            {
                Console.WriteLine($"{indent}Текущее значение:");
                Console.WriteLine($"{indent}  {strVal}");
            }
            else
            {
                Console.WriteLine($"{indent}Текущее значение: {strVal}");
            }
        }

        Console.ResetColor();
    }

    private static void PrintEnumDetails(Type t, string indent)
    {
        Type actual = Nullable.GetUnderlyingType(t) ?? t;
        if (!actual.IsEnum)
            return;

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"{indent}Допустимые варианты:");

        foreach (var name in Enum.GetNames(actual))
        {
            var field = actual.GetField(name);
            var desc = field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            string display = string.IsNullOrEmpty(desc) ? name : $"{name,-15} # {desc}";
            Console.WriteLine($"{indent}  * {display}");
        }
        Console.ResetColor();
    }
}

public static class TypeHelper
{
    public static bool IsComplexType(Type t)
    {
        // Тип сложный, если это класс, но не строка и не коллекция простых типов
        if (t == typeof(string))
            return false;

        // Если это коллекция, проверяем тип её элемента
        if (IsCollection(t))
        {
            var elementType = GetCollectionElementType(t);
            return elementType.IsClass && elementType != typeof(string);
        }

        return t.IsClass;
    }

    /// <summary>
    /// Проверяет, является ли тип коллекцией (но не строкой)
    /// </summary>
    public static bool IsCollection(Type t)
    {
        if (t == typeof(string))
            return false;
        return typeof(System.Collections.IEnumerable).IsAssignableFrom(t);
    }

    /// <summary>
    /// Извлекает тип элемента коллекции (для массивов или Generic списков)
    /// </summary>
    public static Type GetCollectionElementType(Type t)
    {
        // Если это массив (T[])
        if (t.IsArray)
            return t.GetElementType() ?? typeof(object);

        // Если это Generic коллекция (List<T>, IEnumerable<T>)
        if (t.IsGenericType)
            return t.GetGenericArguments().FirstOrDefault() ?? typeof(object);

        // Если это необобщенная коллекция (ArrayList), возвращаем object
        return typeof(object);
    }
}

public static class HelpGenerator
{
    public static void GenerateHelp(object? referenceInstance, string appVersion, string? path = null, bool expand = false)
    {
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine($"СПРАВКА ПО СИСТЕМЕ КОНФИГУРАЦИИ (v{appVersion})");
        Console.WriteLine(new string('=', 60));

        var resolution = ResolveTargetByPath(referenceInstance, path);

        if (resolution.TargetType != null)
        {
            GenerateHelpRecursive(resolution, expand, path ?? "Root", 0);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: Элемент по пути '{path}' не найден в структуре настроек.");
            Console.ResetColor();
        }
        PrintFooter();
    }

    private static void GenerateHelpRecursive(HelpTarget target, bool expand, string path, int depth)
    {
        if (target.TargetType == null)
            return;

        string indent = new string(' ', depth * 4);

        // Печатаем заголовок только если это не корень или мы в режиме детализации
        if (depth > 0 && !expand)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{indent}>>> РАЗДЕЛ: {path}");
            if (!string.IsNullOrEmpty(target.Description))
                Console.WriteLine($"{indent}    {target.Description}");
            Console.ResetColor();
        }

        var properties = target.TargetType.GetProperties()
            .Where(p => p.GetCustomAttribute<CommandLineArgumentAttribute>() != null)
            .OrderByDescending(p => p.GetCustomAttribute<RequiredMemberAttribute>() != null);

        if (properties == null || properties.Count() == 0)
        {
            PrintNoHelpDefinedYet(indent);
            return;
        }

        foreach (var prop in properties)
        {
            HelpPrinter.PrintProperty(prop, target.TargetInstance, path, indent);

            bool isComplex = TypeHelper.IsComplexType(prop.PropertyType);

            if (expand && isComplex)
            {
                // 1. Получаем значение 
                object? nextInstance = null;
                try
                {
                    nextInstance = target.TargetInstance != null ? prop.GetValue(target.TargetInstance) : null;
                }
                catch
                {
                }

                // 2. Определяем тип
                var attr = prop.GetCustomAttribute<CommandLineArgumentAttribute>()!;
                Type nextType = attr.ImplementationType ?? nextInstance?.GetType() ?? prop.PropertyType;

                // 3. Обработка коллекций
                if (TypeHelper.IsCollection(nextType))
                {
                    nextType = TypeHelper.GetCollectionElementType(nextType);
                    if (nextInstance is System.Collections.IEnumerable en)
                    {
                        var first = en.Cast<object>().FirstOrDefault();
                        if (first != null)
                        {
                            nextInstance = first;
                            nextType = first.GetType();
                        }
                    }
                }

                // Заходим в рекурсию только если в целевом типе есть хотя бы один [CommandLineArgument]
                bool hasNestedArgs = nextType.GetProperties()
                    .Any(p => p.GetCustomAttribute<CommandLineArgumentAttribute>() != null);

                if (hasNestedArgs)
                {
                    var subPath = (path == "Root" || string.IsNullOrEmpty(path)) ? attr.Name : $"{path}.{attr.Name}";
                    var nextTarget = new HelpTarget(nextType, nextInstance, attr.Description);
                    GenerateHelpRecursive(nextTarget, true, subPath, depth + 1);
                }
            }
        }
    }

    private static void PrintNoHelpDefinedYet(string indent)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{indent}  (Для этого раздела пока не описано доступных параметров)");
        Console.ResetColor();
    }

    private static HelpTarget ResolveTargetByPath(object? rootInstance, string? path)
    {
        if (rootInstance == null)
            return new HelpTarget();

        object? currentObj = rootInstance;
        Type currentType = rootInstance.GetType();
        string? lastDesc = null;

        if (string.IsNullOrEmpty(path))
            return new HelpTarget(currentType, currentObj);

        foreach (var part in path.Split('.'))
        {
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(currentType) && currentType != typeof(string))
            {
                currentObj = (currentObj as System.Collections.IEnumerable)?.Cast<object>().FirstOrDefault();
                if (currentObj != null)
                    currentType = currentObj.GetType();
            }

            var prop = currentType.GetProperties().FirstOrDefault(p =>
                p.GetCustomAttribute<CommandLineArgumentAttribute>()?.Name.Equals(part, StringComparison.OrdinalIgnoreCase) ?? false);

            if (prop == null)
                return new HelpTarget();

            var attr = prop.GetCustomAttribute<CommandLineArgumentAttribute>();
            lastDesc = attr?.Description;
            currentObj = currentObj != null ? prop.GetValue(currentObj) : null;
            currentType = attr?.ImplementationType ?? currentObj?.GetType() ?? prop.PropertyType;
        }

        return new HelpTarget(currentType, currentObj, lastDesc);
    }

    private static void PrintFooter()
    {
        Console.WriteLine(new string('-', 60));
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("СОВЕТЫ ПО НАВИГАЦИИ:");

        Console.ResetColor();
        Console.WriteLine($"  • Используйте --help <путь> для детального изучения раздела.");

        //Console.WriteLine($"  • Добавьте --expand true для вывода всех вложенных параметров сразу.");

        Console.WriteLine("  • Путь можно копировать из строки 'Подробности' в описании параметра.");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nПример: ContextBrowser.exe --help appOptions.classifier");
        Console.ResetColor();
        Console.WriteLine(new string('=', 60));
    }
}