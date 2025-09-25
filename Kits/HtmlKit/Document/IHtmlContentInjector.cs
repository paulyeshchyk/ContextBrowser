namespace HtmlKit.Document;

public interface IHtmlContentInjector<TTensor>
    where TTensor : notnull
{
    string Inject(TTensor container, int cnt);
}
