using NGSBlazor.Client.Interfaces;

namespace NGSBlazor.Client.LocalItems
{
    public class BearerLocalItem : ILocalItem
    {
        public string Name => "Bearer";
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
