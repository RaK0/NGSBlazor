using NGSBlazor.Client.Interfaces;

namespace NGSBlazor.Client.LocalItems
{
    public class LanguageLocalItem : ILocalItem
    {
        public string? Code { get; set; }

        string ILocalItem.Name => "Language";
    }
}
