using System;
using System.Collections.Generic;
using SemanticKit.Model.Signature;

namespace SemanticKit.Model.Signature;

public record SignatureParsingResult
{
    public SignatureDefault Result { get; }

    public bool Success { get; }

    private SignatureParsingResult(SignatureDefault result, bool success)
    {
        Result = result;
        Success = success;
    }

    public static SignatureParsingResult SuccessResult(SignatureDefault result)
    {
        return new SignatureParsingResult(result, true);
    }

    public static SignatureParsingResult FailureResult()
    {
        // При неудаче можно использовать пустой или null SignatureDefault,
        // но лучше просто вернуть Fail и не инициализировать Result.
        // Здесь мы вернем "пустую" структуру, т.к. нас интересует только Success = false.
        return new SignatureParsingResult(default, false);
    }

    public static SignatureParsingResult FailureResult(SignatureDefault defaultResult)
    {
        // Может использоваться для возврата 'дефолтного' результата при неудаче
        return new SignatureParsingResult(defaultResult, false);
    }
}
