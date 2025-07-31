#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    //namespace System.Runtime.CompilerServices

    // Атрибут, необходимый компилятору для функции 'required'
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }
    }

    // Атрибут, который компилятор добавляет к свойствам с модификатором 'required'
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class RequiredMemberAttribute : Attribute
    {
    }
}
#endif