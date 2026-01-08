using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

// context: signature, build
public abstract class SignatureBuilder<TSymbol>
    where TSymbol: notnull
{
    public bool IncludeGenerics;
    public bool IncludeParameters;
    public bool IncludeReturnType;
    public bool IncludeNamespace;
    public bool IncludeContainingType;
    public bool QualifyTypes;

    protected SignatureBuilder()
    {
    }

    protected SignatureBuilder(SignatureBuilder<TSymbol> source)
    {
        IncludeContainingType = source.IncludeContainingType;
        IncludeParameters = source.IncludeParameters;
        IncludeReturnType = source.IncludeReturnType;
        IncludeGenerics = source.IncludeGenerics;
        QualifyTypes = source.QualifyTypes;
    }

    public SignatureBuilder<TSymbol> SetIncludeGenerics(bool value)
    {
        IncludeGenerics = value;
        return this;
    }

    public SignatureBuilder<TSymbol> SetIncludeParameters(bool value)
    {
        IncludeParameters = value;
        return this;
    }

    public SignatureBuilder<TSymbol> SetIncludeReturnType(bool value)
    {
        IncludeReturnType = value;
        return this;
    }

    public SignatureBuilder<TSymbol> SetIncludeNamespace(bool value)
    {
        IncludeNamespace = value;
        return this;
    }

    public SignatureBuilder<TSymbol> SetIncludeContainingType(bool value)
    {
        IncludeContainingType = value;
        return this;
    }

    public SignatureBuilder<TSymbol> SetTypeQualification(bool value)
    {
        QualifyTypes = value;
        return this;
    }

    // context: signature, build
    public abstract string Build(TSymbol symbol);
}
