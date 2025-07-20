using ContextBrowser.Parser.csharp;

namespace ContextBrowser.Parser;

// Интерфейс одного шагового парсера
interface ILineParser
{
    // Возвращает true, если этот парсер «забрал» строку
    bool TryParse(string line, ParseContext ctx);
}

