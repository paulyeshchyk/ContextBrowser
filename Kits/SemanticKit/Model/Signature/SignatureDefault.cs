namespace SemanticKit.Model.Signature;

public record struct SignatureDefault(string ResultType,
                                           string Namespace,
                                           string ClassName,
                                           string MethodName,
                                           string Arguments,
                                           string Raw) : ISignature
{
    public readonly bool Equals(ISignature customMethodSignature)
    {
        if (customMethodSignature is not SignatureDefault methodSignature)
            return false;

        var result = methodSignature.Raw.Equals(Raw);
        return result;
    }
}