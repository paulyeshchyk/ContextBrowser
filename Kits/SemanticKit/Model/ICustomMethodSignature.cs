using System.Text.RegularExpressions;

namespace SemanticKit.Model;

public interface ICustomMethodSignature
{
    string ResultType { get; set; }

    string Namespace { get; set; }

    string ClassName { get; set; }

    string MethodName { get; set; }

    string Arguments { get; set; }

    string Raw { get; set; }

    void Deconstruct(out string ResultType, out string Namespace, out string ClassName, out string MethodName, out string Arguments, out string Raw);

    bool Equals(object obj);

    bool Equals(ICustomMethodSignature other);

    int GetHashCode();

    string ToString();
}
