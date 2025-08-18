using System.Reflection;

namespace CommandlineKit;

// context: commandline, build
internal static class CommandLineNodeIterator
{
    private const string SParsingErrorUnknownPropertyMessage = "Ошибка: Неизвестное свойство '{0}' в пути '{1}'.";
    private const string SParsingErrorCantInstanciateObjectMessage = "Ошибка: Не удалось создать экземпляр для типа '{0}'.";

    // context: commandline, update
    public static void SetNestedPropertyValue<T>(T targetObject, string[] propertyPath, string value)
    {
        object? currentObject = targetObject;
        PropertyInfo? currentProperty = null;

        for(int i = 0; i < propertyPath.Length; i++)
        {
            // Находим свойство по имени
            currentProperty = currentObject?.GetType().GetProperty(
                propertyPath[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if(currentProperty == null)
            {
                throw new ArgumentException(string.Format(SParsingErrorUnknownPropertyMessage, propertyPath[i], string.Join(".", propertyPath)));
            }

            // Если это не последнее свойство в пути, создаём вложенный объект
            if(i < propertyPath.Length - 1)
            {
                object? nestedObject = currentProperty.GetValue(currentObject);
                if(nestedObject == null)
                {
                    // Если вложенный объект null, создаём его
                    if(currentProperty.PropertyType.IsClass)
                    {
                        nestedObject = Activator.CreateInstance(currentProperty.PropertyType);
                        currentProperty.SetValue(currentObject, nestedObject);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(SParsingErrorCantInstanciateObjectMessage, currentProperty.PropertyType.Name));
                    }
                }
                currentObject = nestedObject;
            }
            else
            {
                // Если это последнее свойство, устанавливаем значение
                object convertedValue = CommandLineArgumentValueConverter.ConvertValue(currentProperty.PropertyType, value);
                currentProperty.SetValue(currentObject, convertedValue);
            }
        }
    }
}