namespace NServiceKit.DesignPatterns.Translator
{
    public interface ITranslator<To, From>
    {
        To Parse(From from);
    }
}