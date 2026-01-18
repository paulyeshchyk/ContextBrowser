using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GSCore.Extensions;

public static class DeepCloneExtension
{
    public static void DeepClone<T>(this T self, T from, DeepCloneRequest? request = null)
    where T : notnull
    {
        DeepCloneInternal(self, from, request ?? DeepCloneRequest.Default());
    }

    public static void DeepClone(this object self, object from, DeepCloneRequest? request = null)
    {
        DeepCloneInternal(self, from, request ?? DeepCloneRequest.Default());
    }

    public static object DeepClone(this object self, DeepCloneRequest? request = null)
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));

        var result = Activator.CreateInstance(self.GetType());
        if (result == null)
            throw new ArgumentNullException(nameof(self));

        result.DeepClone(self, request);
        return result;
    }

    public static T DeepClone<T>(this object self, DeepCloneRequest? request = null)
    where T : notnull
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));

        var result = Activator.CreateInstance(typeof(T));
        if (result == null)
            throw new ArgumentNullException(nameof(self));

        result.DeepClone(self, request);
        return (T)result;
    }

    private static void DeepCloneInternal(object self, object from, DeepCloneRequest request)
    {
        if (self == null)
            throw new ArgumentNullException(nameof(self));
        if (from == null)
            throw new ArgumentNullException(nameof(from));

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new ExceptPropertiesResolver(request),
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            NullValueHandling = NullValueHandling.Ignore
        };

        // Копирование всех объектов, включая вложенные
        var serializedObject = JsonConvert.SerializeObject(from, settings);

        // При копировании вложенных объектов игнорируем режим, чтобы они всегда копировались
        JsonConvert.PopulateObject(serializedObject, self, settings);
    }

    private class ExceptPropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _exceptFieldNames;
        private readonly PropertyComparisonMode _mode;

        public ExceptPropertiesResolver(DeepCloneRequest request)
        {
            _exceptFieldNames = (request.ExceptFieldNames ?? new HashSet<string>()).ToHashSet();
            _mode = request.Mode;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperties(type, memberSerialization)
                             .Where(p => !_exceptFieldNames.Contains(p.PropertyName!))
                             .Where(p => ShouldIncludeProperty(p.PropertyType!))
                             .ToList();
            return result;
        }

        private bool ShouldIncludeProperty(Type type)
        {
            return _mode switch
            {
                PropertyComparisonMode.SimpleTypesOnly => type.IsPrimitive || type == typeof(string) || type == typeof(decimal),
                PropertyComparisonMode.All => true,
                _ => throw new NotImplementedException()
            };
        }
    }
}

public readonly struct DeepCloneRequest
{
    public IEnumerable<string>? ExceptFieldNames { get; }

    public PropertyComparisonMode Mode { get; }

    public DeepCloneRequest(IEnumerable<string>? exceptFieldNames, PropertyComparisonMode mode)
    {
        ExceptFieldNames = exceptFieldNames;
        Mode = mode;
    }

    public static DeepCloneRequest Default() => new(null, PropertyComparisonMode.All);
}

public enum PropertyComparisonMode
{
    SimpleTypesOnly, // int, string, bool и т. д.
    All // Все свойства (по умолчанию)
}