using NGSBlazor.Client.Interfaces;

namespace NGSBlazor.Client.LocalItems
{
    public class LanguageLocalItem : ILocalItem
    {
        public string Name => "Language";
        public string? Code { get; set; }
    }
}
